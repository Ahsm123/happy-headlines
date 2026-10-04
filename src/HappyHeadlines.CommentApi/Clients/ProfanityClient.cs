using HappyHeadlines.Contracts.Profanity;

namespace HappyHeadlines.CommentApi.Clients;

public sealed class ProfanityClient(HttpClient client) : IProfanityClient
{
    public async Task<string> FilterAsync(string commentText, CancellationToken ct)
    {
        var response = await client.PostAsJsonAsync("api/v1/profanities/filter", new FilterRequest(Text: commentText), ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FilterResult>(ct)
            ?? throw new InvalidOperationException("ProfanityApi returned null");
        return result.CleanedText;
    }
}
