using HappyHeadlines.DraftApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyHeadlines.DraftApi.Data;

public class DraftDbContext : DbContext
{
    public DraftDbContext(DbContextOptions<DraftDbContext> options)
        : base(options) {}

    public DbSet<Draft> Drafts { get; set; }
}
