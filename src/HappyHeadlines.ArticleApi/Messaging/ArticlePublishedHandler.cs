using HappyHeadlines.ArticleApi.Services;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.ServiceDefaults;

namespace HappyHeadlines.ArticleApi.Messaging;

public sealed class ArticlePublishedHandler(IArticleService service)
    : IMessageHandler<ArticlePublishedEvent>
{
    public async Task HandleAsync(ArticlePublishedEvent message, CancellationToken ct)
    {
        await service.CreateAsync(message, ct);
    }
}
