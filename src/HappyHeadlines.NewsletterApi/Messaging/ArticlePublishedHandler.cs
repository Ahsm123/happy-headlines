using HappyHeadlines.Contracts.Events;
using HappyHeadlines.NewsletterApi.Services;
using HappyHeadlines.ServiceDefaults;

namespace HappyHeadlines.NewsletterApi.Messaging;

public sealed class ArticlePublishedHandler(INewsletterService newsletterService) 
    : IMessageHandler<ArticlePublishedEvent>
{
    public async Task HandleAsync(ArticlePublishedEvent message, CancellationToken ct)
    {
        await newsletterService.SendImmediateNewsletterAsync(message, ct);
    }
}
