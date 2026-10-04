namespace HappyHeadlines.Contracts.Comments;

public record CreateCommentRequest(Guid ArticleId, string CommentText);
