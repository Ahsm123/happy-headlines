using HappyHeadlines.Contracts.Events;
using HappyHeadlines.ServiceDefaults;

namespace HappyHeadlines.ArticleApi.Messaging;

public sealed class ArticlePublishedWorker(IMessageClient messageClient, IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await messageClient.SubscribeAsync<ArticlePublishedEvent>("article-api", async message =>
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<ArticlePublishedEvent>>();
            await handler.HandleAsync(message, stoppingToken);
        }, stoppingToken);
    }
}
