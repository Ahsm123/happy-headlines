using ServiceDefaults;
using ServiceDefaults.Contracts;

namespace NewsletterService.Workers;

public class NewsletterWorker(IMessageClient messageClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await messageClient.SubscribeAsync<ArticleMessage>("NewsletterWorker", SendImmediateNewsletter, ct);
    }

    private Task SendImmediateNewsletter(ArticleMessage articleMessage)
    {
        MonitorService.Log.Here().Information("NewsletterWorker receivedMessage");
        return Task.CompletedTask;
    }
}   