using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NilArea.Account.DTOs;
using NilArea.Account.Infrastructure.Data;
using NilArea.Account.Infrastructure.Data.Entities;
using NilArea.Account.Infrastructure.Services;
using NilArea.Common.Utils;
using NilArea.Contracts.Enums;
using NilArea.Contracts.Exceptions;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     账号相关内容数据库实现
/// </summary>
public sealed class AccountRepository(
    ILogger<AccountRepository> logger,
    IDbContextFactory<AccountDbContext> dbContextFactory,
    IIdGenerator<long> idGenerator,
    IBloomFilterServices bloomFilterServices
) : IAccountRepository
{
    /// <summary>
    ///     检查账号是否存在
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号是否存在</returns>
    public async ValueTask<bool> ExistsAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
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
        if (!await bloomFilterServices.CheckEmailAsync(email)) return false;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    /// <summary>
    ///     根据用户ID获取账号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>账号信息</returns>
    public async ValueTask<AccountUserInfo?> GetAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.UserId == userId && au.DeleteAt == null)
            .Select(au => new AccountUserInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                UpdatedAt = au.UpdateAt
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
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AccountUsers.AsNoTracking()
            .Where(au => au.Email == email && au.DeleteAt == null)
            .Select(au => new AccountUserInfo
            {
                UserId = au.UserId,
                Email = au.Email,
                UserName = au.UserName,
                CreatedAt = au.CreatedAt,
                UpdatedAt = au.UpdateAt
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
        var uid = idGenerator.NextId();
        var timeNow = DateTimeOffset.UtcNow;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
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

            await dbContext.AccountUsers.AddAsync(nau);
            await dbContext.SaveChangesAsync();
            await bloomFilterServices.AddEmailAsync(email);
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
            UpdatedAt = timeNow
        };
    }

    /// <summary>
    ///     修改账号密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="newPasswordHash">新密码哈希</param>
    /// <returns>是否修改成功</returns>
    /// <exception cref="AccountException">账号不存在时抛出</exception>
    public async ValueTask<AccountUserInfo> ChangePasswordAsync(long userId, string newPasswordHash)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user is null) throw new AccountException("Account not found", AccountAction.ChangePassword);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            user.PasswordSaltHash = newPasswordHash;
            user.UpdateAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to change password", AccountAction.ChangePassword);
        }

        return new AccountUserInfo
        {
            UserId = userId,
            Email = user.Email,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdateAt
        };
    }

    /// <summary>
    ///     修改账号邮箱
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="newEmail">新邮箱</param>
    /// <returns>是否修改成功</returns>
    public async ValueTask<AccountUserInfo> ChangeEmailAsync(long userId, string newEmail)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null) throw new AccountException("Account not found", AccountAction.ChangeEmail);

        // 检查邮箱是否已被其他用户使用
        if (user.Email != newEmail && await dbContext.AccountUsers.AsNoTracking()
                .AnyAsync(au => au.Email == newEmail && au.UserId != userId && au.DeleteAt == null))
            throw new AccountException("Email already in use", AccountAction.ChangeEmail);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            user.Email = newEmail;
            user.UpdateAt = DateTimeOffset.UtcNow;
            await dbContext.SaveChangesAsync();
            await bloomFilterServices.AddEmailAsync(newEmail);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to change email for user", AccountAction.ChangeEmail);
        }

        return new AccountUserInfo
        {
            UserId = userId,
            Email = newEmail,
            UserName = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdateAt
        };
    }

    /// <summary>
    ///     删除账号
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否删除成功</returns>
    public async ValueTask DeleteAccountAsync(long userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.UserId == userId && au.DeleteAt == null);
        if (user == null) throw new AccountException("Account not found", AccountAction.Delete);
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var dateTime = DateTimeOffset.UtcNow;
            user.DeleteAt = dateTime;
            user.UpdateAt = dateTime;
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AccountException("Failed to delete account", AccountAction.Delete);
        }
    }
}