using Microsoft.Extensions.Hosting;
using NilArea.Account.Configurations;
using NilArea.Common;
using NilArea.Contracts.Annotation;
using Orleans.Configuration;
using Orleans.Dashboard;

var builder = CreateHostBuilder(args);
var host = builder.Build();
await host.RunAsync();


[RequireEnvironmentVariable("CLUSTER_ID", DefaultValue = "nilarea")]
[RequireEnvironmentVariable("SERVICE_ID", DefaultValue = "default")]
[EnvironmentVariableNameFormat(Prefix = "NIL_")]
static IHostBuilder CreateHostBuilder(string[] args)
{
    var builder = Host.CreateDefaultBuilder(args);

    builder.UseOrleans(siloBuilder =>
    {
        var configuration = siloBuilder.Configuration;

        siloBuilder
            .ConfigureServices(services => services
                .AddDataValidators(configuration)
                .AddNilareaTools(configuration)
                .AddNilareaServices(configuration))
            .ConfigureStorage()
            .AddDashboard();
#if DEBUG
        siloBuilder
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = configuration.SafeGetConfigureValue("CLUSTER_ID");
                options.ServiceId = configuration.SafeGetConfigureValue("SERVICE_ID");
            })
            .Configure<EndpointOptions>(options =>
            {
                options.GatewayPort = 30000;
                options.SiloPort = 11111;
            });
#else
        siloBuilder
            .UseKubernetesHosting();
#endif
    });

    return builder;
}