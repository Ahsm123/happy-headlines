using Microsoft.EntityFrameworkCore;
using HappyHeadlines.ProfanityApi.Models;

namespace HappyHeadlines.ProfanityApi.Data;

public class ProfanityDbContext : DbContext
{
    public ProfanityDbContext(DbContextOptions<ProfanityDbContext> options)
        : base(options) {}

    public DbSet<Word> Words { get; set; }
}