namespace CommentService.Clients;

public interface IProfanityClient
{
    public Task<string> FilterAsync(string commentText);
}