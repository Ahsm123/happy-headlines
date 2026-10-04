using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.CommentApi.Clients;

public sealed class ArticleClient(HttpClient client) : IArticleClient
{
    public async Task<IEnumerable<ArticleDto>> GetLatestArticlesAsync(int count)
    {
        var response = await client.GetAsync($"api/v1/latest/articles?count={count}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<ArticleDto>>()
                     ?? throw new InvalidOperationException("ArticleApi returned null");
        return result;
    }
}
