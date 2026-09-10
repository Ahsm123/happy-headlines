using CommentService.Clients;
using CommentService.Data;
using CommentService.Models;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;

namespace CommentService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CommentsController(
    IProfanityClient profanityClient,
    CommentDbContext commentDbContext) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Comment>> PostComment(Comment comment)
    {
        try
        {
            comment.CommentText = await profanityClient.FilterAsync(comment.CommentText);
            comment.IsFiltered = true;
        }
        catch (BrokenCircuitException)
        {
            comment.IsFiltered = false;
        }
        
        commentDbContext.Comments.Add(comment);
        await commentDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Comment>> GetComment(Guid id)
    {
        var comment = await commentDbContext.Comments.FindAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        return comment;
    }
}
