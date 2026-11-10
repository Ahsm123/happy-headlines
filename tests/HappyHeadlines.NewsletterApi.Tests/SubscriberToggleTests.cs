using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.NewsletterApi.Clients;
using HappyHeadlines.NewsletterApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace HappyHeadlines.NewsletterApi.Tests;

public class SubscriberToggleTests
{
    private readonly Mock<ISubscriberClient> _subscribers = new();

    private readonly ArticlePublishedEvent _published =
        new(Guid.NewGuid(), "Title", "Content", "Author", DateTime.UtcNow, Region.Europe);

    private NewsletterService CreateService(string enabled) =>
        new(Mock.Of<IArticleApiClient>(),
            _subscribers.Object,
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["Features:SubscriberService"] = enabled })
                .Build(),
            NullLogger<NewsletterService>.Instance);

    [Fact]
    public async Task Disabled_DoesNotCallSubscriberService()
    {
        var service = CreateService("false");

        await service.SendImmediateNewsletterAsync(_published, CancellationToken.None);
        await service.SendDailyNewsletterAsync(CancellationToken.None);

        _subscribers.Verify(s => s.GetSubscriberEmails(), Times.Never);
    }

    [Fact]
    public async Task Enabled_CallsSubscriberService()
    {
        _subscribers.Setup(s => s.GetSubscriberEmails()).ReturnsAsync([]);
        var service = CreateService("true");

        await service.SendImmediateNewsletterAsync(_published, CancellationToken.None);

        _subscribers.Verify(s => s.GetSubscriberEmails(), Times.Once);
    }
}
