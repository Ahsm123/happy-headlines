using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.SubscriberApi.Models;

public class Subscriber
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public Region SubscribedRegion { get; set; }
}
