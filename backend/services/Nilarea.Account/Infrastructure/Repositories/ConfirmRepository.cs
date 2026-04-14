using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NilArea.Account.Infrastructure.Data;
using NilArea.Common.Services;
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
    AccountDbContext dbContext,
    IRedisDatabase redisDatabase
) : IConfirmRepository, IAsyncLifetime
{
    /// <summary>
    ///     验证码缓存键前缀
    /// </summary>
    private static RedisKey ConfirmKeyPrefix => "NA:CK";

    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing confirm repository...");
    }

    public async Task DisposeAsync()
    {
        logger.LogInformation("Disposing confirm repository...");
    }

    /// <summary>
    ///     根据邮箱查找账号的验证密钥
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>用户ID和密码哈希</returns>
    /// <exception cref="AuthenticationException">邮箱未注册时抛出</exception>
    public async ValueTask<(long UserId, string PasswordSaltHash)> GetAccountVerifyAsync(string email)
    {
        var up = await dbContext.AccountUsers.AsNoTracking()
                     .Where(au => au.Email == email && au.DeleteAt == null)
                     .Select(au => new { au.UserId, au.PasswordSaltHash })
                     .FirstOrDefaultAsync() ??
                 throw new AuthenticationException("Email does not registered", AuthenticationResult.Failed);
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
            """;
        var keys = new[] { ConfirmKeyPrefix.Append(unique) };
        var args = new RedisValue[] { code, GetExpirationTime().TotalSeconds };
        var result = await redisDatabase.Database.ScriptEvaluateAsync(luaScript, keys, args);
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
        var rk = ConfirmKeyPrefix.Append(unique);
        return await redisDatabase.Database.KeyExistsAsync(rk);
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
        var rk = ConfirmKeyPrefix.Append(unique);
        const string script =
            """
            local current = redis.call('GET', KEYS[1])
            if current == nil then
                return 'expired'
            if current == ARGV[1] then
                redis.call('DEL', KEYS[1])
                return 'success'
            else
                return 'incorrect'
            end
            """;
        var result = await redisDatabase.Database.ScriptEvaluateAsync(script, [rk], [code]);
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
}