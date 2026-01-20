using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Grains.DbContext;
using NilArea.Grains.Repositories;
using Orleans.Configuration;
using StackExchange.Redis;

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans(siloBuilder =>
{
    siloBuilder
        .ConfigureServices(services =>
        {
            services
                .AddContractsValidators()
                //Tools
                .AddOptions<SnowflakeIdGeneratorOptions>().Services
                .AddSingleton<IIdGenerator<long>, SnowflakeIdGenerator>()
                .AddSingleton<IPasswordHasher, BCryptPasswordHasher>()
                //DbContext
                .AddSingleton<NilDbContext>()
                //Redis
                .AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(sp =>
                {
                    var cfg = siloBuilder.Configuration;
                    return ConnectionMultiplexer.Connect(cfg.SafeGetConfigureValue("REDIS_CONNECTION_STRING"));
                })
                //Repository
                .AddSingleton<IAccountRepository, AccountRepository>();
        })
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "nilarea-cluster";
            options.ServiceId = "nilarea-service";
        })
        .UseLocalhostClustering()
        .ConfigureLogging(logging => logging.AddConsole());
});

var host = builder.Build();

await host.RunAsync();
