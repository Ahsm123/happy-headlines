using DraftService.Models;

namespace DraftService.Services;

public interface IDraftService
{
    Task<Draft> CreateAsync(Draft draft, CancellationToken ct = default);
    Task<IEnumerable<Draft>> GetAsync(CancellationToken ct = default);
}