using HappyHeadlines.Contracts.Drafts;
using HappyHeadlines.DraftApi.Models;

namespace HappyHeadlines.DraftApi.Services;

public interface IDraftService
{
    Task<Draft> CreateAsync(CreateDraftRequest request, CancellationToken ct);
    Task<IEnumerable<Draft>> GetAllAsync(CancellationToken ct);
    Task<Draft?> GetByIdAsync(Guid id, CancellationToken ct);
}
