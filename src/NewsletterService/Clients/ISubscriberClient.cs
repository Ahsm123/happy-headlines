namespace NewsletterService.Clients;

public interface ISubscriberClient
{
    Task<List<string>> GetSubscriberEmails();
    
}