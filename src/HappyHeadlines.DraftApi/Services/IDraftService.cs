using HappyHeadlines.DraftApi.Models;

namespace HappyHeadlines.DraftApi.Services;

public interface IDraftService
{
    Task<Draft> CreateAsync(Draft draft, CancellationToken ct = default);
    Task<IEnumerable<Draft>> GetAllAsync(CancellationToken ct = default);
    Task<Draft?> GetByIdAsync(Guid id, CancellationToken ct = default);
}