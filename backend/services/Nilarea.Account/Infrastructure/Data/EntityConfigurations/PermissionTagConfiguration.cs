using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NilArea.Account.Infrastructure.Data.Entities;

namespace NilArea.Account.Infrastructure.Data.EntityConfigurations;

public class PermissionTagConfiguration :
    IEntityTypeConfiguration<PermissionTag>
{
    public void Configure(EntityTypeBuilder<PermissionTag> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("PermissionTag");
        builder.HasKey(e => e.PermissionId)
            .HasName("PK_PermissionTag_Id");

        builder.Property(e => e.PermissionId)
            .IsRequired()
            .ValueGeneratedOnAdd();
        /* ---------- 字段 ---------- */
        builder.Property(ug => ug.PermissionName)
            .IsRequired()
            .HasMaxLength(100);
        /* ---------- 索引 ---------- */
        builder.HasIndex(ug => ug.PermissionName)
            .IsUnique()
            .HasDatabaseName("IX_PermissionTag_Name");
        /* --------- 导航属性 --------- */
        builder.HasMany(pt => pt.UserPermissions)
            .WithOne(pt => pt.Permission)
            .HasForeignKey(pt => pt.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(pt => pt.GroupPermissions)
            .WithOne(pt => pt.Permission)
            .HasForeignKey(pt => pt.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}