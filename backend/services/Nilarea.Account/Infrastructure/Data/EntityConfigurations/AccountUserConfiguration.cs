using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NilArea.Account.Infrastructure.Data.Entities;

namespace NilArea.Account.Infrastructure.Data.EntityConfigurations;

public class AccountUserConfiguration : IEntityTypeConfiguration<AccountUser>
{
    public void Configure(EntityTypeBuilder<AccountUser> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("AccountUser");
        builder.HasKey(e => e.UserId)
            .HasName("PK_AccountUser_UserId");

        builder.Property(e => e.UserId)
            .IsRequired()
            .ValueGeneratedNever();

        /* ---------- 字段 ---------- */
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.PasswordSaltHash)
            .IsRequired()
            .HasMaxLength(255)
            .IsUnicode(false);

        builder.Property(e => e.UserName)
            .IsRequired()
            .HasMaxLength(100);

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
        builder.HasIndex(e => new { e.Email, e.DeleteAt })
            .HasDatabaseName("IX_AccountUser_Email")
            .IsUnique();

        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AccountUser_CreatedAt");

        builder.HasIndex(e => e.UpdateAt)
            .HasDatabaseName("IX_AccountUser_UpdateAt");
    }
}