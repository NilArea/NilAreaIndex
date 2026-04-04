using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Enums;
using NilArea.Contracts.Exceptions;
using NilArea.Contracts.Responses.Account;

namespace NilArea.Contracts.Grains.Account;

/// <summary>
///     瞬态 账号管理
/// </summary>
/// <exception cref="AccountException">该Grain类异常均封装为该异常</exception>
public interface IAccountGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     邮箱是否在使用
    /// </summary>
    /// <param name="email">邮箱</param>

    ValueTask<bool> ExistAccountAsync(string email);

    /// <summary>
    ///     通知发送验证码
    /// </summary>

    ValueTask CallConfirmKey(string email, ConfirmType typeCode = ConfirmType.Default);

    /// <summary>
    ///     注册账号
    /// </summary>

    ValueTask<RegisterAccountResponse> RegisterUserAsync(RegisterAccountCommand command);

    /// <summary>
    ///     删除账号
    /// </summary>

    ValueTask DeleteAccountAsync(DeleteAccountCommand command);

    /// <summary>
    ///     修改i密码
    /// </summary>

    ValueTask ChangePassword(ChangePasswordCommand command);
}