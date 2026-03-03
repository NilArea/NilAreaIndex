using Microsoft.EntityFrameworkCore;
using Nilarea.Database.Dbe;
using ShardingCore.Sharding;

namespace Nilarea.Database;

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
        var ec = new AccountUserEntityConfig();
        modelBuilder
            .ApplyConfiguration<AccountUser>(ec)
            .ApplyConfiguration<AccountGroup>(ec)
            .ApplyConfiguration<AccountUserGroup>(ec)
            .ApplyConfiguration<PermissionTag>(ec)
            .ApplyConfiguration<UserPermission>(ec)
            .ApplyConfiguration<GroupPermission>(ec);
    }
}
