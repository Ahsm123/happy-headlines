using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.Contracts.Subscribers;

public record UnsubscribeRequest(string Email, Region Region);
