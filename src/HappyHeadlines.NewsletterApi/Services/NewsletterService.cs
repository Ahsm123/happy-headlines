using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.NewsletterApi.Clients;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace HappyHeadlines.NewsletterApi.Services;

public sealed class NewsletterService(
    IArticleApiClient articleApiClient,
    ISubscriberClient subscriberClient,
    ILogger<NewsletterService> logger
) : INewsletterService
{
    public async Task SendImmediateNewsletterAsync(ArticlePublishedEvent publishedEvent, CancellationToken ct)
    {
        logger.LogInformation("Received article {ArticleId} for {Region}", publishedEvent.ArticleId,
            publishedEvent.Region);
        var subs = await subscriberClient.GetSubscriberEmails();
        foreach (var sub in subs)
        {
            if (sub.Region == publishedEvent.Region)
            {
                logger.LogInformation("Sending article {ArticleTitle} to {SubscriberEmail}",
                    publishedEvent.Title, sub.Email);
            }
        }
    }

    public async Task SendDailyNewsletterAsync(CancellationToken ct)
    {
        var articles = new List<ArticleDto>();
        foreach (var region in Enum.GetValues<Region>())
        {
            try
            {
                var regionalArticles = await articleApiClient.GetTodaysArticles(region);
                articles.AddRange(regionalArticles);
                logger.LogInformation("Fetched: {ArticleCount} articles from {Region}",
                    regionalArticles.Count, region);
            }
            catch (Exception ex) when (ex is BrokenCircuitException or TimeoutRejectedException or HttpRequestException)
            {
                logger.LogWarning(ex, "Failed to fetch articles from {Region}", region);
            }
        }

        var subs = await subscriberClient.GetSubscriberEmails();
        foreach (var sub in subs)
        {
            var articlesForSub = articles.Where(a => a.Region == sub.Region).ToList();
            logger.LogInformation("Sent {ArticleCount} articles to {SubscriberEmail}",
                articlesForSub.Count, sub.Email);
        }
    }
}
