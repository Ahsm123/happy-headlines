using HappyHeadlines.CommentApi.Models;
using HappyHeadlines.Contracts.Comments;

namespace HappyHeadlines.CommentApi.Extensions;

public static class CommentMappingExtensions
{
    public static CommentDto ToDto(this Comment comment) =>
        new(
            comment.Id,
            comment.ArticleId,
            comment.CommentText,
            comment.IsFiltered
        );
}
