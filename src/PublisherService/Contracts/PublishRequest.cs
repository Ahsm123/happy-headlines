namespace ServiceDefaults.Contracts;

public record PublishRequest(
    string Title, 
    string Content, 
    string Author, 
    Region Region);