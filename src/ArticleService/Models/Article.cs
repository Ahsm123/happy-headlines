using System.ComponentModel.DataAnnotations;

namespace ArticleService.Models;

public class Article
{
    public int Id {get; set;}
    [MaxLength(200)]public required string Title {get; set;}
    [MaxLength(20000)]public required string Content {get; set;}
    [MaxLength(200)]public required string Author {get; set;}
    public DateTime PublishDate {get; set;}
}
