using HappyHeadlines.ArticleApi.Caching;
using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleApi.Services;

public sealed class ArticleService(
    ArticleCache cache,
    ICoordinator coordinator,
    ILogger<ArticleService> logger) : IArticleService

{
    private const int MaxLatestCount = 50;
    private const int TeaserLength = 200;

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
        if (region == Region.Global)
        {
            return await cache.GetArticle(id);
        }

        await using var db = coordinator.GetArticleDbContext(region);
        return await db.Articles.FindAsync([id], ct);
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
        await cache.RemoveAsync(id);
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
        await cache.RemoveAsync(id);
        logger.LogInformation("Deleted article {ArticleId} in {Region}", id, region);

        return true;
    }

    public async Task<IEnumerable<ArticleSummaryDto>> GetLatestAsync(int count, CancellationToken ct)
    {
        count = Math.Clamp(count, 0, MaxLatestCount);

        var latest = await cache.GetLatestAsync();
        if (latest is null)
        {
            latest = await FetchLatestFromRegions(ct);
            await cache.SetLatestAsync(latest);
        }

        return latest.Take(count);
    }

    private async Task<List<ArticleSummaryDto>> FetchLatestFromRegions(CancellationToken ct)
    {
        var tasks = Enum.GetValues<Region>().Select(async region =>
        {
            await using var db = coordinator.GetArticleDbContext(region);
            return await db.Articles
                .OrderByDescending(a => a.PublishDate)
                .Take(MaxLatestCount)
                .Select(a => new ArticleSummaryDto(
                    a.Id,
                    a.Title,
                    a.Content.Substring(0, a.Content.Length > TeaserLength ? TeaserLength : a.Content.Length),
                    a.Author,
                    a.PublishDate,
                    a.Region))
                .ToListAsync(ct);
        });

        var results = await Task.WhenAll(tasks);

        return results
            .SelectMany(r => r)
            .OrderByDescending(a => a.PublishDate)
            .Take(MaxLatestCount)
            .ToList();
    }
}

