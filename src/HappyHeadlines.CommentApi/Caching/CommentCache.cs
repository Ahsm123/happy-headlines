using System.Text.Json;
using HappyHeadlines.CommentApi.Data;
using HappyHeadlines.CommentApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace HappyHeadlines.CommentApi.Caching;

// Cache med LRU, der har comments for de seneste 30 tilgåede artikler i Redis.
// Der er to slags data i Redis:
// 1. Cachen
// 2. LRU liste
//
// Cachen har en key pr. article hvor key er "CommentApi"+articleId, og value er artiklens comments.
// LRU listen er et sorted set, som er en liste af articleId's, hvor hver har en score som er tidspunktet for
// sidste læsning. Der er ingen comments her, den bruges kun til at holde styr på hvilken article der skal fjernes fra cache.
// Listen er sorteret efter score(tidspunkt), så den med laveste score er den der er længst tid siden læst, og den fjernes først.

public class CommentCache(
    IDistributedCache cache,
    CommentDbContext dbContext,
    IConnectionMultiplexer redis,
    ILogger<CommentCache> logger)
{
    // To counters i Redis som dashboard bruger til at beregne hit ratio.
    private const string HitsKey = "CommentApi:cache:hits";
    private const string MissesKey = "CommentApi:cache:misses";

    // Redis-key for LRU-listen
    // member: articleId
    // score: tidspunkt artiklen sidst blev læst.
    private const string LruKey = "CommentApi:lru";

    // Der må højst være comments for 30 artikler i cachen, altså højst 30 articleId'er i LRU-listen.
    private const int Capacity = 30;

    private Task OnCacheHit() => redis.GetDatabase().StringIncrementAsync(HitsKey);
    private Task OnCacheMiss() => redis.GetDatabase().StringIncrementAsync(MissesKey);

    public async Task<(long Hits, long Misses)> Stats()
    {
        var db = redis.GetDatabase();
        return ((long)await db.StringGetAsync(HitsKey), (long)await db.StringGetAsync(MissesKey));
    }

    // Kaldes, når der bliver oprettet en ny comment. Vi sletter artiklens comments fra cachen,
    // så næste læsning bliver et cache miss, henter den friske liste fra DB og cacher den igen.
    // Dermed er data ikke stale. Artiklen bliver stående i LRU-listen. Det er ikke et problem,
    // for den bliver bare fyldt i cachen igen ved næste læsning.
    public async Task InvalidateCacheEntry(Guid articleId)
    {
        await cache.RemoveAsync(articleId.ToString());
    }

    public async Task<List<Comment>> Comments(Guid articleId)
    {
        var id = articleId.ToString();
        var db = redis.GetDatabase();

        // Opdaterer i LRU-listen, hvornår artiklen sidst er læst. Det skal ske både ved hit og miss,
        // sådan at en populær artikel bliver ved med at være "senest brugt" og ikke bliver evicted.
        // SortedSetAdd tilføjer artiklen, hvis den ikke er i LRU-listen, og ellers opdateres scoren hvis den er.
        await db.SortedSetAddAsync(LruKey, id, DateTime.UtcNow.Ticks);

        var cacheHit = await cache.GetStringAsync(id);
        if (cacheHit is not null)
        {
            await OnCacheHit();
            return JsonSerializer.Deserialize<List<Comment>>(cacheHit) ?? [];
        }

        // Cache miss = hent fra databasen og læg i cachen. Der er ingen TTL.
        // Størrelsen styres kun af LRU-eviction nedenunder, og freshness styres af
        // InvalidateCacheEntry, når der kommer nye comments.
        await OnCacheMiss();
        var comments = await dbContext.Comments
            .Where(c => c.ArticleId == articleId)
            .ToListAsync();

        await cache.SetAsync(id, JsonSerializer.SerializeToUtf8Bytes(comments));
        await EvictLeastRecentlyUsed(db);

        return comments;
    }

    // Hvis der er mere end 30 artikler i LRU-listen, fjernes dem med de laveste scores.
    // De fjernes begge steder, deres comments slettes fra cachen, og deres articleId slettes fra LRU-listen.
    private async Task EvictLeastRecentlyUsed(IDatabase db)
    {
        // Fx 31 artikler i listen - 30 = 1 artikel for meget.
        var overflow = await db.SortedSetLengthAsync(LruKey) - Capacity;
        if (overflow <= 0)
        {
            return;
        }
        
        // Rank er pladsen i den sorted list: rank 0 = laveste score = længst tid siden brugt.
        // Vi henter articleId på index 0 til overflow-1, altså de overflow artikler, der blev læst for længst tid siden.
        var victims = await db.SortedSetRangeByRankAsync(LruKey, 0, overflow - 1);
        foreach (var victim in victims)
        {
            await cache.RemoveAsync(victim.ToString());
        }

        // Fjerner de samme articleId'er fra LRU-listen.
        await db.SortedSetRemoveRangeByRankAsync(LruKey, 0, overflow - 1);
        logger.LogInformation("Evicted {Count} articles from comment cache", victims.Length);
    }
}
