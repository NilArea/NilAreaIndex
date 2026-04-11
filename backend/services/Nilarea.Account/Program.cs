using Microsoft.Extensions.Hosting;
using NilArea.Account.Configurations;
using NilArea.Common;
using NilArea.Contracts.Annotation;
using Orleans.Dashboard;

namespace NilArea.Account;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();
        await host.RunAsync();
    }

    [RequireEnvironmentVariable("CLUSTER_ID", DefaultValue = "nilarea-cluster")]
    [RequireEnvironmentVariable("SERVICE_ID", DefaultValue = "nilarea-account")]
    [EnvironmentVariableNameFormat(Prefix = "NIL_", Priority = 1)]
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
}