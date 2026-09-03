using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ArticleService.Models;

[JsonConverter(typeof(JsonStringEnumConverter<Region>))]
public enum Region
{
    Africa,
    Antarctica,
    Asia,
    Europe,
    Global,
    NorthAmerica,
    Oceania,
    SouthAmerica
}
public class Article
{
    public int Id {get; set;}
    [MaxLength(200)]public required string Title {get; set;}
    [MaxLength(20000)]public required string Content {get; set;}
    [MaxLength(200)]public required string Author {get; set;}
    public DateTime PublishDate {get; set;}
    public Region Region { get; set; }
}
