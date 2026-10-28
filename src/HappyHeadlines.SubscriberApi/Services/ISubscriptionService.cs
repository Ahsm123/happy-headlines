using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Subscribers;
using HappyHeadlines.SubscriberApi.Models;

namespace HappyHeadlines.SubscriberApi.Services;

public interface ISubscriptionService
{
    Task<bool> SubscribeAsync(SubscribeRequest request, CancellationToken ct);
    Task<bool> UnsubscribeAsync(UnsubscribeRequest request, CancellationToken ct);
    Task<IEnumerable<Subscription>> GetAllFromRegionAsync(Region region, CancellationToken ct);
}
