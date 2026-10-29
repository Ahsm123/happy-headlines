using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.Contracts.Subscribers;
using HappyHeadlines.ServiceDefaults;
using HappyHeadlines.SubscriberApi.Data;
using HappyHeadlines.SubscriberApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.SubscriberApi.Services;

public class SubscriptionService(
    SubscriptionDbContext db,
    IMessageClient messageClient,
    ILogger<SubscriptionService> logger
) : ISubscriptionService
{
    public async Task<bool> SubscribeAsync(SubscribeRequest request, CancellationToken ct)
    {
        var result =
            await db.Subscriptions.AnyAsync(s => s.Email == request.Email && s.SubscribedRegion == request.Region, ct);
        if (result)
        {
            return false;
        }

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            SubscribedRegion = request.Region,
        };

        await db.Subscriptions.AddAsync(subscription, ct);
        await db.SaveChangesAsync(ct);
        
        var message = new NewSubscriptionEvent(subscription.Email, subscription.SubscribedRegion);
        await messageClient.PublishAsync<NewSubscriptionEvent>(message, ct);
        
        return true;
    }

    public async Task<bool> UnsubscribeAsync(UnsubscribeRequest request, CancellationToken ct)
    {
        var subscription =
            await db.Subscriptions.FirstOrDefaultAsync(
                s => s.Email == request.Email && s.SubscribedRegion == request.Region, ct);
        if (subscription is null)
        {
            return false;
        }

        db.Subscriptions.Remove(subscription);
        await db.SaveChangesAsync(ct);

        return true;
    }

    public Task<IEnumerable<Subscription>> GetAllFromRegionAsync(Region region, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
