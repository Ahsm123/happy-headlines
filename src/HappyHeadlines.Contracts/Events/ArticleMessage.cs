using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.Contracts.Events;

public record ArticleMessage(
    Guid ArticleId, 
    string Title, 
    string Content, 
    string Author, 
    DateTime PublishDate, 
    Region Region);

