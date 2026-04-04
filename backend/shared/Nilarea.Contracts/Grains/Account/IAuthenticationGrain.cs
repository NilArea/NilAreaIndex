using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Responses.Account;

namespace NilArea.Contracts.Grains.Account;

/// <summary>
///     瞬态 身份验证 权限验证
/// </summary>
/// <exception cref="AuthenticationException">该Grain类异常均封装为该异常</exception>

public interface IAuthenticationGrain : IGrainWithGuidKey
{
    ValueTask<LoginAccountResponse> LoginAsync(LoginAccountCommand command);

    ValueTask<bool> ValidateTokenAsync(string token);

    ValueTask<bool> VarifyPermissionAsync(Guid userId, ICollection<string> requiredPermissions);
}