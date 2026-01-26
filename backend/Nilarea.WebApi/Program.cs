using FluentValidation;
using NilArea.Api.Utils;
using NilArea.Api.Utils.ExceptionHandler;
using NilArea.Api.Utils.Helpers;
using NilArea.Common.Utils;
using NilArea.Contracts;
using Orleans.Configuration;
using Orleans.Dashboard;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.UseOrleansClient(clientBuilder =>
{
    clientBuilder
        .AddDashboard();
#if DEBUG
    clientBuilder
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = configuration.SafeGetConfigureValue("ClusterOptions:ClusterId");
            options.ServiceId = configuration.SafeGetConfigureValue("ClusterOptions:ServiceId");
        })
        .UseLocalhostClustering();
#endif
});

builder.Services
    .AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(configuration.SafeGetConnectionString("REDIS_CLUSTER")))
    .AddSingleton<IRedisDatabaseFactory, RedisDatabaseFactory>();

builder.Services
    .AddContractsValidators()
    .AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);

builder.Services
    .AddProblemDetails()
    .AddExceptionHandler<OrleansExceptionHandler>()
    .AddExceptionHandler<ValidationExceptionHandler>()
    .AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddCors(Helpers.CorsSetup);

builder.Services
    .AddAuthentication();

builder.Services
    .AddControllers();

builder.Services
    .AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapOrleansDashboard("/admin/dashboard");
//    .RequireAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapControllers();

await app.RunAsync();
