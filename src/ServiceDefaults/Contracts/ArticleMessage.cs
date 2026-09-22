namespace ServiceDefaults.Contracts;

public record ArticleMessage(
    Guid ArticleId, 
    string Title, 
    string Content, 
    string Author, 
    DateTime PublishDate, 
    Region Region);

