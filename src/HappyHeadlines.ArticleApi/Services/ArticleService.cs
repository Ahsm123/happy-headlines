using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Events;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleApi.Services;

public sealed class ArticleService(
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
}
