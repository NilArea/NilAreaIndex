using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NilArea.Common.Utils;
using NilArea.Contracts;
using NilArea.Silo;
using Orleans.Configuration;
using Orleans.Dashboard;

var builder = Host.CreateApplicationBuilder(args);

builder.UseOrleans(siloBuilder =>
{
    var configuration = siloBuilder.Configuration;

    siloBuilder
        .ConfigureServices(services => services
            .AddContractsValidators()
            .AddNilareaTools(configuration)
            .AddNilareaDbContext(configuration)
            .AddNilareaRepositories(configuration))
        .AddDashboard()
        .ConfigureLogging(logging => logging.AddConsole());
#if DEBUG
    siloBuilder
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = configuration.SafeGetConfigureValue("CLUSTER_ID");
            options.ServiceId = configuration.SafeGetConfigureValue("SERVICE_ID");
        })
        .UseLocalhostClustering();
#else
    siloBuilder
        .UseKubernetesHosting();
#endif
});

var host = builder.Build();

await host.RunAsync();
