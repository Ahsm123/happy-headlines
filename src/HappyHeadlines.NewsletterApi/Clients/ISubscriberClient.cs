using HappyHeadlines.Contracts.Subscribers;

namespace HappyHeadlines.NewsletterApi.Clients;

public interface ISubscriberClient
{
    Task<List<SubscriberDto>> GetSubscriberEmails();
    
}