using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.Contracts.Articles;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleApi.Tests;

public class FakeCoordinator : ICoordinator
{
    private readonly string _testId = Guid.NewGuid().ToString();
    private int _callCount = 0;
    public int CallCount => _callCount;

    public ArticleDbContext GetArticleDbContext(Region region)
    {
        Interlocked.Increment(ref _callCount);

        var options = new DbContextOptionsBuilder<ArticleDbContext>()
            .UseInMemoryDatabase($"{_testId}-{region}")
            .Options;

        return new ArticleDbContext(options);
    }
}
