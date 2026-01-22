using FluentValidation;
using NilArea.Api.Utils.ExceptionHandler;
using NilArea.Api.Utils.Helpers;
using NilArea.Common.Utils;
using NilArea.Contracts;
using Orleans.Configuration;

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
        .UseLocalhostClustering();
});

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
    .AddControllers();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapControllers();

await app.RunAsync();
