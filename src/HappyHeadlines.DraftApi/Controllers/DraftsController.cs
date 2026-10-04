using HappyHeadlines.Contracts.Drafts;
using HappyHeadlines.DraftApi.Extensions;
using HappyHeadlines.DraftApi.Models;
using HappyHeadlines.DraftApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HappyHeadlines.DraftApi.Controllers;

[ApiController]
[Route("api/v1/drafts")]
public class DraftsController(IDraftService draftService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<DraftDto>> Create(CreateDraftRequest request, CancellationToken ct)
    {
        var draft = await draftService.CreateAsync(request, ct);
        var dto = draft.ToDto();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DraftDto>>> GetAll(CancellationToken ct)
    {
        var drafts = await draftService.GetAllAsync(ct);

        return Ok(drafts.Select(d => d.ToDto()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DraftDto>> GetById(Guid id, CancellationToken ct)
    {
        var draft = await draftService.GetByIdAsync(id, ct);
        if (draft is null)
        {
            return NotFound();
        }

        return Ok(draft.ToDto());
    }
}
