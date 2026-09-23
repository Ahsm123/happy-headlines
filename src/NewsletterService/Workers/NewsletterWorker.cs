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

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromHours(24), ct);
            await SendDailyNewsletter();
        }
    }

    private async Task SendImmediateNewsletter(ArticleMessage articleMessage)
    {
        MonitorService.Log.Here().Information("NewsletterWorker receivedMessage");
        var subs = await subscriberClient.GetSubscriberEmails();
        foreach (var sub in subs)
        {
            if (sub.Region == articleMessage.Region)
            {
                MonitorService.Log.Here().Information("Sending article {articleTitle} to {subscriberEmail}", sub.Email,
                    articleMessage.Title);
            }
        }
    }

    private async Task SendDailyNewsletter()
    {
        var articles = new List<ArticleDto>();
        foreach (var region in Region.GetValues<Region>())
        {
            var regionalArticles = await articleApiClient.GetTodaysArticles(region);
            articles.AddRange(regionalArticles);
            MonitorService.Log.Here().Information("Fetched: {articleCount} articles from {region}",
                regionalArticles.Count, region);
        }

        var subs = await subscriberClient.GetSubscriberEmails();
        foreach (var sub in subs)
        {
            var articlesForSub = articles.Where(a => a.Region == sub.Region).ToList();
            MonitorService.Log.Here().Information("Sent {articleCount} articles to {subscriberEmail}", articlesForSub.Count, sub.Email);
        }
    }
}