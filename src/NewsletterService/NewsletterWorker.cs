using ServiceDefaults;
using ServiceDefaults.Contracts;

namespace NewsletterService;

public class NewsletterWorker(IMessageClient messageClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await messageClient.SubscribeAsync<ArticleMessage>("NewsletterWorker", SendImmediateNewsletter, ct);
    }

    private Task SendImmediateNewsletter(ArticleMessage articleMessage)
    {
        MonitorService.Log.Here().Information("Received message and executed handler");
        return Task.CompletedTask;
    }
}   