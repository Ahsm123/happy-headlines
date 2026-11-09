using Microsoft.EntityFrameworkCore;
using HappyHeadlines.ArticleApi.Models;

namespace HappyHeadlines.ArticleApi.Data;

public class ArticleDbContext : DbContext
{
    public ArticleDbContext(DbContextOptions<ArticleDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>()
            .HasIndex(a => a.PublishDate);
    }
}
