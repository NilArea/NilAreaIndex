using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NilArea.Common;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Grains;
using NilArea.Grains.DbContext;
using NilArea.Grains.Services;
using NilArea.Interfaces;
using ShardingCore;
using ShardingCore.Core.ShardingConfigurations;
using ShardingCore.Sharding.ReadWriteConfigurations;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace NilArea.Silo;

public static class Configures
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddDataValidators(IConfiguration configuration)
        {
            return collection
                .AddCommonValidators()
                .AddContractsValidators()
                .AddInterfaceValidators()
                .AddNilareaValidator();
        }

        public IServiceCollection AddNilareaTools(IConfiguration configuration)
        {
            return collection
                .AddSingleton<IIdGenerator<Guid>, GuidGenerator>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        }

        public IServiceCollection AddNilareaDbContext(IConfiguration configuration)
        {
            collection.AddShardingDbContext<NilDbContext>()
                .UseConfig(ConfigDbContext)
                .AddShardingCore();
            return collection;

            void ConfigDbContext(ShardingConfigOptions op)
            {
                var sqlMaster = configuration.SafeGetConfigureValue("MYSQL_MASTER");
                var sqlSlave = configuration.SafeGetConfigureValue("MYSQL_SLAVE");
                const string ds0 = "ds0";
                //如何通过字符串查询创建DbContext
                op.UseShardingQuery((conStr, sqb) => { sqb.UseMySQL(conStr); });
                //如何通过事务创建DbContext
                op.UseShardingTransaction((conStr, stb) => { stb.UseMySQL(conStr); });
                op.AddDefaultDataSource(ds0, sqlMaster);
                op.AddReadWriteSeparation(_ => new Dictionary<string, IEnumerable<string>>
                {
                    { ds0, [sqlSlave] }
                }, ReadStrategyEnum.Loop, ReadWriteDefaultEnableBehavior.DefaultEnable);
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
                .AddSingleton<IEmailServices, EmailServices>()
                .AddHostedService<ServiceInitializer>();
            return collection;
        }
    }
}
