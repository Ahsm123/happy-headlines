using ServiceDefaults.Contracts;

namespace NewsletterService.Clients;

public interface IArticleApiClient
{
    Task<List<ArticleDto>> GetTodaysArticles(Region region);
}