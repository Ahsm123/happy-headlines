using HappyHeadlines.ArticleApi.Caching;
using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleApi.Services;

public sealed class ArticleService(
    ArticleCache cache,
    Coordinator coordinator,
    ILogger<ArticleService> logger) : IArticleService
{
    public async Task CreateAsync(ArticlePublishedEvent publishedEvent, CancellationToken ct)
    {
        await using var db = coordinator.GetArticleDbContext(publishedEvent.Region);
        if (await db.Articles.AnyAsync(a => a.Id == publishedEvent.ArticleId, ct))
        {
            logger.LogInformation("Article {ArticleId} already exists", publishedEvent.ArticleId);
            return;
        }

        db.Articles.Add(new Article
        {
            Id = publishedEvent.ArticleId,
            Title = publishedEvent.Title,
            Content = publishedEvent.Content,
            Author = publishedEvent.Author,
            PublishDate = publishedEvent.PublishDate,
            Region = publishedEvent.Region
        });

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Added article {ArticleId} in {Region}", publishedEvent.ArticleId, publishedEvent.Region);
    }

    public async Task<Article?> GetByIdAsync(Region region, Guid id, CancellationToken ct)
    {
        var article = await cache.GetArticle(region, id);
        return article;
    }

    public async Task<IEnumerable<Article>> GetAllAsync(Region region, DateTime? fromDate, CancellationToken ct)
    {
        await using var db = coordinator.GetArticleDbContext(region);

        var query = db.Articles.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.PublishDate >= fromDate.Value);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<bool> UpdateAsync(Region region, Guid id, UpdateArticleRequest request, CancellationToken ct)
    {
        await using var db = coordinator.GetArticleDbContext(region);

        var existing = await db.Articles.FindAsync([id], ct);
        if (existing is null)
        {
            return false;
        }

        existing.Title = request.Title;
        existing.Content = request.Content;
        existing.Author = request.Author;
        existing.PublishDate = request.PublishDate;

        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync(region, id);
        logger.LogInformation("Updated article {ArticleId} in {Region}", existing.Id, region);

        return true;
    }

    public async Task<bool> DeleteAsync(Region region, Guid id, CancellationToken ct)
    {
        await using var db = coordinator.GetArticleDbContext(region);

        var existing = await db.Articles.FindAsync([id], ct);
        if (existing is null)
        {
            return false;
        }

        db.Articles.Remove(existing);
        await db.SaveChangesAsync(ct);
        await cache.RemoveAsync(region, id);
        logger.LogInformation("Deleted article {ArticleId} in {Region}", id, region);

        return true;
    }

    public async Task<IEnumerable<Article>> GetLatestAsync(int count, CancellationToken ct)
    {
        var articles = new List<Article>();
        foreach (var region in Enum.GetValues<Region>())
        {
            await using var db = coordinator.GetArticleDbContext(region);
            var regionArticles = await db.Articles
                .OrderByDescending(a => a.PublishDate)
                .Take(count)
                .ToListAsync(ct);
            articles.AddRange(regionArticles);
        }

        return articles.OrderByDescending(a => a.PublishDate).Take(count).ToList();
    }
}
