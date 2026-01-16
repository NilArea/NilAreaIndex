using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans(siloBuilder =>
{
    siloBuilder
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
