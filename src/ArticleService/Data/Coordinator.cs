using ArticleService.Models;
using Microsoft.EntityFrameworkCore;
using ServiceDefaults.Contracts;

namespace ArticleService.Data;

public class Coordinator(IConfiguration configuration)
{
    public ArticleDbContext GetArticleDbContext(Region region) =>
        new(new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql(configuration.GetConnectionString($"ArticleDb{region}") ??
                       throw new InvalidOperationException($"Missing ConnectionStrings:ArticleDb{region}"))
            .Options);
}
