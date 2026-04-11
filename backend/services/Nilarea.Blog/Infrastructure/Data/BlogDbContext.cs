using Microsoft.EntityFrameworkCore;

namespace NilArea.Blog.Infrastructure.Data;

public class
    BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}