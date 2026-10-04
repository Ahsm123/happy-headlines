namespace HappyHeadlines.Contracts.Articles;

public record UpdateArticleRequest(string Title, string Content, string Author, DateTime PublishDate);
