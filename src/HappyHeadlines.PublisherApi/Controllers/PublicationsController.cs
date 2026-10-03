using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.Contracts.Publications;
using HappyHeadlines.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.PublisherApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PublicationsController(IMessageClient client) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult<Guid>> PublishArticle([FromBody] PublishRequest request)
    {
        var message = new ArticleMessage(
            Guid.NewGuid(), 
            request.Title, 
            request.Content, 
            request.Author, 
            DateTime.UtcNow, 
            request.Region);
        
        await client.PublishAsync<ArticleMessage>(message);
        return Accepted(message.ArticleId); 
    }
}