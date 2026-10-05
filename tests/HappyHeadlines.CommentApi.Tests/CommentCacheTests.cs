using HappyHeadlines.CommentApi.Caching;
using HappyHeadlines.CommentApi.Data;
using HappyHeadlines.CommentApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;

namespace HappyHeadlines.CommentApi.Tests;

public class CommentCacheTests
{
    private readonly Guid _articleId = Guid.NewGuid();

    private readonly IDistributedCache _redis =
        new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

    private readonly CommentDbContext _db = new(new DbContextOptionsBuilder<CommentDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private readonly Mock<IDatabase> _counters = new();

    private CommentCache CreateCache() =>
        new(_redis, _db,
            Mock.Of<IConnectionMultiplexer>(m =>
                m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()) == _counters.Object),
            NullLogger<CommentCache>.Instance);

    private async Task SeedComment()
    {
        _db.Comments.Add(new Comment { Id = Guid.NewGuid(), ArticleId = _articleId, CommentText = "hi" });
        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task Miss_IsCached()
    {
        await SeedComment();

        var comments = await CreateCache().Comments(_articleId);

        Assert.Single(comments);
        Assert.NotNull(await _redis.GetStringAsync(_articleId.ToString()));
    }

    [Fact]
    public async Task Read_TouchesLruTimestamp()
    {
        await CreateCache().Comments(_articleId);

        _counters.Verify(d => d.SortedSetAddAsync("CommentApi:lru", _articleId.ToString(), It.IsAny<double>(),
            It.IsAny<SortedSetWhen>(), It.IsAny<CommandFlags>()));
    }

    [Fact]
    public async Task Miss_OverCapacity_EvictsLeastRecentlyUsed()
    {
        // Simulerer at LRU-sættet har 31 artikler, og at oldId er den længst tid siden brugte.
        var oldId = Guid.NewGuid().ToString();
        await _redis.SetStringAsync(oldId, "[]");
        _counters.Setup(d => d.SortedSetLengthAsync("CommentApi:lru", It.IsAny<double>(), It.IsAny<double>(),
                It.IsAny<Exclude>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(31);
        _counters.Setup(d => d.SortedSetRangeByRankAsync("CommentApi:lru", 0, 0, It.IsAny<Order>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync([oldId]);
        await SeedComment();

        await CreateCache().Comments(_articleId);

        Assert.Null(await _redis.GetStringAsync(oldId));
        Assert.NotNull(await _redis.GetStringAsync(_articleId.ToString()));
        _counters.Verify(d => d.SortedSetRemoveRangeByRankAsync("CommentApi:lru", 0, 0, It.IsAny<CommandFlags>()));
    }

    [Fact]
    public async Task Invalidate_RemovesEntry()
    {
        await SeedComment();
        var cache = CreateCache();
        await cache.Comments(_articleId);

        await cache.InvalidateCacheEntry(_articleId);

        Assert.Null(await _redis.GetStringAsync(_articleId.ToString()));
    }

    [Fact]
    public async Task MissThenHit_IncrementsCounters()
    {
        await SeedComment();
        var cache = CreateCache();

        await cache.Comments(_articleId);
        await cache.Comments(_articleId);

        _counters.Verify(d => d.StringIncrementAsync("CommentApi:cache:misses", 1, CommandFlags.None), Times.Once);
        _counters.Verify(d => d.StringIncrementAsync("CommentApi:cache:hits", 1, CommandFlags.None), Times.Once);
    }
}
