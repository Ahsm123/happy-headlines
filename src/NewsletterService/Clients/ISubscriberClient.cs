using ServiceDefaults.Contracts;

namespace NewsletterService.Clients;

public interface ISubscriberClient
{
    Task<List<SubscriberDto>> GetSubscriberEmails();
    
}