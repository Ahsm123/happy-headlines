using ServiceDefaults.Contracts;

namespace CommentService.Clients;

public interface IArticleClient
{
    public Task<IEnumerable<ArticleDto>> GetNewestArticlesAsync(int count);
}