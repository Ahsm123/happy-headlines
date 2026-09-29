using System.Text.Json;
using CommentService.Clients;
using CommentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi;
using ServiceDefaults;
using ServiceDefaults.Contracts;

namespace CommentService.Data;

public class CommentCache(
    IDistributedCache cache,
    IArticleClient articleClient,
    CommentDbContext dbContext)
{
    private readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(10);

    public async Task InvalidateCacheEntry(Guid articleId)
    {
        await cache.RemoveAsync(articleId.ToString());
    }

    public async Task<List<Comment>> Comments(Guid articleId)
    {
        var cacheHit = await cache.GetStringAsync(articleId.ToString());
        if (cacheHit is null)
        {
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
                MonitorService.Log.Here().Warning("ArticleService unavailable, skipping cache for article {ArticleId}", articleId);
            }

            return fetchedComments;
        }

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