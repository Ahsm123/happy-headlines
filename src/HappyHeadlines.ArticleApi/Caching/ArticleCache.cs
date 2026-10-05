using System.Text.Json;
using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
        var cached = await cache.GetAsync(id.ToString());
        if (cached is not null)
        {
            await OnCacheHit();
            return JsonSerializer.Deserialize<Article>(cached);
        }

        await OnCacheMiss();
        await using var db = coordinator.GetArticleDbContext(Region.Global);
        var article = await db.Articles.FindAsync(id);
        if (article is not null && DateTime.UtcNow < article.PublishDate + CacheWindow)
        {
            await SetCache(article);
        }

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
