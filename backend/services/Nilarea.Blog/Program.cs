using Microsoft.Extensions.Hosting;
using NilArea.Blog.Configurations;
using NilArea.Common;
using Orleans.Configuration;
using Orleans.Dashboard;

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