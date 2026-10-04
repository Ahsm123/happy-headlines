using HappyHeadlines.DraftApi.Models;
using HappyHeadlines.DraftApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.DraftApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DraftsController(IDraftService draftService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Draft>> CreateAsync(Draft draft, CancellationToken ct = default)
    {
        var result = await draftService.CreateAsync(draft, ct);
        return CreatedAtAction(nameof(GetDraft), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Draft>>> GetDrafts(CancellationToken ct = default)
    {
        var drafts = await draftService.GetAllAsync(ct);
        return Ok(drafts);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Draft>> GetDraft(Guid id, CancellationToken ct = default)
    {
        var draft = await draftService.GetByIdAsync(id, ct);
        return draft is null ? NotFound() : draft;
    }
}
