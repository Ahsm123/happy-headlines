using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.CommentApi.Clients;

public class ArticleClient(HttpClient client) : IArticleClient
{
    public async Task<IEnumerable<ArticleDto>> GetNewestArticlesAsync(int count)
    {
        var response = await client.GetAsync($"api/v1/newest/Articles?count={count}");
        response.EnsureSuccessStatusCode();
        //
        var result = await response.Content.ReadFromJsonAsync<List<ArticleDto>>()
                     ?? throw new InvalidOperationException("ArticleApi returned null");
        return result;
    }
}