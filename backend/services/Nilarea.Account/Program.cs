using System.Runtime.CompilerServices;
using Microsoft.Extensions.Hosting;
using NilArea.Account.Configurations;
using NilArea.Common;
using Orleans.Dashboard;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.UseOrleans(siloBuilder =>
        {
            var configuration = siloBuilder.Configuration;

            siloBuilder
                .ConfigureServices(services => services
                    .AddDataValidators(configuration)
                    .AddNilareaTools(configuration)
                    .AddNilareaDbContext(configuration)
                    .AddNilareaCache(configuration)
                    .AddNilareaServices(configuration))
                .AddDashboard();
#if DEBUG
            siloBuilder
                .UseLocalhostClustering(
                    primarySiloEndpoint: null,
                    siloPort: 11111,
                    gatewayPort: 30000,
                    clusterId: configuration.SafeGetConfigureValue("CLUSTER_ID"),
                    serviceId: configuration.SafeGetConfigureValue("SERVICE_ID"));
#else
            siloBuilder
                .UseKubernetesHosting();
#endif
        });

        return builder;
    }

    [ModuleInitializer]
    internal static void Initialization()
    {
        // 校验JWT相关环境变量
        ValidateEnvironmentVariable("JWT_AUDIENCE", "Jwt:Audience");
        ValidateEnvironmentVariable("JWT_ISSUER", "Jwt:Issuer");
        ValidateEnvironmentVariable("JWT_SECRET_KEY", "Jwt:SecretKey");
        ValidateEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRY_MINUTES", "Jwt:AccessTokenExpiryMinutes", false);

        // 校验集群相关环境变量
        ValidateEnvironmentVariable("CLUSTER_ID", "CLUSTER_ID");
        ValidateEnvironmentVariable("SERVICE_ID", "SERVICE_ID");
    }

    private static void ValidateEnvironmentVariable(string envName, string configName = null, bool failFast = true)
    {
        var value = Environment.GetEnvironmentVariable(envName);
        if (string.IsNullOrWhiteSpace(value))
        {
            var err = new InvalidOperationException($"环境变量 {envName} (对应配置 {configName}) 未设置或为空");
            if (failFast)
                throw err;
            else
                Console.Error.WriteLine(err);
        }
    }
}