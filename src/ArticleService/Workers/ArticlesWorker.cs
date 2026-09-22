using ServiceDefaults;
using ServiceDefaults.Contracts;

namespace ArticleService.Workers;

public class ArticlesWorker(IMessageClient messageClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await messageClient.SubscribeAsync<ArticleMessage>("ArticlesWorker", SaveNewArticles, ct);
    }

    private Task SaveNewArticles(ArticleMessage articleMessage)
    {
        MonitorService.Log.Here().Information("ArticleWorker received message");
        return Task.CompletedTask;
    }
}