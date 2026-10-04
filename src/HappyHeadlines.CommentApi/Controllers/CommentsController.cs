using HappyHeadlines.CommentApi.Extensions;
using HappyHeadlines.CommentApi.Models;
using HappyHeadlines.CommentApi.Services;
using HappyHeadlines.Contracts.Comments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HappyHeadlines.CommentApi.Controllers;

[ApiController]
[Route("api/v1/comments")]
public class CommentsController(
    ICommentService commentService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<CommentDto>> Create(CreateCommentRequest request, CancellationToken ct)
    {
        var comment = await commentService.CreateAsync(request, ct);
        var dto = comment.ToDto();
        
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> GetById(Guid id, CancellationToken ct)
    {
        var comment = await commentService.GetByIdAsync(id, ct);
        if (comment is null)
        {
            return NotFound();
        }
        
        return Ok(comment.ToDto());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetAll([FromQuery, BindRequired] Guid articleId, CancellationToken ct)
    {
        var comments = await commentService.GetAllAsync(articleId, ct);
        
        return Ok(comments.Select(c => c.ToDto()));
    }
}
    
