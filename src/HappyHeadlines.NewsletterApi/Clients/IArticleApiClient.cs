using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.NewsletterApi.Clients;

public interface IArticleApiClient
{
    Task<List<ArticleDto>> GetTodaysArticles(Region region);
}
