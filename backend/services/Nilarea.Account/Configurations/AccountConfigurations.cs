using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NilArea.Account.Infrastructure.Data;
using NilArea.Account.Infrastructure.Repositories;
using NilArea.Account.Infrastructure.Services;
using NilArea.Common;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Contracts.Annotation;
using Orleans.Configuration;
using Orleans.Storage;
using StackExchange.Redis;
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
                .AddSingleton<IIdGenerator<long>, SnowflakeIdGenerator>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        }

        public IServiceCollection AddNilareaServices(IConfiguration configuration)
        {
            collection
                .AddScoped<IAccountRepository, AccountRepository>()
                .AddScoped<IConfirmRepository, ConfirmRepository>()
                .AddScoped<ITokenRepository, TokenRepository>()
                .AddAsyncLifetimeSingleton<IEmailServices, EmailServices>()
                .AddAsyncLifetimeSingleton<ITokenService, TokenService>()
                .AddAsyncLifetimeSingleton<IBloomFilterServices, BloomFilterServices>()
                .AddHostedService<ServiceInitializer>();
            return collection;
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddStackExchangeRedisExtensions<T>(IEnumerable<RedisConfiguration> redisConfiguration)
            where T : class, ISerializer
        {
            return services.AddStackExchangeRedisExtensions<T>(sp => redisConfiguration);
        }

        public IServiceCollection AddStackExchangeRedisExtensions<T>(
            Func<IServiceProvider, IEnumerable<RedisConfiguration>> redisConfigurationFactory)
            where T : class, ISerializer
        {
            services.AddSingleton<IRedisClientFactory, RedisClientFactory>();
            services.AddSingleton<ISerializer, T>();
            services.AddSingleton(provider =>
                provider.GetRequiredService<IRedisClientFactory>().GetDefaultRedisClient());
            services.AddSingleton<IRedisDatabase>(provider =>
                provider.GetRequiredService<IRedisClientFactory>().GetDefaultRedisClient().GetDefaultDatabase());
            services.AddSingleton(redisConfigurationFactory);
            return services;
        }
    }

    extension(ISiloBuilder siloBuilder)
    {
        [RequireEnvironmentVariable("MYSQL_CONNECTION_STRING")]
        [RequireEnvironmentVariable("REDIS_CLUSTER")]
        [EnvironmentVariableNameFormat(Suffix = "_FILE")]
        public ISiloBuilder ConfigureStorage()
        {
            const string invariant = "MySql.Data.MySqlClient";
            var mysqlConnectionString = siloBuilder.Configuration.GetSecretFromFile("MYSQL_CONNECTION_STRING");
            // 配置Orleans持久化存储
            Action<OptionsBuilder<AdoNetGrainStorageOptions>> builder = builder =>
            {
                builder.Configure<IGrainStorageSerializer>((options, serializer) =>
                {
                    options.Invariant = invariant;
                    options.ConnectionString = mysqlConnectionString;
                    options.GrainStorageSerializer = serializer;
                });
            };
            siloBuilder.AddAdoNetGrainStorageAsDefault(builder);
            siloBuilder.AddAdoNetGrainStorage("OrleansStorage", builder);
            // 配置Orleans提醒服务
            siloBuilder.UseAdoNetReminderService(options =>
            {
                options.Invariant = invariant;
                options.ConnectionString = mysqlConnectionString;
            });
            // 配置EFCore上下文
            siloBuilder.Services.AddDbContextFactory<AccountDbContext>((sp, factory) =>
            {
                factory.UseMySQL(mysqlConnectionString);
            });
            var redisConnectionString = siloBuilder.Configuration.GetSecretFromFile("REDIS_CLUSTER");
            // 配置Orleans集群发现
            siloBuilder.UseRedisClustering(options =>
            {
                options.ConfigurationOptions = ConfigurationOptions.Parse(redisConnectionString);
            });
            // 配置Orleans缓存
            siloBuilder.Services
                .AddStackExchangeRedisExtensions<NewtonsoftSerializer>(sp =>
                {
                    var conf = new RedisConfiguration
                    {
                        ConnectionString = redisConnectionString
                    };
                    return [conf];
                });
            return siloBuilder;
        }
    }
}