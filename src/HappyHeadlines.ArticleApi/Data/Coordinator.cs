using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleApi.Data;

public class Coordinator(IConfiguration configuration)
{
    public ArticleDbContext GetArticleDbContext(Region region) =>
        new(new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql(configuration.GetConnectionString($"Database{region}") ??
                       throw new InvalidOperationException($"Missing ConnectionStrings:Database{region}"))
            .Options);
}
