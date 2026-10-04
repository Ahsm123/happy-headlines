using System.ComponentModel.DataAnnotations;

namespace HappyHeadlines.DraftApi.Models;

public class Draft
{
    public Guid Id { get; set; }
    [MaxLength(200)]public required string  Title { get; set; }
    [MaxLength(20000)]public required string Content { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
