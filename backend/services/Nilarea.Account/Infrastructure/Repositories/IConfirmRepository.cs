using NilArea.Contracts.Enums;
using NilArea.Contracts.Exceptions;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     验证码管理数据库接口
/// </summary>
public interface IConfirmRepository
{
    /// <summary>
    ///     根据邮箱查找账号的验证密钥
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>用户ID和密码哈希</returns>
    /// <exception cref="AuthenticationException">邮箱未注册时抛出</exception>
    ValueTask<(long UserId, string PasswordSaltHash)> GetAccountVerifyAsync(string email);

    /// <summary>
    ///     缓存验证码
    ///     <br />验证码类型影响过期时间
    /// </summary>
    /// <param name="unique">唯一标识（通常是邮箱）</param>
    /// <param name="code">验证码</param>
    /// <param name="typeCode">验证码类型</param>
    /// <returns>是否缓存成功</returns>
    ValueTask<bool> CacheConfirmCodeAsync(string unique, string code, ConfirmType typeCode);

    /// <summary>
    ///     检查验证码是否存在
    /// </summary>
    /// <param name="unique">唯一标识（通常是邮箱）</param>
    /// <returns>验证码是否存在</returns>
    ValueTask<bool> ExistConfirmCodeAsync(string unique);

    /// <summary>
    ///     检查验证码是否正确
    ///     <br />检查后的验证码会被删除
    /// </summary>
    /// <param name="unique">唯一标识（通常是邮箱）</param>
    /// <param name="code">验证码</param>
    /// <returns>验证码是否正确</returns>
    /// <exception cref="AccountException">验证码已过期或不正确时抛出</exception>
    ValueTask<bool> CheckConfirmCodeAsync(string unique, string code);
}