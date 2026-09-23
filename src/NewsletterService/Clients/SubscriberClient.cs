using ServiceDefaults.Contracts;

namespace NewsletterService.Clients;

public class SubscriberClient : ISubscriberClient
{
    private readonly List<SubscriberDto> _subscriberEmails = [];

    public SubscriberClient()
    {
        _subscriberEmails.Add(new SubscriberDto("test@gmail.com", Region.Africa));
        _subscriberEmails.Add(new SubscriberDto("test2@gmail.com", Region.Asia));
        _subscriberEmails.Add(new SubscriberDto("test3@gmail.com", Region.Europe));
        _subscriberEmails.Add(new SubscriberDto("test4@gmail.com", Region.Global));
    }
    public Task<List<SubscriberDto>> GetSubscriberEmails()
    {
        return Task.FromResult(_subscriberEmails);
    }
}