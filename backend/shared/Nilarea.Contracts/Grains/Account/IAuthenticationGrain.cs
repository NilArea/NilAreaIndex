using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Exceptions;
using NilArea.Contracts.Responses.Account;

namespace NilArea.Contracts.Grains.Account;

/// <summary>
///     瞬态 身份验证 权限验证
/// </summary>
/// <exception cref="AuthenticationException">该Grain类异常均封装为该异常</exception>
public interface IAuthenticationGrain : IGrainWithIntegerKey
{
    /// <summary>
    ///     用户登录
    /// </summary>
    /// <param name="command">登录命令</param>
    /// <returns>登录响应，包含访问令牌和刷新令牌</returns>
    /// <exception cref="AuthenticationException">登录失败时抛出</exception>
    ValueTask<LoginAccountResponse> LoginAsync(LoginAccountCommand command);

    /// <summary>
    ///     刷新访问令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的登录响应，包含新的访问令牌和刷新令牌</returns>
    /// <exception cref="AuthenticationException">刷新令牌无效时抛出</exception>
    ValueTask<LoginAccountResponse> RefreshTokenAsync(long userId, string refreshToken);

    /// <summary>
    ///     验证访问令牌
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <returns>令牌是否有效</returns>
    ValueTask<bool> ValidateTokenAsync(string token);

    /// <summary>
    ///     用户登出
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>任务完成状态</returns>
    ValueTask LogoutAsync(long userId);
}