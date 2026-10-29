using HappyHeadlines.Contracts.Events;
using HappyHeadlines.NewsletterApi.Services;
using HappyHeadlines.ServiceDefaults;

namespace HappyHeadlines.NewsletterApi.Messaging;

public sealed class NewSubscriberHandler(INewsletterService newsletterService)
    : IMessageHandler<NewSubscriptionEvent>
{
    public async Task HandleAsync(NewSubscriptionEvent message, CancellationToken ct)
    {
        await newsletterService.SendWelcomeMailAsync(message, ct);
    }
}
