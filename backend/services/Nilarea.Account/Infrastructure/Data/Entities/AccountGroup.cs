using System.ComponentModel.DataAnnotations;

namespace NilArea.Account.Infrastructure.Data.Entities;

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