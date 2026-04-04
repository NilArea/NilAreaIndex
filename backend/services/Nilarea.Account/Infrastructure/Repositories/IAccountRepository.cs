using NilArea.Account.DTOs;
using NilArea.Common.Services;
using NilArea.Contracts.Enums;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     账号相关内容数据库接口
/// </summary>
public interface IAccountRepository : IAsyncLifetime
{
    #region Other

    /// <summary>
    ///     根据用户ID获取所有权限
    /// </summary>
    ValueTask<PermissionTagInfo[]> GetAllPermissionAsync(Guid userId);

    #endregion

    #region Account

    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    ValueTask<bool> ExistsAccountAsync(Guid userId);

    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    ValueTask<bool> ExistsAccountAsync(string email);

    /// <summary>
    ///     根据用户ID查找账号
    /// </summary>
    ValueTask<AccountUserInfo?> FindAccountAsync(Guid userId);

    /// <summary>
    ///     根据邮箱查找账号
    /// </summary>
    ValueTask<AccountUserInfo?> FindAccountAsync(string email);

    /// <summary>
    ///     新建账号
    ///     <br />需求初始化验证码
    /// </summary>
    ValueTask<AccountUserInfo> InsertAccountAsync(string email, string passwordHash, string username);

    /// <summary>
    ///     更新账号信息
    ///     <br />需求可逆操作验证码
    /// </summary>
    ValueTask UpdateAccountAsync(object update);

    /// <summary>
    ///     修改账号密码
    ///     <br />需求可逆操作验证码
    /// </summary>
    ValueTask<bool> ChangePasswordAsync(Guid userId, string newPasswordHash);

    /// <summary>
    ///     修改账号邮箱
    ///     <br />需求可逆操作验证码
    /// </summary>
    ValueTask<bool> ChangeEmailAsync(Guid userId, string newEmail);

    /// <summary>
    ///     删除账号
    ///     <br />需求不可逆操作验证码
    /// </summary>
    ValueTask<bool> DeleteAccountAsync(Guid userId);

    #endregion

    #region Confirm

    /// <summary>
    ///     根据邮箱查找账号的验证密钥
    /// </summary>
    ValueTask<(Guid UserId, string PasswordSaltHash)> GetAccountVerifyAsync(string email);

    /// <summary>
    ///     缓存验证码
    ///     <br />验证码类型影响过期时间
    /// </summary>
    ValueTask<bool> CacheConfirmCodeAsync(string unique, string code, ConfirmType typeCode);

    /// <summary>
    ///     检查验证码是否存在
    /// </summary>
    ValueTask<bool> ExistConfirmCodeAsync(string unique);

    /// <summary>
    ///     检查验证码是否正确
    ///     <br />检查后的验证码会被删除
    /// </summary>
    ValueTask<bool> CheckConfirmCodeAsync(string unique, string code);

    #endregion
}