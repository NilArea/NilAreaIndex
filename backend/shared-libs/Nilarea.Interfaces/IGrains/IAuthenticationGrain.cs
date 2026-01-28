using NilArea.Contracts.Dto;
using NilArea.Interfaces.Exceptions;

namespace NilArea.Interfaces.IGrains;

/// <summary>
///     瞬态 身份验证 权限验证
/// </summary>
/// <exception cref="AuthenticationException">该Grain类异常均封装为该异常</exception>
[Alias("NilArea.Interfaces.IGrains.IAuthenticationGrain")]
public interface IAuthenticationGrain : IGrainWithGuidKey
{
    [Alias("LoginAsync")]
    ValueTask<Responses.Login> LoginAsync(Requests.LoginAccount request);

    [Alias("ValidateTokenAsync")]
    ValueTask<bool> ValidateTokenAsync(string token);
}
