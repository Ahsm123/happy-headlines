using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.CommentApi.Clients;

public interface IArticleClient
{
    public Task<IEnumerable<ArticleDto>> GetNewestArticlesAsync(int count);
}