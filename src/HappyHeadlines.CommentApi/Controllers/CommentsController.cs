using HappyHeadlines.CommentApi.Clients;
using HappyHeadlines.CommentApi.Data;
using HappyHeadlines.CommentApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Polly.CircuitBreaker;
using HappyHeadlines.ServiceDefaults;

namespace HappyHeadlines.CommentApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CommentsController(
    IProfanityClient profanityClient,
    CommentDbContext commentDbContext,
    CommentCache cache) : ControllerBase
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
            MonitorService.Log.Here().Warning(ex,
                "ProfanityApi unavailable, saving unfiltered comment for article {ArticleId}", comment.ArticleId);
        }

        commentDbContext.Comments.Add(comment);
        await commentDbContext.SaveChangesAsync();
        MonitorService.Log.Here().Information("Created comment with ID: {CommentId} on article: {ArticleId}",
            comment.Id, comment.ArticleId);

        await cache.InvalidateCacheEntry(comment.ArticleId);

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
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments([FromQuery, BindRequired] Guid articleId)
    {
        return await cache.Comments(articleId);
    }
}