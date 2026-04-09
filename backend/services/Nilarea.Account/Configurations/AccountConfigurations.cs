using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NilArea.Account.Infrastructure.Data;
using NilArea.Account.Infrastructure.Repositories;
using NilArea.Account.Infrastructure.Services;
using NilArea.Common;
using NilArea.Common.Utils;
using NilArea.Contracts;
using ShardingCore;
using ShardingCore.Core.ShardingConfigurations;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace NilArea.Account.Configurations;

public static class AccountConfigurations
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddDataValidators(IConfiguration configuration)
        {
            return collection
                .AddCommonValidators()
                .AddContractsValidators()
                .AddAccountValidator();
        }

        public IServiceCollection AddNilareaTools(IConfiguration configuration)
        {
            return collection
                .AddSingleton<IIdGenerator<Guid>, GuidGenerator>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        }

        public IServiceCollection AddNilareaDbContext(IConfiguration configuration)
        {
            collection.AddShardingDbContext<AccountDbContext>()
                .UseConfig(ConfigDbContext)
                .AddShardingCore();
            return collection;

            void ConfigDbContext(ShardingConfigOptions op)
            {
                var sqlShard0 = configuration.SafeGetConfigureValue("MYSQL_SHARD0");
                var sqlShard1 = configuration.SafeGetConfigureValue("MYSQL_SHARD1");
                var sqlShard2 = configuration.SafeGetConfigureValue("MYSQL_SHARD2");
                //如何通过字符串查询创建DbContext
                op.UseShardingQuery((conStr, sqb) => { sqb.UseMySQL(conStr); });
                //如何通过事务创建DbContext
                op.UseShardingTransaction((conStr, stb) => { stb.UseMySQL(conStr); });
                op.AddDefaultDataSource("ds0", sqlShard0);
                op.AddExtraDataSource(_ => new Dictionary<string, string>
                {
                    { "ds1", sqlShard1 },
                    { "ds2", sqlShard2 }
                });
            }
        }

        public IServiceCollection AddNilareaCache(IConfiguration configuration)
        {
            collection.AddSingleton<IRedisClientFactory, RedisClientFactory>();
            collection.AddSingleton<ISerializer, NewtonsoftSerializer>();
            collection.AddSingleton<IRedisClient>(provider =>
                provider.GetRequiredService<IRedisClientFactory>().GetDefaultRedisClient());
            collection.AddSingleton<IRedisDatabase>(provider =>
                provider.GetRequiredService<IRedisClientFactory>().GetDefaultRedisClient().GetDefaultDatabase());
            collection.AddSingleton<IEnumerable<RedisConfiguration>>(sp =>
            {
                var conf = new RedisConfiguration
                {
                    ConnectionString = configuration.SafeGetConfigureValue("REDIS_CLUSTER")
                };
                return [conf];
            });
            return collection;
        }

        public IServiceCollection AddNilareaServices(IConfiguration configuration)
        {
            collection
                .AddSingleton<IAccountRepository, AccountRepository>()
                .AddSingleton<IPermissionRepository, PermissionRepository>()
                .AddSingleton<IConfirmRepository, ConfirmRepository>()
                .AddSingleton<IEmailServices, EmailServices>()
                .AddSingleton<ITokenService, TokenService>()
                .AddSingleton<ITokenStorageService, TokenStorageService>()
                .AddHostedService<ServiceInitializer>();
            return collection;
        }
    }
}