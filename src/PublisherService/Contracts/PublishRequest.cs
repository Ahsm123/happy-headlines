using ServiceDefaults.Contracts;

namespace PublisherService.Contracts;

public record PublishRequest(
    string Title, 
    string Content, 
    string Author, 
    Region Region);