using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.CommentApi.Clients;

public interface IArticleClient
{
    public Task<IEnumerable<ArticleDto>> GetLatestArticlesAsync(int count);
}
