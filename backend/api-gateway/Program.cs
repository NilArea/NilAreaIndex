using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.UseOrleansClient(clientBuilder =>
{
    clientBuilder
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "nilarea-cluster";
            options.ServiceId = "nilarea-service";
        })
        .UseLocalhostClustering();
});

builder.Services
    .AddControllers();

var app = builder.Build();

app.MapControllers();

await app.RunAsync();
