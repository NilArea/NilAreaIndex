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
    ValueTask CallConfirmKey(string email, ConfirmType typeCode = ConfirmType.Default);

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
    ValueTask ChangePassword(ChangePasswdRequest request);
}

/// <summary>
///     验证码作用类型<br />
///     <see cref="Default" />默认类型或其他类型<br />
///     <see cref="Initial" />初始化操作<br />
///     <see cref="Reversible" />可逆操作<br />
///     <see cref="Irreversible" />不可逆操作<br />
/// </summary>
public enum ConfirmType
{
    Default,
    Initial,
    Reversible,
    Irreversible
}
