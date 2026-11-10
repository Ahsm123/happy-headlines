using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.NewsletterApi.Clients;
using System.Runtime.CompilerServices;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace HappyHeadlines.NewsletterApi.Services;

public sealed class NewsletterService(
    IArticleApiClient articleApiClient,
    ISubscriberClient subscriberClient,
    IConfiguration configuration,
    ILogger<NewsletterService> logger
) : INewsletterService
{
    private bool SubscriberServiceEnabled => configuration.GetValue<bool>("Features:SubscriberService");

    public async Task SendImmediateNewsletterAsync(ArticlePublishedEvent publishedEvent, CancellationToken ct)
    {
        if (IsSubscriberServiceDisabled())
        {
            return;
        }

        logger.LogInformation("Received article {ArticleId} for {Region}", publishedEvent.ArticleId,
            publishedEvent.Region);
        var subs = await subscriberClient.GetSubscriberEmails();
        var sentCount = 0;
        foreach (var sub in subs)
        {
            if (sub.Region == publishedEvent.Region)
            {
                //Send newsletter to subscriber
                sentCount++;
            }
        }

        logger.LogInformation("Article: {ArticleTitle} was sent to {SubCount} subscribers in {Region}",
            publishedEvent.Title, sentCount, publishedEvent.Region);
    }

    public async Task SendDailyNewsletterAsync(CancellationToken ct)
    {
        if (IsSubscriberServiceDisabled())
        {
            return;
        }

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
        }

        logger.LogInformation(
            "Sent daily newsletter ({ArticleCount} articles across all regions) to {SubCount} subscribers",
            articles.Count, subs.Count);
    }

    public async Task SendWelcomeMailAsync(NewSubscriptionEvent message, CancellationToken ct)
    {
        if (IsSubscriberServiceDisabled())
        {
            return;
        }

        logger.LogInformation("Send Welcome mail to new subscriber with email {Email} for {Region}", message.Email,
            message.Region);
    }

    private bool IsSubscriberServiceDisabled([CallerMemberName] string operation = "")
    {
        if (!SubscriberServiceEnabled)
        {
            logger.LogInformation("SubscriberService is disabled, skipping {Operation}", operation);
            return true;
        }

        return false;
    }
}
