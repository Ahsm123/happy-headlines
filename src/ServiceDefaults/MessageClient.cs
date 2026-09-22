using EasyNetQ;

namespace ServiceDefaults;

public class MessageClient(IBus bus) : IMessageClient
{
    private readonly Dictionary<string, IAsyncDisposable> _subscriptions = new();

    public Task PublishAsync<T>(T message, CancellationToken ct = default)
    {
        var task = bus.PubSub.PublishAsync(message, ct);
        return task;
    }

    public async Task SubscribeAsync<T>(string subscriberId, Func<T, Task> handler, CancellationToken ct = default)
    {
        if (_subscriptions.ContainsKey(subscriberId))
        {
            throw new ArgumentException($"The subscriber {subscriberId} is already subscribed");
        }
        var handle = await bus.PubSub.SubscribeAsync(subscriberId, handler, ct);
        _subscriptions[subscriberId] = handle;
    }

    public async Task UnsubscribeAsync(string subscriberId, CancellationToken ct = default)
    {
        if (_subscriptions.Remove(subscriberId, out var handle))
            await handle.DisposeAsync();
    }
}