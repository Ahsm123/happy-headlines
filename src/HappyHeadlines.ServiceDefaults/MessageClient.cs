using System.Diagnostics;
using System.Text;
using System.Text.Json;
using EasyNetQ;

namespace HappyHeadlines.ServiceDefaults;

public class MessageClient(IAdvancedBus bus) : IMessageClient
{
    public const string ActivitySourceName = "HappyHeadlines.Messaging";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    private readonly Dictionary<string, IAsyncDisposable> _subscriptions = new();

    public async Task PublishAsync<T>(T message, CancellationToken ct = default)
    {
        var properties = new MessageProperties { Headers = new Dictionary<string, object>() };
        var current = Activity.Current;
        if (current != null)
        {
            var activity = current;
            properties.Headers["Activity-Id"] = activity.Id;
        }

        var exchange = typeof(T).Name;
        var routingKey = typeof(T).Name;

        await bus.ExchangeDeclareAsync(exchange, cancellationToken: ct);

        var busMessage = new Message<T>(message, properties);
        await bus.PublishAsync(exchange, routingKey, mandatory: false, busMessage, ct);
    }

    public async Task SubscribeAsync<T>(string subscriberId, Func<T, Task> handler, CancellationToken ct = default)
    {
        if (_subscriptions.ContainsKey(subscriberId))
        {
            throw new ArgumentException($"The subscriber {subscriberId} is already subscribed");
        }

        var exchange = typeof(T).Name;
        var routingKey = typeof(T).Name;

        await bus.ExchangeDeclareAsync(exchange, cancellationToken: ct);
        var queue = await bus.QueueDeclareAsync(subscriberId, ct);
        await bus.QueueBindAsync(queue.Name, exchange, routingKey, cancellationToken: ct);

        var handle = await bus.ConsumeAsync(queue, async (body, properties, info) =>
        {
            var message = JsonSerializer.Deserialize<T>(body.Span)!;

            var hasActivity = properties.Headers.TryGetValue("Activity-Id", out var parentId);

            var encoder = Encoding.UTF8;
            string activityIdStr = parentId is byte[] bytes
                ? encoder.GetString(bytes)
                : parentId?.ToString() ?? string.Empty;

            using var activity = ActivitySource.StartActivity("Consume", ActivityKind.Consumer, activityIdStr);
            MonitorService.Log.Here().Information("Consuming {MessageType} from {Queue}", typeof(T).Name, queue);

            try
            {
                await handler(message);
            }
            catch (Exception ex)
            {
                MonitorService.Log.Here()
                    .Error(ex, "Failed to handle {MessageType} from {Queue}", typeof(T).Name, queue);
                throw;
            }

        });

        _subscriptions[subscriberId] = handle;
    }

    public async Task UnsubscribeAsync(string subscriberId, CancellationToken ct = default)
    {
        if (_subscriptions.Remove(subscriberId, out var handle))
            await handle.DisposeAsync();
    }
}
