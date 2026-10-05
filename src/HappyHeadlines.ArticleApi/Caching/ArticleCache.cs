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
    private const string HitsKey = "ArticleApi:cache:hits";
    private const string MissesKey = "ArticleApi:cache:misses";

    private Task OnCacheHit() => redis.GetDatabase().StringIncrementAsync(HitsKey);
    private Task OnCacheMiss() => redis.GetDatabase().StringIncrementAsync(MissesKey);

    public async Task<(long Hits, long Misses)> Stats()
    {
        var db = redis.GetDatabase();
        return ((long)await db.StringGetAsync(HitsKey), (long)await db.StringGetAsync(MissesKey));
    }

    private static string Key(Region region, Guid id) => id.ToString() + region.ToString();
    public async Task RemoveAsync(Region region, Guid id) => await cache.RemoveAsync(Key(region, id));

    public async Task<Article?> GetArticle(Region region, Guid id)
    {
        if (region != Region.Global)
        {
            await using var regionalDb = coordinator.GetArticleDbContext(region);
            return await regionalDb.Articles.FindAsync(id);
        }

        var key = Key(region, id);
        var cacheHit = await cache.GetAsync(key);
        if (cacheHit is null)
        {
            await OnCacheMiss();
            await using var db = coordinator.GetArticleDbContext(region);
            var article = await db.Articles.FindAsync(id);
            if (article == null)
            {
                return null;
            }

            if (DateTime.UtcNow < article.PublishDate.AddDays(14))
            {
                await SetCache(article);
            }

            return article;
        }

        await OnCacheHit();
        return JsonSerializer.Deserialize<Article>(cacheHit);
    }

    private async Task SetCache(Article article)
    {
        var publishDate = article.PublishDate;
        var cacheOffset = new DateTimeOffset(publishDate.AddDays(14), TimeSpan.Zero);

        var key = Key(article.Region, article.Id);
        await cache.SetAsync(
            key,
            JsonSerializer.SerializeToUtf8Bytes(article),
            new DistributedCacheEntryOptions
                { AbsoluteExpiration = cacheOffset });
    }

    public async Task WarmUpAsync(CancellationToken ct)
    {
        var since = DateTime.UtcNow.AddDays(-14);
        await using var db = coordinator.GetArticleDbContext(Region.Global);
        var articles = await db.Articles.Where(a => a.PublishDate > since).ToListAsync(ct);
        foreach (var article in articles)
        {
            await SetCache(article);
        }

        logger.LogInformation("Warmed article cache with {ArticleCount} global articles", articles.Count);
    }
}
