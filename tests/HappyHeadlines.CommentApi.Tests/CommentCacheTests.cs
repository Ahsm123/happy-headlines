using HappyHeadlines.CommentApi.Caching;
using HappyHeadlines.CommentApi.Clients;
using HappyHeadlines.CommentApi.Data;
using HappyHeadlines.CommentApi.Models;
using HappyHeadlines.Contracts.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;

namespace HappyHeadlines.CommentApi.Tests;

public class CommentCacheTests
{
    private readonly Guid _articleId = Guid.NewGuid();
    private readonly IDistributedCache _redis = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
    private readonly CommentDbContext _db = new(new DbContextOptionsBuilder<CommentDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private readonly Mock<IDatabase> _counters = new();

    private CommentCache CreateCache(params Guid[] newestIds) =>
        new(_redis, new FakeArticleClient(newestIds), _db,
            Mock.Of<IConnectionMultiplexer>(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()) == _counters.Object));

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

    [Fact]
    public async Task MissThenHit_IncrementsCounters()
    {
        await SeedComment();
        var cache = CreateCache(_articleId);

        await cache.Comments(_articleId);
        await cache.Comments(_articleId);

        _counters.Verify(d => d.StringIncrementAsync("CommentApi:cache:misses", 1, CommandFlags.None), Times.Once);
        _counters.Verify(d => d.StringIncrementAsync("CommentApi:cache:hits", 1, CommandFlags.None), Times.Once);
    }

    private class FakeArticleClient(Guid[] ids) : IArticleClient
    {
        public Task<IEnumerable<ArticleDto>> GetNewestArticlesAsync(int count) =>
            Task.FromResult(ids.Select(id => new ArticleDto { Id = id, Title = "", Content = "", Author = "" }));
    }
}
