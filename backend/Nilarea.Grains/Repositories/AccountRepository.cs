using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using NilArea.Common.Utils;
using NilArea.Contracts.Dto;
using NilArea.Grains.DbContext;
using NilArea.Grains.Dtos;
using NilArea.Interfaces.Exceptions;
using StackExchange.Redis;

namespace NilArea.Grains.Repositories;

public interface IAccountRepository
{
    ValueTask<bool> ExistsAccountAsync(string email);
    ValueTask<(long, DateTime)> InsertAccount(RegisterRequest accountInfo);
}

public class AccountRepository(
    ILogger<AccountRepository> logger,
    NilDbContext dbContext,
    IIdGenerator<long> idGenerator,
    IPasswordHasher passwordHasher,
    IConnectionMultiplexer connectionMultiplexer
) : IAccountRepository
{
    private IDatabase Redis { get; } = connectionMultiplexer.GetDatabase();

    public async ValueTask<bool> ExistsAccountAsync(string email)
    {
        return await dbContext.AccountUsers.AnyAsync(au =>
            au.Email == email && (au.DeleteAt == null || au.DeleteAt > DateTime.UtcNow));
    }

    public async ValueTask<(long, DateTime)> InsertAccount(RegisterRequest accountInfo)
    {
        if (await ExistsAccountAsync(accountInfo.Email))
            throw new AccountException("Account already exists.", AccountAction.Register);
        var uid = idGenerator.NextId();
        var add = new AccountDbDto
        {
            UserId = uid,
            Email = accountInfo.Email,
            UserName = accountInfo.Username,
            PasswordSaltHash = passwordHasher.SaltedHash(accountInfo.Password),
            CreatedAt = DateTime.UtcNow
        };
        await dbContext.AccountUsers.AddAsync(add);
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new AccountException("Account already exists.", AccountAction.Register);
        }

        return (uid, add.CreatedAt.Value);
    }

    private static RedisKey GenRedisKey()
    {
        throw new NotImplementedException();
    }
}
