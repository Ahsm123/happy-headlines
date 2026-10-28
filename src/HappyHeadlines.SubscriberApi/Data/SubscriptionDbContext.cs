using HappyHeadlines.SubscriberApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.SubscriberApi.Data;

public class SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : DbContext(options)
{
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>()
            .HasIndex(s => new { s.Email, s.SubscribedRegion })
            .IsUnique();
    }
}
