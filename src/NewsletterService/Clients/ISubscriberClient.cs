namespace NewsletterService.Clients;

public interface ISubscriberClient
{
    // Fetch subscriber list from SubscriberService to we can send newsletter to them
    void Subscribe();
    void Unsubscribe();
    
}