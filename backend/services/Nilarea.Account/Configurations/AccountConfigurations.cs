using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NilArea.Account.Infrastructure.Data;
using NilArea.Account.Infrastructure.Repositories;
using NilArea.Account.Infrastructure.Services;
using NilArea.Common;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Contracts.Annotation;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace NilArea.Account.Configurations;

[EnvironmentVariableNameFormat(Suffix = "_FILE")]
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

        [RequireEnvironmentVariable("MYSQL_CONNECTION_STRING")]
        public IServiceCollection AddNilareaDbContext(IConfiguration configuration)
        {
            collection.AddDbContextPool<AccountDbContext>((sp, builder) =>
            {
                var connectionString = configuration.GetSecretFromFile("MYSQL_CONNECTION_STRING");
                builder.UseMySQL(connectionString);
            });
            return collection;
        }

        [RequireEnvironmentVariable("REDIS_CLUSTER")]
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
                    ConnectionString = configuration.GetSecretFromFile("REDIS_CLUSTER")
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