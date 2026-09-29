using CommentService.Clients;
using CommentService.Data;
using CommentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ServiceDefaults.Contracts;

namespace CommentService.Tests;

public class CommentCacheTests
{
    private readonly Guid _articleId = Guid.NewGuid();
    private readonly IDistributedCache _redis = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
    private readonly CommentDbContext _db = new(new DbContextOptionsBuilder<CommentDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private CommentCache CreateCache(params Guid[] newestIds) => new(_redis, new FakeArticleClient(newestIds), _db);

    private async Task SeedComment()
    {
        _db.Comments.Add(new Comment { Id = Guid.NewGuid(), ArticleId = _articleId, CommentText = "hi" });
        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task Miss_ArticleInNewest_IsCached()
    {
        await SeedComment();

        var comments = await CreateCache(_articleId).Comments(_articleId);

        Assert.Single(comments);
        Assert.NotNull(await _redis.GetStringAsync(_articleId.ToString()));
    }

    [Fact]
    public async Task Miss_ArticleNotInNewest_IsNotCached()
    {
        await SeedComment();

        var comments = await CreateCache(Guid.NewGuid()).Comments(_articleId);

        Assert.Single(comments);
        Assert.Null(await _redis.GetStringAsync(_articleId.ToString()));
    }

    [Fact]
    public async Task Invalidate_RemovesEntry()
    {
        await SeedComment();
        var cache = CreateCache(_articleId);
        await cache.Comments(_articleId);

        await cache.InvalidateCacheEntry(_articleId);

        Assert.Null(await _redis.GetStringAsync(_articleId.ToString()));
    }

    private class FakeArticleClient(Guid[] ids) : IArticleClient
    {
        public Task<IEnumerable<ArticleDto>> GetNewestArticlesAsync(int count) =>
            Task.FromResult(ids.Select(id => new ArticleDto { Id = id, Title = "", Content = "", Author = "" }));
    }
}
