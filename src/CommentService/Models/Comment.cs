namespace CommentService.Models;

public class Comment
{
    public Guid Id { get; set; }
    public int ArticleId { get; set; }
    public string CommentText { get; set; }
    public bool IsFiltered { get; set; } = false;
}