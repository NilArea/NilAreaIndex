using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NilArea.Account.DTOs;
using NilArea.Account.Infrastructure.Data;
using NilArea.Account.Infrastructure.Data.Entities;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Contracts.Enums;
using NilArea.Contracts.Exceptions;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     账号相关内容数据库实现
/// </summary>
public sealed class AccountRepository(
    ILogger<AccountRepository> logger,
    AccountDbContext dbContext,
    IIdGenerator<Guid> idGenerator,
    IRedisDatabase redisDatabase) : IAccountRepository
{
    /// <summary>
    ///     布隆过滤器,邮箱是否注册快速查询
    /// </summary>
    private static RedisKey BfAccount => "BF:Account";

    /// <summary>
    ///     系统用户组名称到ID的映射
    /// </summary>
    private Dictionary<string, int> SystemGroupMap { get; set; } = null!;

    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号是否存在</returns>
    public async ValueTask<bool> ExistsAccountAsync(Guid userId)
    {
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.UserId == userId && au.DeleteAt == null);
    }

    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>账号是否存在</returns>
    public async ValueTask<bool> ExistsAccountAsync(string email)
    {
        if (!await redisDatabase.Database.BloomExistsAsync(BfAccount, email)) return false;
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    /// <summary>
    ///     根据用户ID获取账号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号信息</returns>
    public async ValueTask<AccountUserInfo?> GetAccountAsync(Guid userId)
    {
        return await dbContext.AccountUsers.AsNoTracking()
            .Include(au => au.UserGroups)
            .ThenInclude(ug => ug.Group)
            .Where(au => au.UserId == userId && au.DeleteAt == null)
            .Select(au => new AccountUserInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                Groups = au.UserGroups.Select(ug => ug.Group.GroupName)
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    ///     根据邮箱获取账号
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>账号信息</returns>
    public async ValueTask<AccountUserInfo?> GetAccountByEmailAsync(string email)
    {
        return await dbContext.AccountUsers.AsNoTracking()
            .Include(au => au.UserGroups)
            .ThenInclude(ug => ug.Group)
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(au => new AccountUserInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                Groups = au.UserGroups.Select(ug => ug.Group.GroupName)
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    ///     新建账号
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="passwordHash">密码哈希</param>
    /// <param name="username">用户名</param>
    /// <returns>新建的账号信息</returns>
    /// <exception cref="AccountException">邮箱已注册时抛出</exception>
    public async ValueTask<AccountUserInfo> InsertAccountAsync(string email, string passwordHash, string username)
    {
        if (await ExistsAccountAsync(email))
            throw new AccountException("Email already registered", AccountAction.Register);

        if (!SystemGroupMap.TryGetValue("Users", out var gid))
            throw new AccountException("System group 'Users' not found", AccountAction.Register);

        var uid = idGenerator.NextId();
        var timeNow = DateTime.UtcNow;

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var nau = new AccountUser
            {
                UserId = uid,
                Email = email,
                UserName = username,
                PasswordSaltHash = passwordHash,
                CreatedAt = timeNow,
                UpdateAt = timeNow
            };

            var ug = new AccountUserGroup
            {
                UserId = uid,
                GroupId = gid,
                JoinedAt = timeNow
            };

            await dbContext.AccountUsers.AddAsync(nau);
            await dbContext.AccountUserGroups.AddAsync(ug);
            await dbContext.SaveChangesAsync();
            await redisDatabase.Database.BloomAddAsync(BfAccount, nau.Email);
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            await transaction.RollbackAsync();
            throw new AccountException("Account already exists.", AccountAction.Register);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Failed to create account");
            throw new AccountException("Failed to create account", AccountAction.Register);
        }

        return new AccountUserInfo
        {
            UserId = uid,
            Email = email,
            UserName = username,
            CreatedAt = timeNow,
            Groups = ["Users"]
        };
    }

    /// <summary>
    ///     更新账号信息
    /// </summary>
    /// <param name="account">账号信息</param>
    /// <returns>更新后的账号信息</returns>
    /// <exception cref="AccountException">账号不存在时抛出</exception>
    public async ValueTask<AccountUserInfo> UpdateAccountAsync(AccountUserInfo account)
    {
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == account.UserId && au.DeleteAt == null);
        if (user == null)
            throw new AccountException("Account not found");

        // 检查邮箱是否已被其他用户使用
        if (user.Email != account.Email && await dbContext.AccountUsers.AsNoTracking()
                .AnyAsync(au => au.Email == account.Email && au.UserId != account.UserId && au.DeleteAt == null))
            throw new AccountException("Email already in use");

        user.Email = account.Email;
        user.UserName = account.UserName;
        user.UpdateAt = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new AccountException("Email already in use");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update account: {UserId}", account.UserId);
            throw new AccountException("Failed to update account");
        }

        return account;
    }

    /// <summary>
    ///     修改账号密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="newPasswordHash">新密码哈希</param>
    /// <returns>是否修改成功</returns>
    /// <exception cref="AccountException">账号不存在时抛出</exception>
    public async ValueTask<bool> ChangePasswordAsync(Guid userId, string newPasswordHash)
    {
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user is null) throw new AccountException(AccountAction.ChangePassword);

        user.PasswordSaltHash = newPasswordHash;
        user.UpdateAt = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException)
        {
            logger.LogError(ex, "Failed to change password for user: {UserId}", userId);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to change password for user: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    ///     修改账号邮箱
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="newEmail">新邮箱</param>
    /// <returns>是否修改成功</returns>
    public async ValueTask<bool> ChangeEmailAsync(Guid userId, string newEmail)
    {
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null)
            return false;

        // 检查邮箱是否已被其他用户使用
        if (user.Email != newEmail && await dbContext.AccountUsers.AsNoTracking()
                .AnyAsync(au => au.Email == newEmail && au.UserId != userId && au.DeleteAt == null))
            return false;

        user.Email = newEmail;
        user.UpdateAt = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            logger.LogError(ex, "Email already in use: {Email}", newEmail);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to change email for user: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    ///     删除账号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否删除成功</returns>
    public async ValueTask<bool> DeleteAccountAsync(Guid userId)
    {
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null)
            return false;

        user.DeleteAt = DateTime.UtcNow;
        user.UpdateAt = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete account: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    ///     初始化账号仓库
    /// </summary>
    /// <returns>任务完成状态</returns>
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

    /// <summary>
    ///     释放账号仓库资源
    /// </summary>
    /// <returns>任务完成状态</returns>
    public Task DisposeAsync()
    {
        logger.LogInformation("Disposing Account Repository");
        return Task.CompletedTask;
    }
}