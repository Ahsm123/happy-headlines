namespace HappyHeadlines.CommentApi.Clients;

public interface IProfanityClient
{
    public Task<string> FilterAsync(string commentText, CancellationToken ct);
}
