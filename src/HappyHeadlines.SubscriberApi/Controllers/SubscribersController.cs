using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Subscribers;
using HappyHeadlines.SubscriberApi.Extensions;
using HappyHeadlines.SubscriberApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.SubscriberApi.Controllers;

[ApiController]
[Route("api/v1/subscribers")]
public class SubscribersController(ISubscriberService subscriberService) : ControllerBase
{
    [HttpPost("subscribe")]
    public async Task<ActionResult> Subscribe([FromBody] SubscribeRequest request, CancellationToken ct)
    {
        await subscriberService.SubscribeAsync(request, ct);
        return NoContent();
    }

    [HttpPost("unsubscribe")]
    public async Task<ActionResult> Unsubscribe([FromBody] UnsubscribeRequest request, CancellationToken ct)
    {
        await subscriberService.UnsubscribeAsync(request, ct);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriberDto>>> GetAllFromRegion(Region region, CancellationToken ct)
    {
        var subscribers = await subscriberService.GetAllFromRegionAsync(region, ct);
        return Ok(subscribers.Select(s => s.ToDto())); 
    }
}
