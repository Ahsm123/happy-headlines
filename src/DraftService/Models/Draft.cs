namespace DraftService.Models;

public class Draft
{
    public Guid Id { get; set; }
    public string  Title { get; set; }
    public string Content { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}