using CommentService.Clients;
using CommentService.Data;
using CommentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Polly.CircuitBreaker;
using ServiceDefaults;

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
        catch (Exception ex) when (ex is BrokenCircuitException or HttpRequestException)
        {
            comment.IsFiltered = false;
            MonitorService.Log.Here().Warning(ex, "ProfanityService unavailable, saving unfiltered comment for article {ArticleId}", comment.ArticleId);
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
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments([FromQuery, BindRequired] Guid article)
    {
        return await commentDbContext.Comments                                                                                  
            .Where(c => c.ArticleId == article)                                                                                 
            .ToListAsync();
    }
}
