using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Subscribers;
using HappyHeadlines.SubscriberApi.Models;

namespace HappyHeadlines.SubscriberApi.Services;

public interface ISubscriberService
{
    Task<bool> SubscribeAsync(SubscribeRequest request, CancellationToken ct);
    Task<bool> UnsubscribeAsync(UnsubscribeRequest request, CancellationToken ct);
    Task<IEnumerable<Subscriber>> GetAllFromRegionAsync(Region region, CancellationToken ct);
}
