using HappyHeadlines.Contracts.Events;

namespace HappyHeadlines.ArticleApi.Services;

public interface IArticleService
{
    Task CreateAsync(ArticlePublishedEvent publishedEvent, CancellationToken ct);
}
