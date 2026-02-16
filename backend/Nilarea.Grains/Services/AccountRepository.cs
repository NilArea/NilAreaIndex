using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NilArea.Common.Services;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Contracts.Dto;
using NilArea.Grains.DbContext;
using NilArea.Grains.Dbe;
using NilArea.Interfaces.Exceptions;
using NilArea.Interfaces.IGrains;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Grains.Services;

public interface IAccountRepository : IAsyncLifetime
{
    ValueTask<bool> ExistsAccountAsync(string email);
    ValueTask CacheConfirmKeyAsync(string email, string key, ConfirmKey keyCode);
    ValueTask<bool> CheckConfirmKeyAsync(string email, string key);
    ValueTask<AccountUser> InsertAccountAsync(string email, string passwordHash, string username);
    ValueTask<AccountUser> FindAccountAsync(Guid uid);
    ValueTask<AccountUser> FindAccountAsync(string email);
    ValueTask<T> FindAccountAsync<T>(Guid uid, Expression<Func<AccountUser, T>> select);
    ValueTask<T> FindAccountAsync<T>(string email, Expression<Func<AccountUser, T>> select);

    ValueTask<T[]> FindAccountsAsync<T>(Expression<Func<AccountUser, bool>> filter,
        Expression<Func<AccountUser, T>> select);

    ValueTask<TokenPair> GenerateTokenAsync(Guid userId, bool overwrite = true);
    ValueTask<PermissionTag[]> GetAllPermissionAsync(Guid userId);
}

public sealed class AccountRepository(
    ILogger<AccountRepository> logger,
    NilDbContext dbContext,
    IIdGenerator<Guid> idGenerator,
    IRedisDatabase redisDatabase)
    : IAccountRepository
{
    private static RedisKey BfAccount => StaticValues.BfAccount;
    private static RedisKey ConfirmKeyPrefix => StaticValues.ConfirmKeyPrefix;

    private Dictionary<string, int> SystemGroupMap { get; set; } = null!;

    public async ValueTask<bool> ExistsAccountAsync(string email)
    {
        if (!await redisDatabase.Database.BloomExistsAsync(BfAccount, email)) return false;
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    public async ValueTask CacheConfirmKeyAsync(string email, string key, ConfirmKey keyCode)
    {
        var rk = ConfirmKeyPrefix.Append(email);
        if (await redisDatabase.Database.KeyExistsAsync(rk))
            throw new AccountException("Confirm key still not expired");
        await redisDatabase.Database.StringSetAsync(rk, key, GetExpirationTime());
        return;

        TimeSpan GetExpirationTime()
        {
            return keyCode switch
            {
                _ => TimeSpan.FromMinutes(5)
            };
        }
    }

    public async ValueTask<bool> CheckConfirmKeyAsync(string email, string key)
    {
        var rk = ConfirmKeyPrefix.Append(email);
        var c = await redisDatabase.Database.StringGetAsync(rk);
        return !c.IsNull && string.Equals(c, key);
    }

    public async ValueTask<AccountUser> InsertAccountAsync(string email, string passwordHash, string username)
    {
        if (await ExistsAccountAsync(email))
            throw new AccountException("Email already registered", AccountAction.Register);
        var uid = idGenerator.NextId();
        var add = new AccountUser
        {
            UserId = uid,
            Email = email,
            UserName = username,
            PasswordSaltHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
        var gid = SystemGroupMap[StaticValues.AccountSystemGroupNames.Users];
        var ug = new AccountUserGroup
        {
            UserId = uid,
            GroupId = gid
        };
        await dbContext.AccountUsers.AddAsync(add);
        await dbContext.AccountUserGroups.AddAsync(ug);
        try
        {
            await dbContext.SaveChangesAsync();
            await redisDatabase.Database.BloomAddAsync(BfAccount, add.Email);
        }
        catch (DbUpdateException ex)when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new AccountException("Account already exists.", AccountAction.Register);
        }

        return add;
    }

    public async ValueTask<AccountUser> FindAccountAsync(Guid uid)
    {
        var account = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == uid);
        if (account is null || account.DeleteAt != null)
            throw new AuthenticationException("Account not available");
        return account;
    }

    public async ValueTask<AccountUser> FindAccountAsync(string email)
    {
        var account = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.Email == email && au.DeleteAt == null);
        if (account is null)
            throw new AuthenticationException("Email does not registered");
        return account;
    }

    public async ValueTask<T> FindAccountAsync<T>(Guid uid, Expression<Func<AccountUser, T>> select)
    {
        if (await dbContext.AccountUsers
                .AnyAsync(au => (au.UserId == uid) & (au.DeleteAt == null)))
            throw new AuthenticationException("Account not available");
        return await dbContext.AccountUsers
            .Where(au => (au.UserId == uid) & (au.DeleteAt == null))
            .Select(select)
            .FirstAsync();
    }

    public async ValueTask<T> FindAccountAsync<T>(string email, Expression<Func<AccountUser, T>> select)
    {
        if (!await dbContext.AccountUsers.AnyAsync(au => au.Email == email && au.DeleteAt == null))
            throw new AuthenticationException("Email does not registered");
        return await dbContext.AccountUsers
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(select)
            .FirstAsync();
    }

    public async ValueTask<T[]> FindAccountsAsync<T>(
        Expression<Func<AccountUser, bool>> filter,
        Expression<Func<AccountUser, T>> select)
    {
        return await dbContext.AccountUsers.Where(filter).Select(select).ToArrayAsync();
    }

    public async ValueTask<TokenPair> GenerateTokenAsync(Guid userId, bool overwrite = true)
    {
        var user = await dbContext.AccountUsers.FirstAsync(u => u.UserId == userId);
        return new TokenPair
        {
            AccessToken = string.Empty,
            AccessExpire = DateTime.UtcNow.AddMinutes(15),
            RefreshToken = string.Empty,
            RefreshExpire = DateTime.UtcNow.AddDays(3)
        };
    }

    public async ValueTask<PermissionTag[]> GetAllPermissionAsync(Guid userId)
    {
        var user = dbContext.AccountUsers
            .Where(u => u.UserId == userId && u.DeleteAt == null);
        if (!await user.AnyAsync()) throw new AccountException("Account not available");
        var permissions = await user.SelectMany(u => u.Permissions
                .Select(up => up.Permission))
            .Union(user.SelectMany(u => u.UserGroups)
                .SelectMany(ug => ug.Group.Permissions
                    .Select(gp => gp.Permission)))
            .ToArrayAsync();
        return permissions.DistinctBy(p => p.PermissionId).ToArray();
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
            SystemGroupMap = await dbContext.AccountGroups
                .Where(g => g.IsSystemGroup == true)
                .Select(g => new { g.GroupName, g.GroupId })
                .ToDictionaryAsync(g => g.GroupName, g => g.GroupId);
            logger.LogInformation("Successful Create SystemGroupMap 'SystemGroupMap'");
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
