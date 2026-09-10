using CommentService.Models;

namespace CommentService.Clients;

public class ProfanityClient(HttpClient client) : IProfanityClient
{
    public async Task<string> FilterAsync(string commentText)
    {
        var response = await client.PostAsJsonAsync("api/v1/profanities/filter", new { Text = commentText });
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FilterResult>()
            ?? throw new InvalidOperationException("ProfanityService returned null");
        return result.CleanedText;
    }
}