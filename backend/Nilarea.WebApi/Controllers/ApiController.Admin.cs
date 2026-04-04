using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NilArea.Web.Authorizations;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Web.Controllers;

[Authorize(Policy = Policies.SystemAdmin)]
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