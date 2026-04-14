using NilArea.Account.DTOs;
using NilArea.Contracts.Exceptions;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     账号管理数据库接口
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号是否存在</returns>
    ValueTask<bool> ExistsAccountAsync(long userId);

    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>账号是否存在</returns>
    ValueTask<bool> ExistsAccountAsync(string email);

    /// <summary>
    ///     根据用户ID获取账号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号信息</returns>
    ValueTask<AccountUserInfo?> GetAccountAsync(long userId);

    /// <summary>
    ///     根据邮箱获取账号
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>账号信息</returns>
    ValueTask<AccountUserInfo?> GetAccountByEmailAsync(string email);

    /// <summary>
    ///     新建账号
    ///     <br />需求初始化验证码
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="passwordHash">密码哈希</param>
    /// <param name="username">用户名</param>
    /// <returns>新建的账号信息</returns>
    /// <exception cref="AccountException">邮箱已注册时抛出</exception>
    ValueTask<AccountUserInfo> InsertAccountAsync(string email, string passwordHash, string username);

    /// <summary>
    ///     修改账号密码
    ///     <br />需求可逆操作验证码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="newPasswordHash">新密码哈希</param>
    /// <returns>是否修改成功</returns>
    /// <exception cref="AccountException">账号不存在时抛出</exception>
    ValueTask<AccountUserInfo> ChangePasswordAsync(long userId, string newPasswordHash);

    /// <summary>
    ///     修改账号邮箱
    ///     <br />需求可逆操作验证码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="newEmail">新邮箱</param>
    /// <returns>是否修改成功</returns>
    ValueTask<AccountUserInfo> ChangeEmailAsync(long userId, string newEmail);

    /// <summary>
    ///     删除账号
    ///     <br />需求不可逆操作验证码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否删除成功</returns>
    ValueTask DeleteAccountAsync(long userId);
}