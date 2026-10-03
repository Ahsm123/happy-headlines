using System.Text.Json;
using HappyHeadlines.CommentApi.Clients;
using HappyHeadlines.CommentApi.Data;
using HappyHeadlines.CommentApi.Models;
using HappyHeadlines.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi;
using StackExchange.Redis;

namespace HappyHeadlines.CommentApi.Caching;

public class CommentCache(
    IDistributedCache cache,
    IArticleClient articleClient,
    CommentDbContext dbContext,
    IConnectionMultiplexer redis)
{
    private const string HitsKey = "CommentApi:cache:hits";
    private const string MissesKey = "CommentApi:cache:misses";

    private Task OnCacheHit() => redis.GetDatabase().StringIncrementAsync(HitsKey);
    private Task OnCacheMiss() => redis.GetDatabase().StringIncrementAsync(MissesKey);

    private readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(10);

    public async Task<(long Hits, long Misses)> Stats()
    {
        var db = redis.GetDatabase();
        return ((long)await db.StringGetAsync(HitsKey), (long)await db.StringGetAsync(MissesKey));
    }

    public async Task InvalidateCacheEntry(Guid articleId)
    {
        await cache.RemoveAsync(articleId.ToString());
    }

    public async Task<List<Comment>> Comments(Guid articleId)
    {
        var cacheHit = await cache.GetStringAsync(articleId.ToString());
        if (cacheHit is null)
        {
            await OnCacheMiss();
            var fetchedComments = await dbContext.Comments
                .Where(c => c.ArticleId == articleId)
                .ToListAsync();

            try
            {
                var newestArticles = await articleClient.GetNewestArticlesAsync(30);
                var isNewArticle = newestArticles.Any(a => a.Id == articleId);

                if (isNewArticle)
                {
                    await SetCache(articleId, fetchedComments);
                }
            }
            catch (HttpRequestException)
            {
                MonitorService.Log.Here().Warning("ArticleApi unavailable, skipping cache for article {ArticleId}",
                    articleId);
            }

            return fetchedComments;
        }

        await OnCacheHit();
        return JsonSerializer.Deserialize<List<Comment>>(cacheHit);
    }

    private async Task SetCache(Guid articleId, List<Comment> fetchedComments)
    {
        await cache.SetAsync(
            articleId.ToString(),
            JsonSerializer.SerializeToUtf8Bytes(fetchedComments),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _expirationTime });
    }
}