using Microsoft.EntityFrameworkCore;
using ShardingCore.Sharding;

namespace NilArea.Blog.Infrastructure.Data;

public class
    BlogDbContext(DbContextOptions<BlogDbContext> options) : AbstractShardingDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}