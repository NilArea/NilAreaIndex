using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
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
        builder.ConfigureAppConfiguration((context, config) => { config.AddEnvironmentVariables("NA_"); });

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
    }
}