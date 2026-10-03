using HappyHeadlines.ArticleApi.Data;
using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.ArticleApi.Messaging;

public class ArticlesWorker(IMessageClient messageClient, Coordinator coordinator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await messageClient.SubscribeAsync<ArticleMessage>("ArticlesWorker", SaveNewArticles, ct);
    }

    private async Task SaveNewArticles(ArticleMessage articleMessage)
    {
        var article = new Article
        {
            Id = articleMessage.ArticleId,
            Title = articleMessage.Title,
            Content = articleMessage.Content,
            Author = articleMessage.Author,
            PublishDate = articleMessage.PublishDate,
            Region = articleMessage.Region
        };
        
        await using var db = coordinator.GetArticleDbContext(articleMessage.Region);
        if (await db.Articles.AnyAsync(a => a.Id == article.Id))
        {
            MonitorService.Log.Here().Information("Article with ID: {ArticleId} already exists", articleMessage.ArticleId);
            return;
        }
        
        db.Articles.Add(article);
        await db.SaveChangesAsync();
        MonitorService.Log.Here().Information("Added article with ID: {ArticleId}", articleMessage.ArticleId);
    }
}