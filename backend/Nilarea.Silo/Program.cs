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
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = configuration.SafeGetConfigureValue("ClusterOptions:ClusterId");
            options.ServiceId = configuration.SafeGetConfigureValue("ClusterOptions:ServiceId");
        })
        .UseLocalhostClustering()
        .AddDashboard()
        .ConfigureLogging(logging => logging.AddConsole());
});

var host = builder.Build();

await host.RunAsync();
