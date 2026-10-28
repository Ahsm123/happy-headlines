using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.SubscriberApi.Models;

public class Subscription
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public Region SubscribedRegion { get; set; }
}
