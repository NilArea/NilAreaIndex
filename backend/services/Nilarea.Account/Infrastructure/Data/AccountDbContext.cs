using Microsoft.EntityFrameworkCore;
using NilArea.Account.Infrastructure.Data.Entities;
using NilArea.Account.Infrastructure.Data.EntityConfigurations;
using ShardingCore.Sharding;

namespace NilArea.Account.Infrastructure.Data;

public class
    AccountDbContext(DbContextOptions<AccountDbContext> options) : AbstractShardingDbContext(options)
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
        modelBuilder
            .ApplyConfiguration(new AccountUserConfiguration())
            .ApplyConfiguration(new AccountGroupConfiguration())
            .ApplyConfiguration(new AccountUserGroupConfiguration())
            .ApplyConfiguration(new PermissionTagConfiguration())
            .ApplyConfiguration(new UserPermissionConfiguration())
            .ApplyConfiguration(new GroupPermissionConfiguration());
    }
}