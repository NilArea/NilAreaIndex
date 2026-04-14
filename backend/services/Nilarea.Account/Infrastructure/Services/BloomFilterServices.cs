using Microsoft.Extensions.Logging;
using NilArea.Common.Services;
using NilArea.Contracts;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Account.Infrastructure.Services;

public interface IBloomFilterServices
{
    /// <summary>
    ///     检查邮箱是否已注册
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>
    ///     表示异步操作的任务，返回一个布尔值：
    ///     <para>true - 元素可能存在于过滤器中（可能是误报）</para>
    ///     <para>false - 元素绝对不存在于过滤器中</para>
    /// </returns>
    public ValueTask<bool> CheckEmailAsync(string email);

    /// <summary>
    ///     添加邮箱到布隆过滤器
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <returns>
    ///     表示异步操作的任务，返回一个布尔值：
    ///     <para>true - 元素是首次添加（之前不存在）</para>
    ///     <para>false - 元素可能已经存在（可能是误报）</para>
    /// </returns>
    public ValueTask<bool> AddEmailAsync(string email);
}

public class BloomFilterServices(
    ILogger<BloomFilterServices> logger,
    IRedisClientFactory redisClientFactory
) : IBloomFilterServices, IAsyncLifetime
{
    /// <summary>
    ///     布隆过滤器,邮箱是否注册快速查询
    /// </summary>
    private static RedisKey BfAccount => "BF:Account";

    /// <summary>
    ///     初始化账号仓库
    /// </summary>
    /// <returns>任务完成状态</returns>
    public async Task InitializeAsync()
    {
        var result = await redisClientFactory.GetDefaultRedisDatabase().Database
            .BloomReserveAsync(BfAccount, 0.01d, 16384, true);
        if (result.IsSuccess)
            logger.LogInformation("Successful Initialize BloomFilter 'BF:Account'");
        else
            logger.LogError("Failed to Initialize BloomFilter 'BF:Account': {ErrorMessage}", result.ErrorMessage);
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

    public async ValueTask<bool> CheckEmailAsync(string email)
    {
        return await redisClientFactory.GetDefaultRedisDatabase().Database.BloomExistsAsync(BfAccount, email);
    }


    public async ValueTask<bool> AddEmailAsync(string email)
    {
        return await redisClientFactory.GetDefaultRedisDatabase().Database.BloomAddAsync(BfAccount, email);
    }
}