using HappyHeadlines.Contracts.Profanity;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.ProfanityApi.Controllers;

[ApiController]
[Route("api/v1/profanities")]
public class ProfanitiesController : ControllerBase
{
    [HttpPost("filter")]
    public ActionResult<FilterResult> Filter([FromBody] FilterRequest request)
    {
        return new FilterResult(request.Text);
    }
}
