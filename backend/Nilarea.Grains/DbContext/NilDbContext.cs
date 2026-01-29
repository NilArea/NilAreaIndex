using Microsoft.EntityFrameworkCore;
using NilArea.Grains.Dtos;
using ShardingCore.Sharding;

namespace NilArea.Grains.DbContext;

public class
    NilDbContext(DbContextOptions<NilDbContext> options) : AbstractShardingDbContext(options)
{
    public DbSet<AccountUser> AccountUsers { get; set; }
    public DbSet<AccountGroup> AccountGroups { get; set; }
    public DbSet<AccountUserGroup> AccountUserGroups { get; set; }
    public DbSet<PermissionTag> PermissionTags { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<GroupPermission> GroupPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyNilareaContractsConfig();
    }
}
