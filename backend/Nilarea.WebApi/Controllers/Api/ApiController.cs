using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NilArea.Api.Utils;
using NilArea.Contracts.Dto;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace NilArea.Api.Controllers;

[Route("/api")]
public partial class ApiController(
    ILogger<ApiController> logger,
    IClusterClient clusterClient,
    IRedisDatabaseFactory redisDatabaseFactory,
    IValidator<RegisterRequest> registerRequestValidator,
    IValidator<LoginRequest> loginRequestValidator
) : ControllerBase
{
    private IDatabase ReadonlyRedis { get; } = redisDatabaseFactory.GetDatabase();
}
