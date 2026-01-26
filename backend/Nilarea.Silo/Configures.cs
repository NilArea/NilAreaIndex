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
                .Configure<SnowflakeIdGeneratorOptions>(options =>
                {
                    options.MachineId = configuration.SafeGetConfigureValue<int>("CLUSTER_MACHINE_ID");
                })
                .AddSingleton<IIdGenerator<long>, SnowflakeIdGenerator>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        }

        public IServiceCollection AddNilareaDbContext(IConfiguration configuration)
        {
            collection.AddShardingDbContext<NilDbContext>()
                .UseConfig(ConfigDbContext)
                .AddShardingCore();
            collection.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configuration.SafeGetConfigureValue("REDIS_CLUSTER")));
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

        public IServiceCollection AddNilareaRepositories(IConfiguration configuration)
        {
            return collection
                .AddSingleton<IAccountRepository, AccountRepository>();
        }
    }
}
