using DraftService.Data;
using DraftService.Models;
using Microsoft.EntityFrameworkCore;
using ServiceDefaults;

namespace DraftService.Services;

public class DraftManager(DraftDbContext db) : IDraftService
{
    public async Task<Draft> CreateAsync(Draft draft, CancellationToken ct = default)
    {
        using var activity = MonitorService.ActivitySource.StartActivity();
        
        MonitorService.Log.Information("Creating draft titled {Title}", draft.Title);
        draft.Created = draft.Updated = DateTime.UtcNow;
        db.Drafts.Add(draft);
        await db.SaveChangesAsync(ct);
        MonitorService.Log.Information("Created draft {DraftId} titled {Title}", draft.Id, draft.Title);
        return draft;
    }

    public async Task<IEnumerable<Draft>> GetAsync(CancellationToken ct = default)
    {
        using var activity = MonitorService.ActivitySource.StartActivity();
        
        var drafts = await db.Drafts.ToListAsync(ct);
        MonitorService.Log.Information("Retrieved {DraftCount} drafts", drafts.Count);
        return drafts;
    }
}
