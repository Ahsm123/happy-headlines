using HappyHeadlines.CommentApi.Models;
using HappyHeadlines.Contracts.Comments;

namespace HappyHeadlines.CommentApi.Services;

public interface ICommentService
{
    Task<Comment> CreateAsync(CreateCommentRequest request, CancellationToken ct);
    Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Comment>> GetAllAsync(Guid articleId, CancellationToken ct);
}
