using HappyHeadlines.CommentApi.Caching;
using HappyHeadlines.CommentApi.Clients;
using HappyHeadlines.CommentApi.Data;
using HappyHeadlines.CommentApi.Models;
using HappyHeadlines.Contracts.Comments;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace HappyHeadlines.CommentApi.Services;

public sealed class CommentService(
    IProfanityClient profanityClient,
    CommentDbContext db,
    CommentCache cache,
    ILogger<CommentService> logger) : ICommentService
{
    public async Task<Comment> CreateAsync(CreateCommentRequest request, CancellationToken ct)
    {
        var comment = new Comment
        {
            ArticleId = request.ArticleId,
            CommentText = request.CommentText,
        };
        try
        {
            comment.CommentText = await profanityClient.FilterAsync(comment.CommentText, ct);
            comment.IsFiltered = true;
        }
        catch (Exception ex) when (ex is BrokenCircuitException or HttpRequestException or TimeoutRejectedException)
        {
            comment.IsFiltered = false;
            logger.LogWarning(ex,
                "ProfanityApi unavailable, saving unfiltered comment for article {ArticleId}", comment.ArticleId);
        }

        db.Comments.Add(comment);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Created comment with ID: {CommentId} on article: {ArticleId}",
            comment.Id, comment.ArticleId);

        await cache.InvalidateCacheEntry(comment.ArticleId);

        return comment;
    }

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await db.Comments.FindAsync([id], ct);
    }
    
    public async Task<IEnumerable<Comment>> GetAllAsync(Guid articleId, CancellationToken ct)
    {
        return await cache.Comments(articleId);
    }
}
