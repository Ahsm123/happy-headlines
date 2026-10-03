using HappyHeadlines.Contracts.Articles;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HappyHeadlines.ArticleApi.Models;

public class Article
{
    public Guid Id {get; set;}
    [MaxLength(200)]public required string Title {get; set;}
    [MaxLength(20000)]public required string Content {get; set;}
    [MaxLength(200)]public required string Author {get; set;}
    public DateTime PublishDate {get; set;}
    public Region Region { get; set; }
}
