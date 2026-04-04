using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NilArea.Account.Infrastructure.Data.Entities;

namespace NilArea.Account.Infrastructure.Data.EntityConfigurations;

public class AccountGroupConfiguration :
    IEntityTypeConfiguration<AccountGroup>
{
    public void Configure(EntityTypeBuilder<AccountGroup> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("AccountGroup");
        builder.HasKey(e => e.GroupId)
            .HasName("PK_AccountGroup_GroupId");

        builder.Property(e => e.GroupId)
            .IsRequired()
            .ValueGeneratedOnAdd();

        /* ---------- 字段 ---------- */
        builder.Property(e => e.GroupName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.IsSystemGroup)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime(6)")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UpdateAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(e => e.DeleteAt)
            .HasColumnType("datetime(6)")
            .ValueGeneratedNever();
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => e.GroupName)
            .HasDatabaseName("IX_AccountGroup_AllGroup")
            .HasFilter($"{nameof(AccountGroup.DeleteAt)} IS NULL")
            .IsUnique();

        builder.HasIndex(e => e.GroupName)
            .HasDatabaseName("IX_AccountGroup_SystemGroup")
            .HasFilter($"{nameof(AccountGroup.IsSystemGroup)} = TRUE")
            .IsUnique();

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AccountGroup_CreatedAt")
            .HasFilter($"{nameof(AccountGroup.DeleteAt)} IS NULL");
        /* --------- 导航属性 --------- */
        builder.HasMany(g => g.UserGroups)
            .WithOne(ug => ug.Group)
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Permissions)
            .WithOne(up => up.Group)
            .HasForeignKey(up => up.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}