using Microsoft.EntityFrameworkCore;
using NilArea.Grains.Dtos;
using ShardingCore.Sharding;

namespace NilArea.Grains.DbContext;

public class
    NilDbContext(DbContextOptions<NilDbContext> options) : AbstractShardingDbContext(options)
{
    public DbSet<AccountDbDto> AccountUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyNilareaContractsConfig();
    }
}
