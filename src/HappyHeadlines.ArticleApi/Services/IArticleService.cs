using HappyHeadlines.ArticleApi.Models;
using HappyHeadlines.Contracts.Articles;
using HappyHeadlines.Contracts.Events;

namespace HappyHeadlines.ArticleApi.Services;

public interface IArticleService
{
    Task CreateAsync(ArticlePublishedEvent publishedEvent, CancellationToken ct);
    Task<Article?> GetByIdAsync(Region region, Guid id, CancellationToken ct);
    Task<IEnumerable<Article>> GetAllAsync(Region region, DateTime? fromDate, CancellationToken ct);
    Task<bool> UpdateAsync(Region region, Guid id, UpdateArticleRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Region region, Guid id, CancellationToken ct);
    Task<IEnumerable<Article>> GetLatestAsync(int count, CancellationToken ct);
}
