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
    .AddProblemDetails()
    .AddExceptionHandler<OrleansExceptionHandler>()
    .AddExceptionHandler<ValidationExceptionHandler>()
    .AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddContractsValidators()
    .AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);

builder.Services
    .AddControllers();
builder.Services
    .AddCors(Helpers.CorsSetup);

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

app.MapControllers();

await app.RunAsync();
