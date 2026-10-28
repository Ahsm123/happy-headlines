using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Subscribers;
using HappyHeadlines.SubscriberApi.Models;

namespace HappyHeadlines.SubscriberApi.Services;

public class SubscriberService : ISubscriberService
{
    public Task<bool> SubscribeAsync(SubscribeRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UnsubscribeAsync(UnsubscribeRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Subscriber>> GetAllFromRegionAsync(Region region, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
