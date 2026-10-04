using HappyHeadlines.Contracts.Drafts;
using HappyHeadlines.DraftApi.Data;
using HappyHeadlines.DraftApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.DraftApi.Services;

public sealed class DraftService(
    DraftDbContext db,
    ILogger<DraftService> logger) : IDraftService
{
    public async Task<Draft> CreateAsync(CreateDraftRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var draft = new Draft
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            Created = now,
            Updated = now
        };

        db.Drafts.Add(draft);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Created draft {DraftId} titled {Title}", draft.Id, draft.Title);
        return draft;
    }

    public async Task<IEnumerable<Draft>> GetAllAsync(CancellationToken ct)
    {
        var drafts = await db.Drafts.ToListAsync(ct);
        logger.LogInformation("Retrieved {DraftCount} drafts", drafts.Count);
        return drafts;
    }

    public async Task<Draft?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await db.Drafts.FindAsync([id], ct);
}
