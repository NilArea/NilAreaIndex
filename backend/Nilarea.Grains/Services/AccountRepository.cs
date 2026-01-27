using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Contracts.Dto;
using NilArea.Grains.DbContext;
using NilArea.Grains.Dtos;
using NilArea.Interfaces.Exceptions;
using StackExchange.Redis;

namespace NilArea.Grains.Services;

public interface IAccountRepository
{
    internal ValueTask InitializeAsync();
    ValueTask<bool> ExistsAccountAsync(string email);
    ValueTask<RegisterResponse> InsertAccountAsync(RegisterRequest accountInfo);
    ValueTask<LoginResponse> VerifyLoginInfoAsync(LoginRequest loginInfo);
}

public sealed class AccountRepository(
    ILogger<AccountRepository> logger,
    NilDbContext dbContext,
    IIdGenerator<long> idGenerator,
    IPasswordHasher passwordHasher,
    IRedisDatabaseFactory redisDatabaseFactory)
    : IAccountRepository
{
    private static RedisKey BfAcount => StaticValues.BfAcount;

    private IDatabase Redis { get; } = redisDatabaseFactory.GetDatabase();

    public async ValueTask<bool> ExistsAccountAsync(string email)
    {
        if (!await Redis.BloomExistsAsync(BfAcount, email)) return false;
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && au.DeleteAt == null);
    }

    public async ValueTask<RegisterResponse> InsertAccountAsync(RegisterRequest accountInfo)
    {
        if (await ExistsAccountAsync(accountInfo.Email))
            throw new AccountException("Email already registered", AccountAction.Register);
        var uid = idGenerator.NextId();
        var add = new AccountUserDto
        {
            UserId = uid,
            Email = accountInfo.Email,
            UserName = accountInfo.Username,
            PasswordSaltHash = passwordHasher.SaltedHash(accountInfo.Password),
            CreatedAt = DateTime.UtcNow
        };
        var gid = await dbContext.AccountGroups
            .Where(g => g.GroupName == StaticValues.AccountSystemGroupNames.User)
            .Select(g => g.GroupId).FirstAsync();
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
            await Redis.BloomAddAsync(BfAcount, add.Email);
        }
        catch (DbUpdateException ex)when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new AccountException("Account already exists.", AccountAction.Register);
        }

        return new RegisterResponse
        {
            UserId = uid,
            Email = accountInfo.Email,
            Username = accountInfo.Username,
            CreatedAt = add.CreatedAt
        };
    }

    public async ValueTask<LoginResponse> VerifyLoginInfoAsync(LoginRequest loginInfo)
    {
        var account = await dbContext.AccountUsers
            .FirstOrDefaultAsync(au => au.Email == loginInfo.Email && au.DeleteAt == null);
        if (account is null)
            throw new AccountException("Email does not registered", AccountAction.Login);
        if (!passwordHasher.Verify(loginInfo.Password, account.PasswordSaltHash))
            throw new AccountException("Password does not match", AccountAction.Login);
        return await GenLoginResponseAsync(account.UserId);
    }


    public async ValueTask InitializeAsync()
    {
        try
        {
            await Redis.BloomReserveAsync(BfAcount, 0.01d, 16384);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Try Create a new BF, but it was existed");
        }
    }

    private static RedisKey GenRedisKey()
    {
        throw new NotImplementedException();
    }

    private async ValueTask<LoginResponse> GenLoginResponseAsync(long userId)
    {
        return new LoginResponse
        {
            UserId = userId
        };
    }
}
