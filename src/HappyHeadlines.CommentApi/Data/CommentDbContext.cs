using Microsoft.EntityFrameworkCore;
using HappyHeadlines.CommentApi.Models;

namespace HappyHeadlines.CommentApi.Data;

public class CommentDbContext(DbContextOptions<CommentDbContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; }
}
