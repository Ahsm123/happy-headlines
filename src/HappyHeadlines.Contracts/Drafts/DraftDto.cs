namespace HappyHeadlines.Contracts.Drafts;

public record DraftDto(
    Guid Id, 
    string Title, 
    string Content, 
    DateTime Created, 
    DateTime Updated);
