using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Enums;
using NilArea.Contracts.Exceptions;
using NilArea.Contracts.Responses.Account;

namespace NilArea.Contracts.Grains.Account;

/// <summary>
///     瞬态 账号管理
/// </summary>
/// <exception cref="AccountException">该Grain类异常均封装为该异常</exception>
public interface IAccountGrain : IGrainWithIntegerKey
{
    /// <summary>
    ///     邮箱是否在使用
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>邮箱是否已被注册</returns>
    /// <exception cref="AccountException">邮箱格式无效时抛出</exception>
    ValueTask<bool> ExistAccountAsync(string email);

    /// <summary>
    ///     通知发送验证码
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="typeCode">验证码类型</param>
    /// <returns>任务完成状态</returns>
    /// <exception cref="AccountException">发送失败时抛出</exception>
    ValueTask CallConfirmKey(string email, ConfirmType typeCode = ConfirmType.Default);

    /// <summary>
    ///     注册账号
    /// </summary>
    /// <param name="command">注册命令</param>
    /// <returns>注册响应，包含用户信息</returns>
    /// <exception cref="AccountException">注册失败时抛出</exception>
    ValueTask<RegisterAccountResponse> RegisterUserAsync(RegisterAccountCommand command);

    /// <summary>
    ///     删除账号
    /// </summary>
    /// <param name="command">删除命令</param>
    /// <returns>任务完成状态</returns>
    /// <exception cref="AccountException">删除失败时抛出</exception>
    ValueTask DeleteAccountAsync(DeleteAccountCommand command);

    /// <summary>
    ///     修改密码
    /// </summary>
    /// <param name="command">修改密码命令</param>
    /// <returns>任务完成状态</returns>
    /// <exception cref="AccountException">修改失败时抛出</exception>
    ValueTask ChangePassword(ChangePasswordCommand command);

    /// <summary>
    ///     获取账号信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号信息响应</returns>
    /// <exception cref="AccountException">账号不存在时抛出</exception>
    ValueTask<AccountInfoResponse> GetAccountInfoAsync(Guid userId);

    /// <summary>
    ///     更新账号信息
    /// </summary>
    /// <param name="command">更新账号信息命令</param>
    /// <returns>更新后的账号信息响应</returns>
    /// <exception cref="AccountException">更新失败时抛出</exception>
    ValueTask<AccountInfoResponse> UpdateAccountInfoAsync(UpdateAccountInfoCommand command);

    /// <summary>
    ///     重置密码
    /// </summary>
    /// <param name="command">重置密码命令</param>
    /// <returns>任务完成状态</returns>
    /// <exception cref="AccountException">重置失败时抛出</exception>
    ValueTask ResetPasswordAsync(ResetPasswordCommand command);
}