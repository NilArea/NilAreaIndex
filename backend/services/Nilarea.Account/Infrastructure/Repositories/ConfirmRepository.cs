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
    AccountDbContext dbContext,
    IRedisDatabase redisDatabase
) : IConfirmRepository
{
    /// <summary>
    ///     验证码缓存键前缀
    /// </summary>
    private static RedisKey ConfirmKeyPrefix => "NA:CK";

    /// <summary>
    ///     根据邮箱查找账号的验证密钥
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>用户ID和密码哈希</returns>
    /// <exception cref="AuthenticationException">邮箱未注册时抛出</exception>
    public async ValueTask<(Guid UserId, string PasswordSaltHash)> GetAccountVerifyAsync(string email)
    {
        var up = await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(au => new { au.UserId, au.PasswordSaltHash })
            .FirstOrDefaultAsync() ?? throw new AuthenticationException("Email does not registered");
        return (up.UserId, up.PasswordSaltHash);
    }

    /// <summary>
    ///     缓存验证码
    /// </summary>
    /// <param name="unique">唯一标识（通常是邮箱）</param>
    /// <param name="code">验证码</param>
    /// <param name="typeCode">验证码类型</param>
    /// <returns>是否缓存成功</returns>
    public async ValueTask<bool> CacheConfirmCodeAsync(string unique, string code, ConfirmType typeCode)
    {
        var rk = ConfirmKeyPrefix.Append(unique);
        if (await redisDatabase.Database.KeyExistsAsync(rk)) return false;
        await redisDatabase.Database.StringSetAsync(rk, code, GetExpirationTime());
        return true;

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
    public async ValueTask<bool> CheckConfirmCodeAsync(string unique, string code)
    {
        var rk = ConfirmKeyPrefix.Append(unique);
        if (!await redisDatabase.Database.KeyExistsAsync(rk))
            throw new AccountException("Verification code has expired");

        const string script = """
                              local current = redis.call('GET', KEYS[1])
                              if current == ARGV[1] then
                                  redis.call('DEL', KEYS[1])
                                  return 1
                              else
                                  return 0
                              end
                              """;
        var ret = await redisDatabase.Database.ScriptEvaluateAsync(script, [rk], [code]);
        if (ret.IsNull) return false;
        var result = (bool)ret;
        if (!result)
            throw new AccountException("Verification code is incorrect");
        return result;
    }
}