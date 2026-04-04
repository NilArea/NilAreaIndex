using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NilArea.Account.Infrastructure.Data.Entities;

namespace NilArea.Account.Infrastructure.Data.EntityConfigurations;

public class AccountUserGroupConfiguration :
    IEntityTypeConfiguration<AccountUserGroup>
{
    public void Configure(EntityTypeBuilder<AccountUserGroup> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("UserGroups");
        builder.HasKey(e => new { e.UserId, e.GroupId })
            .HasName("PK_UserGroup_UID_GID");

        /* ---------- 字段 ---------- */
        builder.Property(ug => ug.JoinedAt)
            .HasColumnType("datetime(6)")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAdd();
        /* ---------- 索引 ---------- */
        builder.HasIndex(ug => ug.GroupId)
            .HasDatabaseName("IX_UserGroup_GroupId");
        builder.HasIndex(ug => ug.UserId)
            .HasDatabaseName("IX_UserGroup_UserId");
        builder.HasIndex(ug => ug.JoinedAt)
            .HasDatabaseName("IX_UserGroup_JoinedAt");
        /* --------- 导航属性 --------- */
        builder.HasOne(ug => ug.User)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(ug => ug.UserId);
        builder.HasOne(ug => ug.Group)
            .WithMany(g => g.UserGroups)
            .HasForeignKey(ug => ug.GroupId);
    }
}