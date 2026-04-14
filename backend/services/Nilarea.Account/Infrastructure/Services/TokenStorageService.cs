using Microsoft.Extensions.Logging;
using NilArea.Common.Services;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Account.Infrastructure.Services;

/// <summary>
///     令牌存储服务
/// </summary>
public interface ITokenStorageService
{
    /// <summary>
    ///     存储刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="expiry">过期时间</param>
    /// <returns>任务完成状态</returns>
    Task StoreRefreshTokenAsync(long userId, string refreshToken, DateTime expiry);

    /// <summary>
    ///     验证刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>刷新令牌是否有效</returns>
    Task<bool> ValidateRefreshTokenAsync(long userId, string refreshToken);

    /// <summary>
    ///     撤销刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>任务完成状态</returns>
    Task RevokeRefreshTokenAsync(long userId);

    /// <summary>
    ///     撤销所有令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>任务完成状态</returns>
    Task RevokeAllTokensAsync(long userId);
}

/// <summary>
///     令牌存储服务实现
/// </summary>
public class TokenStorageService(
    ILogger<TokenStorageService> logger,
    IRedisDatabase redisDatabase
) : ITokenStorageService, IAsyncLifetime
{
    private const string RefreshTokenPrefix = "refresh_token:";
    private const string UserTokenPrefix = "user_tokens:";

    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing token storage service...");
    }

    public async Task DisposeAsync()
    {
        logger.LogInformation("Disposing token storage service...");
    }

    /// <summary>
    ///     存储刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="expiry">过期时间</param>
    /// <returns>任务完成状态</returns>
    public async Task StoreRefreshTokenAsync(long userId, string refreshToken, DateTime expiry)
    {
        var tokenKey = $"{RefreshTokenPrefix}{refreshToken}";
        var userKey = $"{UserTokenPrefix}{userId}";
        var expiryTime = expiry - DateTime.UtcNow;

        // 存储刷新令牌，关联用户ID
        await redisDatabase.Database.StringSetAsync(tokenKey, userId.ToString(), expiryTime);

        // 存储用户的刷新令牌列表，用于快速查找和撤销
        await redisDatabase.Database.SetAddAsync(userKey, refreshToken);
        await redisDatabase.Database.KeyExpireAsync(userKey, expiryTime);
    }

    /// <summary>
    ///     验证刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>刷新令牌是否有效</returns>
    public async Task<bool> ValidateRefreshTokenAsync(long userId, string refreshToken)
    {
        var tokenKey = $"{RefreshTokenPrefix}{refreshToken}";
        var storedUserId = await redisDatabase.Database.StringGetAsync(tokenKey);

        return storedUserId.HasValue && storedUserId.ToString() == userId.ToString();
    }

    /// <summary>
    ///     撤销刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>任务完成状态</returns>
    public async Task RevokeRefreshTokenAsync(long userId)
    {
        var userKey = $"{UserTokenPrefix}{userId}";
        var refreshTokens = await redisDatabase.Database.SetMembersAsync(userKey);

        foreach (var token in refreshTokens)
        {
            var tokenKey = $"{RefreshTokenPrefix}{token}";
            await redisDatabase.Database.KeyDeleteAsync(tokenKey);
        }

        await redisDatabase.Database.KeyDeleteAsync(userKey);
    }

    /// <summary>
    ///     撤销所有令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>任务完成状态</returns>
    public async Task RevokeAllTokensAsync(long userId)
    {
        await RevokeRefreshTokenAsync(userId);
    }
}