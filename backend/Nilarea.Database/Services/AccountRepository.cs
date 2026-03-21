using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Contracts.Dto;
using Nilarea.Database.Abstract.Dto;
using Nilarea.Database.Abstract.Services;
using Nilarea.Database.Dbe;
using NilArea.Interfaces.Exceptions;
using NilArea.Interfaces.IGrains;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Nilarea.Database.Services;

public sealed class AccountRepository(
    ILogger<AccountRepository> logger,
    NilDbContext dbContext,
    IIdGenerator<Guid> idGenerator,
    IRedisDatabase redisDatabase)
    : IAccountRepository
{
    /// <summary>
    ///     布隆过滤器,邮箱是否注册快速查询
    /// </summary>
    private static RedisKey BfAccount => StaticValues.BfAccount;

    /// <summary>
    ///     验证码缓存键前缀
    /// </summary>
    private static RedisKey ConfirmKeyPrefix => StaticValues.ConfirmKeyPrefix;

    /// <summary>
    ///     系统用户组名称到ID的映射
    /// </summary>
    private Dictionary<string, int> SystemGroupMap { get; set; } = null!;

    public async ValueTask<bool> ExistsAccountAsync(Guid userId)
    {
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.UserId == userId && au.DeleteAt == null);
    }

    public async ValueTask<bool> ExistsAccountAsync(string email)
    {
        if (!await redisDatabase.Database.BloomExistsAsync(BfAccount, email)) return false;
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    public async ValueTask<AccountUserInfo?> FindAccountAsync(Guid userId)
    {
        return await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.UserId == userId && au.DeleteAt == null)
            .Select(au => new AccountUserInfo(au.UserId, au.Email, au.UserName, au.CreatedAt,
                au.UserGroups.Select(ug => ug.Group.GroupName)))
            .FirstOrDefaultAsync();
    }

    public async ValueTask<AccountUserInfo?> FindAccountAsync(string email)
    {
        return await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(au => new AccountUserInfo(au.UserId, au.Email, au.UserName, au.CreatedAt,
                au.UserGroups.Select(ug => ug.Group.GroupName)))
            .FirstOrDefaultAsync();
    }

    public async ValueTask<AccountUserInfo> InsertAccountAsync(string email, string passwordHash, string username)
    {
        if (await ExistsAccountAsync(email))
            throw new AccountException("Email already registered", AccountAction.Register);
        var uid = idGenerator.NextId();
        var timeNow = DateTime.UtcNow;
        var nau = new AccountUser
        {
            UserId = uid,
            Email = email,
            UserName = username,
            PasswordSaltHash = passwordHash,
            CreatedAt = timeNow
        };
        var gid = SystemGroupMap[StaticValues.AccountSystemGroupNames.Users];
        var ug = new AccountUserGroup
        {
            UserId = uid,
            GroupId = gid
        };
        await dbContext.AccountUsers.AddAsync(nau);
        await dbContext.AccountUserGroups.AddAsync(ug);
        try
        {
            await dbContext.SaveChangesAsync();
            await redisDatabase.Database.BloomAddAsync(BfAccount, nau.Email);
        }
        catch (DbUpdateException ex)when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new AccountException("Account already exists.", AccountAction.Register);
        }

        return new AccountUserInfo(uid, email, username, timeNow, [StaticValues.AccountSystemGroupNames.Users]);
    }

    public async ValueTask UpdateAccountAsync(object update)
    {
    }

    public async ValueTask<bool> ChangePasswordAsync(Guid userId, string newPasswordHash)
    {
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user is null) throw new AccountException(AccountAction.ChangePassword);
        user.PasswordSaltHash = newPasswordHash;
        dbContext.AccountUsers.Update(user);
        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)when (ex.InnerException is MySqlException)
        {
            return false;
        }
    }

    public async ValueTask<bool> ChangeEmailAsync(Guid userId, string newEmail)
    {
        if (await dbContext.AccountUsers.AsNoTracking()
                .AnyAsync(au => au.Email == newEmail && au.DeleteAt == null))
            return false;
        var count = await dbContext.AccountUsers.Where(au => au.UserId == userId && au.DeleteAt == null)
            .ExecuteUpdateAsync(au => au.SetProperty(p => p.Email, newEmail));
        return count switch
        {
            0 => false,
            1 => true,
            _ => throw new AccountException("Unknown error")
        };
    }

    public async ValueTask<bool> DeleteAccountAsync(Guid userId)
    {
        var count = await dbContext.AccountUsers.Where(au => au.UserId == userId && au.DeleteAt == null)
            .ExecuteUpdateAsync(au => au.SetProperty(p => p.DeleteAt, DateTime.UtcNow));
        return count switch
        {
            0 => false,
            1 => true,
            _ => throw new AccountException("Unknown error")
        };
    }

    public async ValueTask<(Guid UserId, string PasswordSaltHash)> GetAccountVerifyAsync(string email)
    {
        var up = await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(au => new { au.UserId, au.PasswordSaltHash })
            .FirstOrDefaultAsync() ?? throw new AuthenticationException("Email does not registered");
        return (up.UserId, up.PasswordSaltHash);
    }

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
                _ => TimeSpan.FromMinutes(5)
            };
        }
    }

    public async ValueTask<bool> ExistConfirmCodeAsync(string unique)
    {
        var rk = ConfirmKeyPrefix.Append(unique);
        return await redisDatabase.Database.KeyExistsAsync(rk);
    }

    public async ValueTask<bool> CheckConfirmCodeAsync(string unique, string code)
    {
        var rk = ConfirmKeyPrefix.Append(unique);
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
        return (bool)ret;
    }

    public async ValueTask<PermissionTagInfo[]> GetAllPermissionAsync(Guid userId)
    {
        var user = dbContext.AccountUsers.AsNoTracking()
            .Where(u => u.UserId == userId && u.DeleteAt == null);
        if (!await user.AnyAsync()) throw new AccountException("Account not available");
        var permissions = await user.SelectMany(au =>
                au.Permissions.Select(ups => ups.Permission)
                    .Select(up => new { up.PermissionId, up.PermissionName }))
            .Union(user.SelectMany(au => au.UserGroups)
                .SelectMany(aug => aug.Group.Permissions)
                .Select(gp => gp.Permission)
                .Select(gp => new { gp.PermissionId, gp.PermissionName }))
            .ToArrayAsync();
        return permissions
            .DistinctBy(p => p.PermissionId)
            .Select(p => new PermissionTagInfo(p.PermissionId, p.PermissionName))
            .ToArray();
    }

    public async ValueTask<TokenPair> GenerateTokenAsync(Guid userId, bool overwrite = true)
    {
        var user = await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.UserId == userId && au.DeleteAt == null)
            .Select(u => new { u.UserId, u.CreatedAt })
            .FirstOrDefaultAsync();
        return new TokenPair
        {
            AccessToken = string.Empty,
            AccessExpire = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = string.Empty,
            RefreshExpire = DateTime.UtcNow.AddDays(3)
        };
    }

    public async Task InitializeAsync()
    {
        try
        {
            await redisDatabase.Database.BloomReserveAsync(BfAccount, 0.01d, 16384);
            logger.LogInformation("Successful Create BloomFilter 'BF:Account'");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "BloomFilter 'BF:Account' already Created");
        }

        try
        {
            SystemGroupMap = await dbContext.AccountGroups.AsNoTracking()
                .Where(g => g.IsSystemGroup == true)
                .Select(g => new { g.GroupName, g.GroupId })
                .ToDictionaryAsync(g => g.GroupName, g => g.GroupId);
            logger.LogInformation("Successful Create SystemGroupMap 'SystemGroupMap' With {Count} Groups.",
                SystemGroupMap.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create system groups");
        }
    }

    public Task DisposeAsync()
    {
        logger.LogInformation("Disposing Account Repository");
        return Task.CompletedTask;
    }
}
