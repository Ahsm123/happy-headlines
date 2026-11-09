using HappyHeadlines.ArticleApi.Caching;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.ArticleApi.Services;
using HappyHeadlines.Contracts.Articles;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;

namespace HappyHeadlines.ArticleApi.Tests;

public class ArticleServiceTests
{
    private readonly FakeCoordinator _coordinator = new();
    private readonly ArticleService _service;

    public ArticleServiceTests()
    {
        var cache = new ArticleCache(
            new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())),
            _coordinator,
            Mock.Of<IConnectionMultiplexer>(),
            NullLogger<ArticleCache>.Instance);

        _service = new ArticleService(cache, _coordinator, NullLogger<ArticleService>.Instance);
    }

    [Fact]
    public async Task Latest_SecondCall_IsServedFromCache()
    {
        // Arrange
        await AddArticle(Region.Europe, DateTime.UtcNow);
        await _service.GetLatestAsync(2, CancellationToken.None);
        var callsAfterFirst = _coordinator.CallCount;

        // Act
        await _service.GetLatestAsync(2, CancellationToken.None);

        // Assert
        Assert.Equal(callsAfterFirst, _coordinator.CallCount);
    }

    [Fact]
    public async Task Latest_MergesRegions_NewestFirst()
    {
        // Arrange
        var now = DateTime.UtcNow;
        await AddArticle(Region.Europe, now.AddHours(-1));
        await AddArticle(Region.Europe, now.AddHours(-3));
        await AddArticle(Region.Asia, now.AddHours(-2));

        // Act
        var latest = (await _service.GetLatestAsync(2, CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, latest.Count);
        Assert.Equal(now.AddHours(-1), latest[0].PublishDate);
        Assert.Equal(now.AddHours(-2), latest[1].PublishDate);
    }

    [Fact]
    public async Task Latest_CountAboveMax_IsClamped()
    {
        // Arrange
        for (var i = 0; i < 60; i++)
        {
            await AddArticle(Region.Europe, DateTime.UtcNow.AddMinutes(-i));
        }

        // Act
        var latest = await _service.GetLatestAsync(1000, CancellationToken.None);

        // Assert
        Assert.Equal(50, latest.Count());
    }

    [Fact]
    public async Task Latest_LongContent_ReturnsTeaserOf200Chars()
    {
        // Arrange
        await AddArticle(Region.Europe, DateTime.UtcNow, new string('x', 500));

        // Act
        var latest = (await _service.GetLatestAsync(1, CancellationToken.None)).Single();

        // Assert
        Assert.Equal(200, latest.Teaser.Length);
    }

    private async Task AddArticle(Region region, DateTime publishDate, string content = "Content")
    {
        await using var db = _coordinator.GetArticleDbContext(region);
        db.Articles.Add(new Article
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Content = content,
            Author = "Author",
            PublishDate = publishDate,
            Region = region
        });
        await db.SaveChangesAsync();
    }
}
