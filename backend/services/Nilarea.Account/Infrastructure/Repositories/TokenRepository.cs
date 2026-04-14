using Microsoft.Extensions.Logging;
using NilArea.Common.Services;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     令牌存储服务实现
/// </summary>
public class TokenRepository(
    ILogger<TokenRepository> logger,
    IRedisDatabase redisDatabase
) : ITokenRepository, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing token storage service...");
    }

    public async Task DisposeAsync()
    {
        logger.LogInformation("Disposing token storage service...");
    }

    public async Task StoreRefreshTokenAsync(long userId, string refreshToken, DateTimeOffset refreshTokenExpiry)
    {
        const string luaScript =
            """
            local set_key = KEYS[1]
            local token_key = KEYS[2]
            local token = ARGV[1]
            local expire_sec = tonumber(ARGV[2])

            redis.call('SADD', set_key, token)
            redis.call('SETEX', token_key, expire_sec, '1')
            """;
        var keys = new[] { UserTokenKey(userId), RefreshTokenKey(userId, refreshToken) };
        var args = new RedisValue[] { refreshToken, (int)(refreshTokenExpiry - DateTimeOffset.UtcNow).TotalSeconds };
        await redisDatabase.Database.ScriptEvaluateAsync(luaScript, keys, args);
    }

    public async Task<bool> ValidateRefreshTokenAsync(long userId, string refreshToken, bool revoke = false)
    {
        var keys = new[] { UserTokenKey(userId), RefreshTokenKey(userId, refreshToken) };
        const string revokeScript =
            """
            local set_key = KEYS[1]
            local token_key = KEYS[2]
            local token = ARGV[1]

            if (redis.call('GET', token_key) == nil) then
                redis.call('SREM', set_key, token)
                return false
            end
            redis.call('DEL', token_key)
            redis.call('SREM', set_key, token)
            return true
            """;
        const string validateScript =
            """
            local set_key = KEYS[1]
            local token_key = KEYS[2]
            local token = ARGV[1]

            if (redis.call('GET', token_key) == nil) then
                redis.call('SREM', set_key, token)
                return false
            end
            return true
            """;
        return (bool)await redisDatabase.Database.ScriptEvaluateAsync(revoke ? revokeScript : validateScript, keys,
            [refreshToken]);
    }

    public async Task RevokeRefreshTokenAsync(long userId, string refreshToken)
    {
        var keys = new[] { UserTokenKey(userId), RefreshTokenKey(userId, refreshToken) };
        const string luaScript =
            """
            local set_key = KEYS[1]
            local token_key = KEYS[2]
            local token = ARGV[1]

            redis.call('DEL', token_key)
            redis.call('SREM', set_key, token)
            return true
            """;
        await redisDatabase.Database.ScriptEvaluateAsync(luaScript, keys, [refreshToken]);
    }

    public async Task RevokeAllTokensAsync(long userId)
    {
        const string luaScript =
            """
            local set_key = KEYS[1]
            local prefix = KEYS[2]

            local members = redis.call('SMEMBERS', set_key)
            for _, token in ipairs(members) do
                redis.call('DEL', prefix .. token)
            end
            redis.call('DEL', set_key)
            """;
        await redisDatabase.Database.ScriptEvaluateAsync(luaScript,
            [UserTokenKey(userId), RefreshTokenKey(userId, string.Empty)]);
    }

    private static RedisKey RefreshTokenKey(long userId, string refreshToken)
    {
        return $"refresh:{{{userId}}}:{refreshToken}";
    }

    private static RedisKey UserTokenKey(long userId)
    {
        return $"refresh:{{{userId}}}";
    }
}