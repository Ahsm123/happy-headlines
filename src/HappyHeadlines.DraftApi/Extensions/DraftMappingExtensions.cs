using HappyHeadlines.Contracts.Drafts;
using HappyHeadlines.DraftApi.Models;

namespace HappyHeadlines.DraftApi.Extensions;

public static class DraftMappingExtensions
{
    public static DraftDto ToDto(this Draft draft) =>
        new(
            draft.Id,
            draft.Title,
            draft.Content,
            draft.Created,
            draft.Updated
        );
}
