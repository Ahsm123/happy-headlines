namespace HappyHeadlines.Contracts.Articles;

public record ArticleSummaryDto(Guid Id, string Title, string Teaser, string Author, DateTime PublishDate, Region Region);
