using HappyHeadlines.ArticleApi.Caching;

namespace HappyHeadlines.ArticleApi.Workers;

public sealed class ArticleCacheWarmupWorker(ArticleCache cache) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        do
        {
            await cache.WarmUpAsync(stoppingToken);
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
