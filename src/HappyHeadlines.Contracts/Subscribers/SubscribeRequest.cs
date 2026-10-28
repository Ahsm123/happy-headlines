using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.Contracts.Subscribers;

public record SubscribeRequest(string Email, Region Region);
