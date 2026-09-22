namespace ServiceDefaults;

public interface IMessageClient
{
    Task PublishAsync<T>(T message, CancellationToken ct = default);
    Task SubscribeAsync<T>(string subscriberId, Func<T, Task> handler, CancellationToken ct = default);
    Task UnsubscribeAsync(string subscriberId, CancellationToken ct = default);
}