using HappyHeadlines.DraftApi.Data;
using HappyHeadlines.DraftApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.DraftApi.Services;

public sealed class DraftService(
    DraftDbContext db,
    ILogger<DraftService> logger) : IDraftService
{
    public async Task<Draft> CreateAsync(Draft draft, CancellationToken ct = default)
    {
        draft.Created = draft.Updated = DateTime.UtcNow;
        db.Drafts.Add(draft);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Created draft {DraftId} titled {Title}", draft.Id, draft.Title);
        return draft;
    }

    public async Task<IEnumerable<Draft>> GetAllAsync(CancellationToken ct = default)
    {
        var drafts = await db.Drafts.ToListAsync(ct);
        logger.LogInformation("Retrieved {DraftCount} drafts", drafts.Count);
        return drafts;
    }

    public async Task<Draft?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Drafts.FindAsync([id], ct);
}
