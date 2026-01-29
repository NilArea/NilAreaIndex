using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Common.Utils;
using NilArea.Contracts.Dto;
using NilArea.Grains.Services;
using NilArea.Interfaces.Exceptions;
using NilArea.Interfaces.IGrains;
using Orleans.Concurrency;

namespace NilArea.Grains.ImplGrains;

[Reentrant]
[StatelessWorker]
public sealed class AuthenticationGrain(
    ILogger<AuthenticationGrain> logger,
    IAccountRepository accountRepository,
    IValidator<Requests.LoginAccount> loginRequestValidator,
    IPasswordHasher passwordHasher
) : Grain, IAuthenticationGrain
{
    public async ValueTask<Responses.Login> LoginAsync(Requests.LoginAccount request)
    {
        var validate = await loginRequestValidator.ValidateAsync(request);
        if (!validate.IsValid)
            throw new AuthenticationException(validate.ToString(), AuthenticationResult.Failed);
        if (!await accountRepository.ExistsAccountAsync(request.Email))
            throw new AuthenticationException("Email is not registered", AuthenticationResult.Failed);
        var add = await accountRepository
            .FindAccountAsync(request.Email, au => new { au.UserId, au.PasswordSaltHash });
        if (!passwordHasher.Verify(request.Password, add.PasswordSaltHash))
            throw new AuthenticationException("Password does not match", AuthenticationResult.Failed);
        var token = await accountRepository.GenerateTokenAsync(add.UserId);
        return new Responses.Login
        {
            UserId = add.UserId,
            AccessToken = token.AccessToken,
            AccessTokenExpiry = token.AccessExpire,
            RefreshToken = token.RefreshToken,
            RefreshTokenExpiry = token.RefreshExpire
        };
    }

    public async ValueTask<bool> ValidateTokenAsync(string token)
    {
        throw new NotImplementedException();
    }
}
