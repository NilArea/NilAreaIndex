using Microsoft.EntityFrameworkCore;
using NilArea.Grains.Dtos;
using ShardingCore.Sharding;

namespace NilArea.Grains.DbContext;

public class
    NilDbContext(DbContextOptions<NilDbContext> options) : AbstractShardingDbContext(options)
{
    public DbSet<AccountUserDto> AccountUsers { get; set; }
    public DbSet<AccountGroupDto> AccountGroups { get; set; }
    public DbSet<AccountUserGroup> AccountUserGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyNilareaContractsConfig();
    }
}
