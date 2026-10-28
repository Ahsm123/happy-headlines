using HappyHeadlines.Contracts.Subscribers;
using HappyHeadlines.SubscriberApi.Models;

namespace HappyHeadlines.SubscriberApi.Extensions;

public static class SubscriberMappingExtensions
{
    public static SubscriberDto ToDto(this Subscriber subscriber) =>
        new(
            subscriber.Email,
            subscriber.SubscribedRegion
        );
}
