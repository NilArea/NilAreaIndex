using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NilArea.Grains.Dtos;

public class AccountDbDto
{
    public required long UserId { get; init; }
    [MaxLength(100)] public required string Email { get; set; } = string.Empty;
    [MaxLength(255)] public required string PasswordSaltHash { get; set; } = string.Empty;
    [MaxLength(100)] public required string UserName { get; set; } = string.Empty;
    public required DateTime? CreatedAt { get; init; }
    public DateTime? DeleteAt { get; init; }
    public DateTime? UpdateAt { get; init; }
}

// 2. 统一配置
public class AccountUserEntityConfig : IEntityTypeConfiguration<AccountDbDto>
{
    public void Configure(EntityTypeBuilder<AccountDbDto> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("AccountUser");
        builder.HasKey(e => e.UserId)
            .HasName("PK_AccountUser_UserId");

        builder.Property(e => e.UserId)
            .ValueGeneratedNever(); // 雪花算法

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
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.UpdateAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(e => e.DeleteAt)
            .HasColumnType("datetime(6)");

        /* ---------- 索引 ---------- */
        // 唯一作用：保证未删除邮箱唯一 + 同时覆盖 Email / Email+DeleteAt 查询
        builder.HasIndex(e => new { e.Email, e.DeleteAt })
            .HasDatabaseName("IX_AccountUser_Email_DeleteAt")
            .IsUnique()
            .HasFilter($"{nameof(AccountDbDto.DeleteAt)} IS NULL");

        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AccountUser_CreatedAt");
    }
}
