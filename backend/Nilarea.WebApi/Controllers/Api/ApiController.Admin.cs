using Microsoft.AspNetCore.Mvc;
using NilArea.Api.Utils;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace NilArea.Api.Controllers;

[Route("/api/admin")]
public class ApiAdminController(
    ILogger<ApiAdminController> logger,
    IClusterClient clusterClient,
    IRedisDatabaseFactory redisDatabaseFactory
) : ControllerBase
{
    private IDatabase ReadonlyRedis { get; } = redisDatabaseFactory.GetDatabase();

    [HttpGet("")]
    public async Task<IActionResult> GetAccountsAsync()
    {
        return Ok();
    }
}
