using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Hosting;
using NilArea.Blog.Configurations;
using NilArea.Common;
using Orleans.Dashboard;

public class Program
{
    static Program()
    {
    }

    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

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
                    primarySiloEndpoint: new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111),
                    siloPort: 11112,
                    gatewayPort: 30001,
                    clusterId: configuration.SafeGetConfigureValue("CLUSTER_ID"),
                    serviceId: configuration.SafeGetConfigureValue("SERVICE_ID"));
#else
            siloBuilder
                .UseKubernetesHosting();
#endif
        });

        var host = builder.Build();

        await host.RunAsync();
    }

    [ModuleInitializer]
    internal static void Initialization()
    {
    }
}