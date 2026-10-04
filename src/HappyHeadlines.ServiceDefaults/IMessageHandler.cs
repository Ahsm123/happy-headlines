namespace HappyHeadlines.ServiceDefaults;

public interface IMessageHandler<in T>
{
    Task HandleAsync(T message, CancellationToken ct);
}
