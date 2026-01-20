using FluentValidation;
using NilArea.Api.Utils.ExceptionHandler;
using NilArea.Api.Utils.Helpers;
using NilArea.Contracts;
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

app.MapControllers();

await app.RunAsync();
