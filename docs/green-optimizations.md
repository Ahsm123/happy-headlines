# Green optimizations

Changes to Happy Headlines based on tactics from the Green Architecture Framework (GAF).

## 1. Return summaries from latest

**Tactic:** Avoid Unnecessary Resources

The latest endpoint fetched full articles from all 8 regions and threw most of them away. With `count=10` that is 80
articles of up to 20,000 characters each, to return 10.

**Before**

```csharp
var regionArticles = await db.Articles
    .OrderByDescending(a => a.PublishDate)
    .Take(count)
    .ToListAsync(ct);
```

**After**

```csharp
count = Math.Clamp(count, 0, MaxLatestCount);
...
.Take(count)
.Select(a => new ArticleSummaryDto(
    a.Id,
    a.Title,
    a.Content.Substring(0, a.Content.Length > TeaserLength ? TeaserLength : a.Content.Length),
    a.Author,
    a.PublishDate,
    a.Region))
.ToListAsync(ct);
```

Only a 200-character teaser now leaves the database, and `count` is capped at 50. The full article is fetched when the
reader opens it.

## 2. Cache latest

**Tactic:** Fetch Data from the Proximity

The latest list is the same for everyone, but every request still queried all 8 regions.

**Before**

```csharp
public async Task<IEnumerable<ArticleSummaryDto>> GetLatestAsync(int count, CancellationToken ct)
{
    // Query all 8 regions on every request
}
```

**After**

```csharp
public async Task<IEnumerable<ArticleSummaryDto>> GetLatestAsync(int count, CancellationToken ct)
{
    count = Math.Clamp(count, 0, MaxLatestCount);

    var latest = await cache.GetLatestAsync();
    if (latest is null)
    {
        latest = await FetchLatestFromRegions(ct);
        await cache.SetLatestAsync(latest);
    }

    return latest.Take(count);
}
```

The top 50 is cached in Redis for 5 minutes, so most requests never touch a database. The trade-off is that the list
can be up to 5 minutes old.
