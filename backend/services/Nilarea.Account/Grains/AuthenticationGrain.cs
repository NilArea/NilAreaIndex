using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Account.Infrastructure.Repositories;
using NilArea.Account.Infrastructure.Services;
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
    IConfirmRepository confirmRepository,
    IValidator<LoginAccountCommand> loginRequestValidator,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ITokenStorageService tokenStorageService
) : Grain, IAuthenticationGrain
{
    public async ValueTask<LoginAccountResponse> LoginAsync(LoginAccountCommand command)
    {
        logger.LogDebug("Login attempt for email: {Email}", command.Email);

        var validate = await loginRequestValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Login validation failed for email: {Email}, errors: {Errors}", command.Email,
                validate.ToString());
            throw new AuthenticationException(validate.ToString(), AuthenticationResult.Failed);
        }

        if (!await accountRepository.ExistsAccountAsync(command.Email))
        {
            logger.LogWarning("Login failed: Email not registered: {Email}", command.Email);
            throw new AuthenticationException("Email is not registered", AuthenticationResult.Failed);
        }

        var add = await confirmRepository.GetAccountVerifyAsync(command.Email);
        if (!passwordHasher.Verify(command.Password, add.PasswordSaltHash))
        {
            logger.LogWarning("Login failed: Password does not match for email: {Email}", command.Email);
            throw new AuthenticationException("Password does not match", AuthenticationResult.Failed);
        }

        // 生成访问令牌和刷新令牌
        var accessToken = tokenService.GenerateAccessToken(add.UserId, command.Email);
        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // 存储刷新令牌
        await tokenStorageService.StoreRefreshTokenAsync(add.UserId, refreshToken, refreshTokenExpiry);

        logger.LogDebug("Login successful for email: {Email}, UserId: {UserId}", command.Email, add.UserId);

        return new LoginAccountResponse
        {
            UserId = add.UserId,
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshTokenExpiry
        };
    }

    public async ValueTask<bool> ValidateTokenAsync(string token)
    {
        logger.LogDebug("Token validation attempt");

        try
        {
            tokenService.ValidateToken(token);
            logger.LogDebug("Token validation successful");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogDebug("Token validation failed: {Exception}", ex.Message);
            return false;
        }
    }

    public async ValueTask<LoginAccountResponse> RefreshTokenAsync(long userId, string refreshToken)
    {
        logger.LogDebug("Refresh token attempt for UserId: {UserId}", userId);

        // 验证刷新令牌
        if (!await tokenStorageService.ValidateRefreshTokenAsync(userId, refreshToken))
        {
            logger.LogWarning("Refresh token validation failed for UserId: {UserId}", userId);
            throw new AuthenticationException("Invalid refresh token", AuthenticationResult.Failed);
        }

        // 获取用户信息
        var account = await accountRepository.GetAccountAsync(userId);
        if (account == null)
        {
            logger.LogWarning("User not found for UserId: {UserId}", userId);
            throw new AuthenticationException("User not found", AuthenticationResult.Failed);
        }

        // 生成新的访问令牌和刷新令牌
        var accessToken = tokenService.GenerateAccessToken(userId, account.Email);
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // 存储新的刷新令牌
        await tokenStorageService.StoreRefreshTokenAsync(userId, newRefreshToken, refreshTokenExpiry);

        logger.LogDebug("Refresh token successful for UserId: {UserId}, Email: {Email}", userId, account.Email);

        return new LoginAccountResponse
        {
            UserId = userId,
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = newRefreshToken,
            RefreshTokenExpiry = refreshTokenExpiry
        };
    }

    public async ValueTask LogoutAsync(long userId)
    {
        logger.LogDebug("Logout attempt for UserId: {UserId}", userId);

        // 撤销用户的所有令牌
        await tokenStorageService.RevokeAllTokensAsync(userId);

        logger.LogDebug("Logout successful for UserId: {UserId}", userId);
    }
}