using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Subscribers;

namespace HappyHeadlines.NewsletterApi.Clients;

public class SubscriberClient : ISubscriberClient
{
    private readonly List<SubscriptionDto> _subscriberEmails = [];

    public SubscriberClient()
    {
        _subscriberEmails.Add(new SubscriptionDto("test@gmail.com", Region.Africa));
        _subscriberEmails.Add(new SubscriptionDto("test2@gmail.com", Region.Asia));
        _subscriberEmails.Add(new SubscriptionDto("test3@gmail.com", Region.Europe));
        _subscriberEmails.Add(new SubscriptionDto("test4@gmail.com", Region.Global));
    }
    public Task<List<SubscriptionDto>> GetSubscriberEmails()
    {
        return Task.FromResult(_subscriberEmails);
    }
}