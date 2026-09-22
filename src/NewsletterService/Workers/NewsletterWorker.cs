using NewsletterService.Clients;
using ServiceDefaults;
using ServiceDefaults.Contracts;

namespace NewsletterService.Workers;

public class NewsletterWorker(
    IMessageClient messageClient,
    IArticleApiClient articleApiClient,
    ISubscriberClient subscriberClient
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await messageClient.SubscribeAsync<ArticleMessage>("NewsletterWorker", SendImmediateNewsletter, ct);
    }

    private async Task SendImmediateNewsletter(ArticleMessage articleMessage)
    {
        MonitorService.Log.Here().Information("NewsletterWorker receivedMessage");
        var subs = await subscriberClient.GetSubscriberEmails();
    }
}   