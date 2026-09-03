using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ArticleService.Data;

public class ArticleDbContextFactory : IDesignTimeDbContextFactory<ArticleDbContext>
{
    public ArticleDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=articledb;Username=postgres;Password=dev")
            .Options);
}
