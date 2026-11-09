using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.ArticleApi.Data;

public interface ICoordinator
{
    ArticleDbContext GetArticleDbContext(Region region);
}
