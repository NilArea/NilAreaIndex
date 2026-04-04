using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NilArea.Account.Infrastructure.Data.Entities;

namespace NilArea.Account.Infrastructure.Data.EntityConfigurations;

public class GroupPermissionConfiguration :
    IEntityTypeConfiguration<GroupPermission>
{
    public void Configure(EntityTypeBuilder<GroupPermission> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("GroupPermission");
        builder.HasKey(e => new { e.GroupId, e.PermissionId })
            .HasName("PK_GroupPermission_GId_PID");
        /* ---------- 索引 ---------- */
        builder.HasIndex(ug => ug.GroupId)
            .HasDatabaseName("IX_GroupPermission_GroupId");
        builder.HasIndex(ug => ug.PermissionId)
            .HasDatabaseName("IX_GroupPermission_PermissionId");
        /* --------- 导航属性 --------- */
        builder.HasOne(ug => ug.Group)
            .WithMany(u => u.Permissions)
            .HasForeignKey(ug => ug.GroupId);
        builder.HasOne(ug => ug.Permission)
            .WithMany(g => g.GroupPermissions)
            .HasForeignKey(ug => ug.PermissionId);
    }
}