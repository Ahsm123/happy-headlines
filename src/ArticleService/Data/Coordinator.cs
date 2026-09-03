using ArticleService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data;

public class Coordinator
{
    public ArticleDbContext GetArticleDbContext(Region region) =>
        new(new DbContextOptionsBuilder<ArticleDbContext>()
            .UseNpgsql($"Host={HostFor(region)};Port=5432;Database=articledb;Username=postgres;Password=dev")
            .Options);

    private static string HostFor(Region region) => region switch
    {
        Region.Global       => "global-db",
        Region.Africa       => "africa-db",
        Region.Antarctica   => "antarctica-db",
        Region.Asia         => "asia-db",
        Region.Europe       => "europe-db",
        Region.NorthAmerica => "north-america-db",
        Region.Oceania      => "oceania-db",
        Region.SouthAmerica => "south-america-db",
        _ => throw new ArgumentOutOfRangeException(nameof(region), region, "Unknown region.")
    };
}