using System.Text.Json;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace HappyHeadlines.ArticleApi.Data;

public class ArticleCache(
    IDistributedCache cache,
    Coordinator coordinator,
    IConnectionMultiplexer redis)
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

    public async Task<Article?> GetArticle(Guid articleId, Region region)
    {
        var key = articleId.ToString() + region.ToString();
        var cacheHit = await cache.GetAsync(key);
        if (cacheHit is null)
        {
            await OnCacheMiss();
            await using var db = coordinator.GetArticleDbContext(region);
            var article = await db.Articles.FindAsync(articleId);
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

    public async Task SetCache(Article article)
    {
        var publishDate = article.PublishDate;
        var cacheOffset = new DateTimeOffset(publishDate.AddDays(14), TimeSpan.Zero);

        var key = article.Id.ToString() + article.Region.ToString();
        await cache.SetAsync(
            key,
            JsonSerializer.SerializeToUtf8Bytes(article),
            new DistributedCacheEntryOptions
                { AbsoluteExpiration = cacheOffset });
    }
}