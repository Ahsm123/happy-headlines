namespace ArticleService.Models;

public class ArticleDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string Author { get; set; }
    public DateTime PublishDate { get; set; }
}