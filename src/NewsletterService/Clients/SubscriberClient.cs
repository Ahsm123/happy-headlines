namespace NewsletterService.Clients;

public class SubscriberClient : ISubscriberClient
{
    private readonly List<string> _subscriberEmails = [];

    public SubscriberClient()
    {
        _subscriberEmails.Add("test1@gmail.com");
        _subscriberEmails.Add("test2@gmail.com");
        _subscriberEmails.Add("test3@gmail.com");
    }
    public Task<List<string>> GetSubscriberEmails()
    {
        return Task.FromResult(_subscriberEmails);
    }
}