using DraftService.Models;
using DraftService.Services;
using Microsoft.AspNetCore.Mvc;
using ServiceDefaults;

namespace DraftService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DraftsController(IDraftService draftService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Draft>> CreateAsync(Draft? draft, CancellationToken ct = default)
    {
        using var activity = MonitorService.ActivitySource.StartActivity();
        
        if (draft == null)
        {
            return BadRequest();
        }
        var result = await draftService.CreateAsync(draft, ct);
        return Ok(result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Draft>>> GetDrafts(CancellationToken ct = default)
    {
        using var activity = MonitorService.ActivitySource.StartActivity();
        
        var drafts = await draftService.GetAsync(ct);
        return Ok(drafts);
    }
    
}