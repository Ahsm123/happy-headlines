using HappyHeadlines.Contracts.Articles;

namespace HappyHeadlines.Contracts.Events;

public record NewSubscriptionEvent(string Email, Region Region);
