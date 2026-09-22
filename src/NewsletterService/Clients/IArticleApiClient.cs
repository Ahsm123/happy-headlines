using ServiceDefaults.Contracts;

namespace NewsletterService.Clients;

public interface IArticleClient
{
    Task<List<ArticleDto>> GetTodaysArticles(Region region, DateTime? fromDate);
}