using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;
using HappyHeadlines.Contracts.Publications;
using HappyHeadlines.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.PublisherApi.Controllers;

[ApiController]
[Route("api/v1/publications")]
public class PublicationsController(IMessageClient client) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult<Guid>> PublishArticle([FromBody] PublishRequest request)
    {
        var message = new ArticlePublishedEvent(
            Guid.NewGuid(), 
            request.Title, 
            request.Content, 
            request.Author, 
            DateTime.UtcNow, 
            request.Region);
        
        await client.PublishAsync<ArticlePublishedEvent>(message);
        return Accepted(message.ArticleId); 
    }
}
