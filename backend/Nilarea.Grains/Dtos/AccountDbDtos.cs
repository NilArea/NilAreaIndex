using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NilArea.Grains.Dtos;

public class AccountUser
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
    public DateTime UpdateAt { get; set; }

    /// <summary>
    ///     用户最近登录时间
    /// </summary>
    [DataType("datetime(6)")]
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    ///     用户所属组
    /// </summary>
    public virtual ICollection<AccountUserGroup> UserGroups { get; } = [];

    /// <summary>
    ///     用户权限
    /// </summary>
    public virtual ICollection<UserPermission> Permissions { get; } = [];
}

public class AccountGroup
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

    public bool IsSystemGroup { get; init; }

    /// <summary>
    ///     组创建时间,系统组为0
    /// </summary>
    [DataType("datetime(6)")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     组删除时间,系统组和未删除为null
    /// </summary>
    [DataType("datetime(6)")]
    public DateTime? DeleteAt { get; set; }

    /// <summary>
    ///     组内容更新时间
    /// </summary>
    [DataType("datetime(6)")]
    public DateTime UpdateAt { get; set; }

    /// <summary>
    ///     组用户
    /// </summary>
    public virtual ICollection<AccountUserGroup> UserGroups { get; } = [];

    /// <summary>
    ///     组权限
    /// </summary>
    public virtual ICollection<GroupPermission> Permissions { get; } = [];
}

public class AccountUserGroup
{
    public required long UserId { get; init; }
    public required int GroupId { get; init; }
    [DataType("datetime(6)")] public DateTime JoinedAt { get; init; }

    // 导航属性
    public virtual AccountUser User { get; set; } = null!;
    public virtual AccountGroup Group { get; set; } = null!;
}

public class PermissionTag
{
    public required short PermissionId { get; init; }
    [MaxLength(100)] public required string PermissionName { get; init; }
    public virtual ICollection<UserPermission> UserPermissions { get; } = [];
    public virtual ICollection<GroupPermission> GroupPermissions { get; } = [];
}

public class UserPermission
{
    public required long UserId { get; init; }
    public required short PermissionId { get; init; }
    public virtual AccountUser User { get; set; } = null!;
    public virtual PermissionTag Permission { get; set; } = null!;
}

public class GroupPermission
{
    public required int GroupId { get; init; }
    public required short PermissionId { get; init; }
    public virtual AccountGroup Group { get; set; } = null!;
    public virtual PermissionTag Permission { get; set; } = null!;
}

public class AccountUserEntityConfig :
    IEntityTypeConfiguration<AccountUser>,
    IEntityTypeConfiguration<AccountGroup>,
    IEntityTypeConfiguration<AccountUserGroup>,
    IEntityTypeConfiguration<PermissionTag>,
    IEntityTypeConfiguration<UserPermission>,
    IEntityTypeConfiguration<GroupPermission>
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

    public void Configure(EntityTypeBuilder<AccountUser> builder)
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

        builder.Property(e => e.LastLoginAt)
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        /* ---------- 索引 ---------- */
        builder.HasIndex(e => e.Email)
            .HasDatabaseName("IX_AccountUser_AllEmail")
            .HasFilter($"{nameof(AccountUser.DeleteAt)} IS NULL")
            .IsUnique();

        // 时间排序/分页
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AccountUser_CreatedAt")
            .HasFilter($"{nameof(AccountUser.DeleteAt)} IS NULL");

        builder.HasIndex(e => e.UpdateAt)
            .HasDatabaseName("IX_AccountUser_UpdateAt")
            .HasFilter($"{nameof(AccountUser.DeleteAt)} IS NULL");

        builder.HasIndex(e => e.LastLoginAt)
            .HasDatabaseName("IX_AccountUser_LastLoginAt")
            .HasFilter($"{nameof(AccountUser.DeleteAt)} IS NULL");

        /* --------- 导航属性 --------- */
        builder.HasMany(u => u.UserGroups)
            .WithOne(ug => ug.User)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Permissions)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId)
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
