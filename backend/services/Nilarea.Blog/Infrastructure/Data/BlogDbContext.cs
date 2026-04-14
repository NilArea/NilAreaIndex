using Microsoft.EntityFrameworkCore;
using NilArea.Blog.Infrastructure.Data.Entities;
using NilArea.Blog.Infrastructure.Data.EntityConfigurations;

namespace NilArea.Blog.Infrastructure.Data;

public class
    BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
    public DbSet<BlogPost> BlogPosts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BlogPostConfiguration());
    }
}