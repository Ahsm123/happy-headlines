using Microsoft.EntityFrameworkCore;
using HappyHeadlines.CommentApi.Models;

namespace HappyHeadlines.CommentApi.Data;

public class CommentDbContext : DbContext
{
    public CommentDbContext(DbContextOptions<CommentDbContext> options)
        : base(options) {}
    
    public DbSet<Comment> Comments { get; set; }
}