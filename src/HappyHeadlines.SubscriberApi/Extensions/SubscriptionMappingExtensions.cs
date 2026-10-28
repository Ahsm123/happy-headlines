using HappyHeadlines.Contracts.Subscribers;
using HappyHeadlines.SubscriberApi.Models;

namespace HappyHeadlines.SubscriberApi.Extensions;

public static class SubscriptionMappingExtensions
{
    public static SubscriptionDto ToDto(this Subscription subscription) =>
        new(
            subscription.Email,
            subscription.SubscribedRegion
        );
}
