using System.ComponentModel.DataAnnotations;

namespace NilArea.Account.Infrastructure.Data.Entities;

public class AccountUser
{
    /// <summary>
    ///     用户唯一ID
    /// </summary>
    public required Guid UserId { get; init; }

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