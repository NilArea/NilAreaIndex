using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NilArea.Common.Utils;
using NilArea.Grains.DbContext;
using NilArea.Grains.Repositories;
using ShardingCore;
using ShardingCore.Core.ShardingConfigurations;
using ShardingCore.Sharding.ReadWriteConfigurations;
using StackExchange.Redis;

namespace NilArea.Silo;

public static class Configures
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddNilareaTools(IConfiguration configuration)
        {
            return collection
                .Configure<SnowflakeIdGeneratorOptions>(_ =>
                    configuration.GetSection(nameof(SnowflakeIdGeneratorOptions)))
                .AddSingleton<IIdGenerator<long>, SnowflakeIdGenerator>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        }

        public IServiceCollection AddNilareaDbContext(IConfiguration configuration)
        {
            collection.AddShardingDbContext<NilDbContext>()
                .UseConfig(ConfigDbContext)
                .AddShardingCore();
            collection.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configuration.SafeGetConnectionString("REDIS_CLUSTER")));
            return collection;

            void ConfigDbContext(ShardingConfigOptions op)
            {
                var sqlMaster = configuration.SafeGetConnectionString("MYSQL_MASTER");
                var sqlSlave = configuration.SafeGetConnectionString("MYSQL_SLAVE");
                const string ds0 = "ds0";
                var sqlVersion = new MySqlServerVersion(new Version(9, 5));
                //如何通过字符串查询创建DbContext
                op.UseShardingQuery((conStr, sqb) => { sqb.UseMySql(conStr, sqlVersion); });
                //如何通过事务创建DbContext
                op.UseShardingTransaction((conStr, stb) => { stb.UseMySql(conStr, sqlVersion); });
                op.AddDefaultDataSource(ds0, sqlMaster);
                op.AddReadWriteSeparation(_ => new Dictionary<string, IEnumerable<string>>
                    {
                        { ds0, [sqlSlave] }
                    }, ReadStrategyEnum.Loop, ReadWriteDefaultEnableBehavior.DefaultEnable,
                    10);
            }
        }

        public IServiceCollection AddNilareaRepositories(IConfiguration configuration)
        {
            return collection
                .AddSingleton<IAccountRepository, AccountRepository>();
        }
    }
}
