using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NilArea.Blog.Infrastructure.Data;
using NilArea.Common;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Contracts.Annotation;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace NilArea.Blog.Configurations;

[EnvironmentVariableNameFormat(Suffix = "_FILE")]
public static class BlogConfigurations
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddDataValidators(IConfiguration configuration)
        {
            return collection
                .AddCommonValidators()
                .AddContractsValidators()
                .AddBlogValidator();
        }

        public IServiceCollection AddNilareaTools(IConfiguration configuration)
        {
            return collection
                .AddSingleton<IIdGenerator<long>, SnowflakeIdGenerator>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        }

        [RequireEnvironmentVariable("MYSQL_CONNECTION_STRING")]
        public IServiceCollection AddNilareaDbContext(IConfiguration configuration)
        {
            collection.AddDbContextPool<BlogDbContext>((sp, builder) =>
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
                .AddHostedService<ServiceInitializer>();
            return collection;
        }
    }
}