using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Subscribers;
using HappyHeadlines.SubscriberApi.Extensions;
using HappyHeadlines.SubscriberApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.SubscriberApi.Controllers;

[ApiController]
[Route("api/v1/subscribers")]
public class SubscriptionsController(ISubscriptionService subscriptionService) : ControllerBase
{
    [HttpPost("subscribe")]
    public async Task<ActionResult> Subscribe([FromBody] SubscribeRequest request, CancellationToken ct)
    {
        await subscriptionService.SubscribeAsync(request, ct);
        return NoContent();
    }

    [HttpPost("unsubscribe")]
    public async Task<ActionResult> Unsubscribe([FromBody] UnsubscribeRequest request, CancellationToken ct)
    {
        await subscriptionService.UnsubscribeAsync(request, ct);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetAllFromRegion(Region region, CancellationToken ct)
    {
        var subscribers = await subscriptionService.GetAllFromRegionAsync(region, ct);
        return Ok(subscribers.Select(s => s.ToDto()));
    }
    
    
}
