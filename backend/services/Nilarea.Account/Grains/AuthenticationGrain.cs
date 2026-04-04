using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Account.Infrastructure.Repositories;
using NilArea.Common.Utils;
using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Enums;
using NilArea.Contracts.Exceptions;
using NilArea.Contracts.Grains.Account;
using NilArea.Contracts.Responses.Account;
using Orleans.Concurrency;

namespace NilArea.Account.Grains;

[Reentrant]
[StatelessWorker]
public sealed class AuthenticationGrain(
    ILogger<AuthenticationGrain> logger,
    IAccountRepository accountRepository,
    IValidator<LoginAccountCommand> loginRequestValidator,
    IPasswordHasher passwordHasher
) : Grain, IAuthenticationGrain
{
    public async ValueTask<LoginAccountResponse> LoginAsync(LoginAccountCommand command)
    {
        var validate = await loginRequestValidator.ValidateAsync(command);
        if (!validate.IsValid)
            throw new AuthenticationException(validate.ToString(), AuthenticationResult.Failed);
        if (!await accountRepository.ExistsAccountAsync(command.Email))
            throw new AuthenticationException("Email is not registered", AuthenticationResult.Failed);
        var add = await accountRepository
            .GetAccountVerifyAsync(command.Email);
        if (!passwordHasher.Verify(command.Password, add.PasswordSaltHash))
            throw new AuthenticationException("Password does not match", AuthenticationResult.Failed);
        return new LoginAccountResponse
        {
            UserId = add.UserId,
            AccessToken = string.Empty,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = string.Empty,
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
        };
    }

    public async ValueTask<bool> ValidateTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> VarifyPermissionAsync(Guid userId, ICollection<string> requiredPermissions)
    {
        if (requiredPermissions.Count is 0) return true;
        var permissions = await accountRepository.GetAllPermissionAsync(userId);
        return !requiredPermissions.Except(permissions.Select(p => p.PermissionName)).Any();
    }
}