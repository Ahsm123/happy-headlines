using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.Contracts.Articles;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleApi.Tests;

public class FakeCoordinator : ICoordinator
{
    private readonly string _testId = Guid.NewGuid().ToString();

    public ArticleDbContext GetArticleDbContext(Region region)
    {
        var options = new DbContextOptionsBuilder<ArticleDbContext>()
            .UseInMemoryDatabase($"{_testId}-{region}")
            .Options;

        return new ArticleDbContext(options);
    }
}
