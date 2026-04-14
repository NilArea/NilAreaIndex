using System.ComponentModel.DataAnnotations;

namespace NilArea.Account.Infrastructure.Data.Entities;

public sealed class AccountUser
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
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    ///     用户删除时间,null表示未删除
    /// </summary>
    [DataType("datetime(6)")]
    public DateTimeOffset? DeleteAt { get; set; }

    /// <summary>
    ///     用户信息更新时间
    /// </summary>
    [DataType("datetime(6)")]
    public DateTimeOffset UpdateAt { get; set; }
}