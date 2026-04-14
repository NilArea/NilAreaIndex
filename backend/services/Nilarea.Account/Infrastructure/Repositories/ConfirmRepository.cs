using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NilArea.Account.Infrastructure.Data;
using NilArea.Contracts.Enums;
using NilArea.Contracts.Exceptions;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     验证码管理数据库实现
/// </summary>
public sealed class ConfirmRepository(
    ILogger<ConfirmRepository> logger,
    IDbContextFactory<AccountDbContext> dbContextFactory,
    IRedisClientFactory redisClientFactory
) : IConfirmRepository
{
    /// <summary>
    ///     根据邮箱查找账号的验证密钥
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>用户ID和密码哈希</returns>
    /// <exception cref="AuthenticationException">邮箱未注册时抛出</exception>
    public async ValueTask<(long UserId, string PasswordSaltHash)> GetAccountVerifyAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var up = await dbContext.AccountUsers.AsNoTracking()
                     .Where(au => au.Email == email && au.DeleteAt == null)
                     .Select(au => new { au.UserId, au.PasswordSaltHash })
                     .FirstOrDefaultAsync() ??
                 throw new AuthenticationException("Email does not registered", AuthenticationResult.Unauthorized);
        return (up.UserId, up.PasswordSaltHash);
    }

    /// <summary>
    ///     缓存验证码
    /// </summary>
    /// <param name="unique">唯一标识（通常是邮箱）</param>
    /// <param name="code">验证码</param>
    /// <param name="typeCode">验证码类型</param>
    public async ValueTask CacheConfirmCodeAsync(string unique, string code, ConfirmType typeCode)
    {
        const string luaScript =
            """
            local current = redis.call('GET', KEYS[1])
            if current == ARGV[1] then
                return 'exist'
            else
                redis.call('SET', KEYS[1], ARGV[1], 'EX', ARGV[2])
                return 'ok'
            end
            """;
        var keys = new[] { GetConfirmKey(unique) };
        var args = new RedisValue[] { code, GetExpirationTime().TotalSeconds };
        var result = await redisClientFactory.GetDefaultRedisDatabase().Database
            .ScriptEvaluateAsync(luaScript, keys, args);
        switch (result.ToString())
        {
            case "ok":
                return;
            case "exist":
                throw new ConfirmException("Confirm code already exists");
            default:
                throw new ConfirmException("Cache confirm code failed");
        }

        TimeSpan GetExpirationTime()
        {
            return typeCode switch
            {
                ConfirmType.Initial => TimeSpan.FromMinutes(30),
                ConfirmType.Reversible => TimeSpan.FromMinutes(15),
                ConfirmType.Irreversible => TimeSpan.FromMinutes(5),
                _ => TimeSpan.FromMinutes(10)
            };
        }
    }

    /// <summary>
    ///     检查验证码是否存在
    /// </summary>
    /// <param name="unique">唯一标识（通常是邮箱）</param>
    /// <returns>验证码是否存在</returns>
    public async ValueTask<bool> ExistConfirmCodeAsync(string unique)
    {
        var rk = GetConfirmKey(unique);
        return await redisClientFactory.GetDefaultRedisDatabase().Database.KeyExistsAsync(rk);
    }

    /// <summary>
    ///     检查验证码是否正确
    /// </summary>
    /// <param name="unique">唯一标识（通常是邮箱）</param>
    /// <param name="code">验证码</param>
    /// <returns>验证码是否正确</returns>
    /// <exception cref="AccountException">验证码已过期或不正确时抛出</exception>
    public async ValueTask CheckConfirmCodeAsync(string unique, string code)
    {
        var rk = GetConfirmKey(unique);
        const string script =
            """
            local current = redis.call('GET', KEYS[1])
            if current == nil then
                return 'expired'
            elseif current == ARGV[1] then
                redis.call('DEL', KEYS[1])
                return 'success'
            else
                return 'incorrect'
            end
            """;
        var result = await redisClientFactory.GetDefaultRedisDatabase().Database
            .ScriptEvaluateAsync(script, [rk], [code]);
        switch (result.ToString())
        {
            case "expired":
                throw new ConfirmException("Confirm key has expired");
            case "success":
                return;
            case "incorrect":
                throw new ConfirmException("Confirm key is incorrect");
            default:
                throw new ConfirmException("Check confirm code failed");
        }
    }

    private static RedisKey GetConfirmKey(string unique)
    {
        return $"NA:CC:{{{unique}}}";
    }
}