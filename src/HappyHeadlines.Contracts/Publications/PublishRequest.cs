using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.Contracts.Publications;

public record PublishRequest(
    string Title, 
    string Content, 
    string Author, 
    Region Region);