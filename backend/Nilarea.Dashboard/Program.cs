using NilArea.Common.Utils;
using Orleans.Configuration;
using Orleans.Dashboard;

var builder = WebApplication.CreateBuilder(args);


builder.UseOrleansClient(clientBuilder =>
{
    var configuration = clientBuilder.Configuration;
    clientBuilder
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = configuration.SafeGetConfigureValue("ClusterOptions:ClusterId");
            options.ServiceId = configuration.SafeGetConfigureValue("ClusterOptions:ServiceId");
        })
        .UseLocalhostClustering()
        .AddDashboard();
});

var app = builder.Build();

app.MapOrleansDashboard();

await app.RunAsync();
