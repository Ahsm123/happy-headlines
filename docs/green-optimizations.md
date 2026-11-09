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

## 3. Index on PublishDate

**Tactic:** Use Efficient Databases

Latest, the daily newsletter and the cache warmup all sort or filter articles on `PublishDate`, but the column had no
index. Every query read the whole table, and latest sorted all of it to find the 50 newest.

**Before**

```csharp
public class ArticleDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
}
```

**After**

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Article>()
        .HasIndex(a => a.PublishDate);
}
```

No queries changed. Postgres picks up the index by itself, so latest reads the 50 newest rows straight from the index
and stops, and date filters jump to the matching rows instead of scanning the table. The cost is a bit of disk and
slightly slower writes, which is a good deal for articles that are written once and read often.

## 4. Log once per newsletter, not once per subscriber

**Tactic:** Reduce Logging

The newsletter service wrote one log line per subscriber, with their email address. With 10,000 subscribers that is
10,000 lines to Seq for every published article, plus the same again for every daily newsletter.

**Before**

```csharp
foreach (var sub in subs)
{
    if (sub.Region == publishedEvent.Region)
    {
        logger.LogInformation("Sending article {ArticleTitle} to {SubscriberEmail}",
            publishedEvent.Title, sub.Email);
    }
}
```

**After**

```csharp
var sentCount = 0;
foreach (var sub in subs)
{
    if (sub.Region == publishedEvent.Region)
    {
        // Send newsletter to subscriber
        sentCount++;
    }
}

logger.LogInformation("Article: {ArticleTitle} was sent to {SubCount} subscribers in {Region}",
    publishedEvent.Title, sentCount, publishedEvent.Region);
```

The daily newsletter got the same change. One line per send-out instead of one per subscriber, so the amount of logging
no longer grows with the number of subscribers. As a bonus, email addresses are no longer stored in the logs.
