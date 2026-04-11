using System.Net;
using Microsoft.Extensions.Hosting;
using NilArea.Blog.Configurations;
using NilArea.Common;
using NilArea.Contracts.Annotation;
using Orleans.Dashboard;

namespace NilArea.Blog;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();
        await host.RunAsync();
    }

    [EnvironmentVariableNameFormat(Prefix = "NIL_")]
    [RequireEnvironmentVariable("CLUSTER_ID", DefaultValue = "nilarea-cluster")]
    [RequireEnvironmentVariable("SERVICE_ID", DefaultValue = "nilarea-blog")]
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
        return builder;
    }
}