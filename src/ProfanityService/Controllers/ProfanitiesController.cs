using Microsoft.AspNetCore.Mvc;
using ProfanityService.Models;

namespace ProfanityService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProfanitiesController : ControllerBase
{
    [HttpPost("filter")]
    public ActionResult<FilterResult> Filter([FromBody] FilterRequest request)
    {
        return new FilterResult(request.Text);
    }

}
