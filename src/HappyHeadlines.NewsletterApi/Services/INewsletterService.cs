using HappyHeadlines.Contracts.Events;

namespace HappyHeadlines.NewsletterApi.Services;

public interface INewsletterService
{
    Task SendImmediateNewsletterAsync(ArticlePublishedEvent publishedEvent, CancellationToken ct);
    Task SendDailyNewsletterAsync(CancellationToken ct);
}
