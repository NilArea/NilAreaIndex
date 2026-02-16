using NilArea.Contracts.Dto;
using NilArea.Interfaces.Exceptions;

namespace NilArea.Interfaces.IGrains;

/// <summary>
///     瞬态 账号管理
/// </summary>
/// <exception cref="AccountException">该Grain类异常均封装为该异常</exception>
[Alias("NilArea.Interfaces.IGrains.IAccountGrain")]
public interface IAccountGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     邮箱是否在使用
    /// </summary>
    /// <param name="email">邮箱</param>
    [Alias("ExistAccountAsync")]
    ValueTask<bool> ExistAccountAsync(string email);

    /// <summary>
    ///     通知发送验证码
    /// </summary>
    [Alias("CallConfirmKey")]
    ValueTask CallConfirmKey(string email, ConfirmKey keyCode = ConfirmKey.Default);

    /// <summary>
    ///     注册账号
    /// </summary>
    [Alias("RegisterUserAsync")]
    ValueTask<AccountRegisterResponse> RegisterUserAsync(AccountRegisterRequest request);

    /// <summary>
    ///     删除账号
    /// </summary>
    [Alias("DeleteAccountAsync")]
    ValueTask DeleteAccountAsync(DeleteAccountRequest request);

    /// <summary>
    ///     修改i密码
    /// </summary>
    [Alias("ChangePasswd")]
    ValueTask ChangePasswd(ChangePasswdRequest request);
}

public enum ConfirmKey
{
    Default,
    Register,
    ChangePasswd,
    DeleteAccount
}
