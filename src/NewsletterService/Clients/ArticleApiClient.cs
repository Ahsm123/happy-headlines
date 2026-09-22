using ServiceDefaults.Contracts;

namespace NewsletterService.Clients;

public class ArticleApiClient(HttpClient httpClient) : IArticleApiClient
{
    public async Task<List<ArticleDto>> GetTodaysArticles(Region region)
    {
        //from the start of today
        var fromDate = DateTime.Today;
        var fromDateString = fromDate.ToString("O");

        var response = await httpClient.GetAsync($"/api/v1/regions/{region}/articles?fromDate={fromDateString}");
        var articles = await response.Content.ReadFromJsonAsAsyncEnumerable<ArticleDto>().ToListAsync();
        return articles;
    }

}