using NilArea.Contracts.Dto;
using NilArea.Interfaces.Exceptions;
using Orleans;

namespace NilArea.Interfaces.IGrains;

[Alias("NilArea.Interfaces.IGrains.IAccountGrain")]
public interface IAccountGrain : IGrainWithGuidKey
{
    /// <summary>
    ///     邮箱是否已被使用
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>是否已被使用</returns>
    /// <exception cref="ArgumentNullException">email is null</exception>
    [Alias("ExistEmailAsync")]
    ValueTask<bool> ExistEmailAsync(string email);

    /// <summary>
    ///     注册帐号
    /// </summary>
    /// <param name="request">注册传入信息</param>
    /// <returns>注册结果</returns>
    /// <exception cref="AccountException">注册失败</exception>
    [Alias("RegisterUserAsync")]
    ValueTask<RegisterResponse> RegisterUserAsync(RegisterRequest request);

    [Alias("LoginAsync")]
    ValueTask<LoginResponse> LoginAsync(LoginRequest request);
}
