using DraftService.Data;
using DraftService.Models;
using Microsoft.EntityFrameworkCore;

namespace DraftService.Services;

public class DraftManager(DraftDbContext db) : IDraftService
{
    public async Task<Draft> CreateAsync(Draft draft, CancellationToken ct = default)
    {
        draft.Created = draft.Updated = DateTime.UtcNow;
        db.Drafts.Add(draft);
        await db.SaveChangesAsync(ct);
        return draft;
    }

    public async Task<IEnumerable<Draft>> GetAsync(CancellationToken ct = default)
    {
        return await db.Drafts.ToListAsync(ct);
    }
}
