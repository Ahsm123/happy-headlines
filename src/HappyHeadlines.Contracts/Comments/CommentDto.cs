namespace HappyHeadlines.Contracts.Comments;

public record CommentDto(Guid Id, Guid ArticleId, string CommentText, bool IsFiltered);
