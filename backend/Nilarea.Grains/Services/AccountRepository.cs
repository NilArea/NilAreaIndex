using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Contracts.Dto;
using NilArea.Grains.DbContext;
using NilArea.Grains.Dtos;
using NilArea.Interfaces.Exceptions;
using NilArea.Interfaces.IGrains;
using StackExchange.Redis;

namespace NilArea.Grains.Services;

public interface IAccountRepository : IAsyncLifetime
{
    ValueTask<bool> ExistsAccountAsync(string email);
    ValueTask CacheConfirmKeyAsync(string email, string key, ConfirmKey keyCode);
    ValueTask<bool> CheckConfirmKeyAsync(string email, string key);
    ValueTask<AccountUser> InsertAccountAsync(string email, string passwordHash, string username);
    ValueTask<AccountUser> FindAccountAsync(string email);
    ValueTask<T> FindAccountAsync<T>(string email, Expression<Func<AccountUser, T>> select);
    public ValueTask<TokenPair> GenerateTokenAsync(long userId, bool overwrite = true);
}

public sealed class AccountRepository(
    ILogger<AccountRepository> logger,
    NilDbContext dbContext,
    IIdGenerator<long> idGenerator,
    IRedisDatabaseFactory redisDatabaseFactory)
    : IAccountRepository
{
    private static RedisKey BfAccount => StaticValues.BfAccount;
    private static RedisKey ConfirmKeyPrefix => StaticValues.ConfirmKeyPrefix;

    private IDatabase Redis { get; } = redisDatabaseFactory.GetDatabase();
    private Dictionary<string, int> SystemGroupMap { get; set; } = null!;

    public async ValueTask<bool> ExistsAccountAsync(string email)
    {
        if (!await Redis.BloomExistsAsync(BfAccount, email)) return false;
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    public async ValueTask CacheConfirmKeyAsync(string email, string key, ConfirmKey keyCode)
    {
        var rk = ConfirmKeyPrefix.Append(email);
        if (await Redis.KeyExistsAsync(rk))
            throw new AccountException("Confirm key still not expired");
        await Redis.StringSetAsync(rk, key, GetExpirationTime());
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
        var c = await Redis.StringGetAsync(rk);
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
            await Redis.BloomAddAsync(BfAccount, add.Email);
        }
        catch (DbUpdateException ex)when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new AccountException("Account already exists.", AccountAction.Register);
        }

        return add;
    }

    public async ValueTask<AccountUser> FindAccountAsync(string email)
    {
        var account = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.Email == email && au.DeleteAt == null);
        if (account is null)
            throw new AuthenticationException("Email does not registered");
        return account;
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

    public async ValueTask<TokenPair> GenerateTokenAsync(long userId, bool overwrite = true)
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

    public async Task InitializeAsync()
    {
        try
        {
            await Redis.BloomReserveAsync(BfAccount, 0.01d, 16384);
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
