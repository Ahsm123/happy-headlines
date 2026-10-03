using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleApi.Workers;

public class ArticleCacheWorker(
    Coordinator coordinator,
    ArticleCache cache) : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        do
        {
            var articles = await GetLatestArticles();
            foreach (var article in articles)
            {
                await cache.SetCache(article);
                MonitorService.Log.Here().Information("Added article {ArticleId} to ArticleCache", article.Id);
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        } while (!stoppingToken.IsCancellationRequested);
    }

    private async Task<IEnumerable<Article>> GetLatestArticles()
    {
        var latestArticles = new List<Article>();
        foreach (var region in Enum.GetValues<Region>())
        {
            await using var db = coordinator.GetArticleDbContext(region);
            var regionArticles =
                await db.Articles.Where(a => a.PublishDate > DateTime.UtcNow.AddDays(-14)).ToListAsync();
            latestArticles.AddRange(regionArticles);
        }

        return latestArticles;
    }
}