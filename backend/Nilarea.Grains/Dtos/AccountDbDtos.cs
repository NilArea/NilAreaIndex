using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NilArea.Grains.Dtos;

public class AccountUserDto
{
    /// <summary>
    ///     用户唯一ID
    /// </summary>
    public required long UserId { get; init; }

    /// <summary>
    ///     用户绑定邮箱
    /// </summary>
    [MaxLength(100)]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    ///     加盐密码
    /// </summary>
    [MaxLength(255)]
    public required string PasswordSaltHash { get; set; }

    /// <summary>
    ///     用户昵称
    /// </summary>
    [MaxLength(100)]
    public required string UserName { get; set; }

    /// <summary>
    ///     用户创建时间
    /// </summary>
    [DataType("datetime(6)")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     用户删除时间,null表示未删除
    /// </summary>
    [DataType("datetime(6)")]
    public DateTime? DeleteAt { get; set; }

    /// <summary>
    ///     用户信息更新时间
    /// </summary>
    [DataType("datetime(6)")]
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    ///     用户是否邮箱验证,未完成与游客相同,完成才为用户
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    ///     用户最近登录时间
    /// </summary>
    [DataType("datetime(6)")]
    public DateTime LastLoginAt { get; set; }

    /// <summary>
    ///     用户所属组
    /// </summary>
    public virtual ICollection<AccountUserGroup> UserGroups { get; } = [];
}

public class AccountGroupDto
{
    /// <summary>
    ///     组唯一ID
    /// </summary>
    public required int GroupId { get; init; }

    /// <summary>
    ///     组名
    /// </summary>

    [MaxLength(100)]
    public required string GroupName { get; init; } = string.Empty;

    /// <summary>
    ///     组描述
    /// </summary>

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    ///     组创建时间,系统组为0
    /// </summary>
    [DataType("datetime(6)")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     组删除时间,系统组和未删除为null
    /// </summary>
    [DataType("datetime(6)")]
    public DateTime? DeleteAt { get; init; }

    /// <summary>
    ///     组内容更新时间
    /// </summary>
    [DataType("datetime(6)")]
    public DateTime UpdateAt { get; init; }

    public virtual ICollection<AccountUserGroup> UserGroups { get; } = [];
}

public class AccountUserGroup
{
    public required long UserId { get; init; }
    public required int GroupId { get; init; }
    [DataType("datetime(6)")] public DateTime JoinedAt { get; init; }

    // 导航属性
    public virtual AccountUserDto User { get; set; } = null!;
    public virtual AccountGroupDto Group { get; set; } = null!;
}

public class AccountUserEntityConfig :
    IEntityTypeConfiguration<AccountUserDto>,
    IEntityTypeConfiguration<AccountGroupDto>,
    IEntityTypeConfiguration<AccountUserGroup>
{
    public void Configure(EntityTypeBuilder<AccountGroupDto> builder)
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
        builder.HasIndex(e => new { e.GroupName, e.DeleteAt })
            .HasDatabaseName("IX_AccountGroup_DeleteAt")
            .IsUnique()
            .HasFilter($"{nameof(AccountGroupDto.DeleteAt)} IS NULL");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AccountUser_CreatedAt");
        /* --------- 导航属性 --------- */
        builder.HasMany(g => g.UserGroups)
            .WithOne(ug => ug.Group)
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<AccountUserDto> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("AccountUser");
        builder.HasKey(e => e.UserId)
            .HasName("PK_AccountUser_UserId");

        builder.Property(e => e.UserId)
            .IsRequired()
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
            .HasFilter($"{nameof(AccountUserDto.DeleteAt)} IS NULL");

        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AccountUser_CreatedAt");

        /* --------- 导航属性 --------- */
        builder.HasMany(u => u.UserGroups)
            .WithOne(ug => ug.User)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<AccountUserGroup> builder)
    {
        /* ---------- 表 & 主键 ---------- */
        builder.ToTable("UserGroups");
        builder.HasKey(e => new { e.UserId, e.GroupId })
            .HasName("PK_UserGroup_UID_GID");

        /* ---------- 字段 ---------- */
        builder.Property(ug => ug.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
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
