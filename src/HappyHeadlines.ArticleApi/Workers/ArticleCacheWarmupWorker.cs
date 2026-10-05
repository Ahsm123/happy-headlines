using HappyHeadlines.ArticleApi.Caching;

namespace HappyHeadlines.ArticleApi.Workers;

public sealed class ArticleCacheWarmupWorker(
    ArticleCache cache,
    ILogger<ArticleCacheWarmupWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        do
        {
            try
            {
                await cache.WarmUpAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Article cache warmup failed, retrying in an hour");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
