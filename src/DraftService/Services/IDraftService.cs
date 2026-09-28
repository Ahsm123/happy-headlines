using DraftService.Models;

namespace DraftService.Services;

public interface IDraftService
{
    Task<Draft> CreateAsync(Draft draft, CancellationToken ct = default);
    Task<IEnumerable<Draft>> GetAllAsync(CancellationToken ct = default);
    Task<Draft?> GetByIdAsync(Guid id, CancellationToken ct = default);
}