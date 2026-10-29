using HappyHeadlines.Contracts.Events;
using HappyHeadlines.ServiceDefaults;

namespace HappyHeadlines.NewsletterApi.Messaging;

public sealed class NewSubscriptionWorker(IMessageClient messageClient, IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageClient.SubscribeAsync<NewSubscriptionEvent>("newsletter-api", async message =>
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<NewSubscriptionEvent>>();
            await handler.HandleAsync(message, stoppingToken);
        }, stoppingToken);
    }
}
