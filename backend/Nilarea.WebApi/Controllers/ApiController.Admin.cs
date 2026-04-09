using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Web.Controllers;

[Route("/api/admin")]
public class ApiAdminController(
    ILogger<ApiAdminController> logger,
    IClusterClient clusterClient,
    IRedisDatabase redisDatabase
) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAccountsAsync()
    {
        return Ok();
    }
}