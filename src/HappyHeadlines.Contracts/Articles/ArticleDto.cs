namespace HappyHeadlines.Contracts.Articles;

public record ArticleDto(Guid Id, string Title, string Content, string Author, DateTime PublishDate, Region Region);
