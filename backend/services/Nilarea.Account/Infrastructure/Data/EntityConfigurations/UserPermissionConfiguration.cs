using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NilArea.Account.Infrastructure.Data.Entities;

namespace NilArea.Account.Infrastructure.Data.EntityConfigurations;

public class UserPermissionConfiguration :
    IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("UserPermission");
        builder.HasKey(e => new { e.UserId, e.PermissionId })
            .HasName("PK_UserPermission_UId_PID");
        /* ---------- 索引 ---------- */
        builder.HasIndex(ug => ug.UserId)
            .HasDatabaseName("IX_UserPermission_UserId");
        builder.HasIndex(ug => ug.PermissionId)
            .HasDatabaseName("IX_UserPermission_PermissionId");
        /* --------- 导航属性 --------- */
        builder.HasOne(ug => ug.User)
            .WithMany(u => u.Permissions)
            .HasForeignKey(ug => ug.UserId);
        builder.HasOne(ug => ug.Permission)
            .WithMany(g => g.UserPermissions)
            .HasForeignKey(ug => ug.PermissionId);
    }
}