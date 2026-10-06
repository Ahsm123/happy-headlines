using System.Diagnostics;
using System.Text.Json;
using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Prometheus;
using StackExchange.Redis;

namespace HappyHeadlines.ArticleApi.Caching;

public class ArticleCache(
    IDistributedCache cache,
    Coordinator coordinator,
    IConnectionMultiplexer redis,
    ILogger<ArticleCache> logger)
{
    private static readonly TimeSpan CacheWindow = TimeSpan.FromDays(14);
    
    private const string HitsKey = "ArticleApi:cache:hits";
    private const string MissesKey = "ArticleApi:cache:misses";

    // Tid for at hente en global artikel, opdelt på hit og miss, så dashboardet kan vise, hvad cachen sparer.
    private static readonly Histogram Duration = Metrics.CreateHistogram(
        "article_cache_duration_seconds", "Time to get a global article",
        new HistogramConfiguration { LabelNames = ["result"] });

    private Task OnCacheHit() => redis.GetDatabase().StringIncrementAsync(HitsKey);
    private Task OnCacheMiss() => redis.GetDatabase().StringIncrementAsync(MissesKey);

    public async Task<(long Hits, long Misses)> Stats()
    {
        var db = redis.GetDatabase();
        return ((long)await db.StringGetAsync(HitsKey), (long)await db.StringGetAsync(MissesKey));
    }

    public Task RemoveAsync(Guid id) => cache.RemoveAsync(id.ToString());

    public async Task<Article?> GetArticle(Guid id)
    {
        var start = Stopwatch.GetTimestamp();
        var cached = await cache.GetAsync(id.ToString());
        if (cached is not null)
        {
            await OnCacheHit();
            Duration.WithLabels("hit").Observe(Stopwatch.GetElapsedTime(start).TotalSeconds);
            return JsonSerializer.Deserialize<Article>(cached);
        }

        await OnCacheMiss();
        await using var db = coordinator.GetArticleDbContext(Region.Global);
        var article = await db.Articles.FindAsync(id);
        if (article is not null && DateTime.UtcNow < article.PublishDate + CacheWindow)
        {
            await SetCache(article);
        }

        Duration.WithLabels("miss").Observe(Stopwatch.GetElapsedTime(start).TotalSeconds);
        return article;
    }

    private Task SetCache(Article article) =>
        cache.SetAsync(
            article.Id.ToString(),
            JsonSerializer.SerializeToUtf8Bytes(article),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(article.PublishDate + CacheWindow, TimeSpan.Zero)
            });

    public async Task WarmUpAsync(CancellationToken ct)
    {
        var since = DateTime.UtcNow - CacheWindow;
        await using var db = coordinator.GetArticleDbContext(Region.Global);
        var articles = await db.Articles.Where(a => a.PublishDate > since).ToListAsync(ct);
        foreach (var article in articles)
        {
            await SetCache(article);
        }

        logger.LogInformation("Warmed article cache with {ArticleCount} global articles", articles.Count);
    }
}
