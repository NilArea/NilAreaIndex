using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Web.Controllers;

[AllowAnonymous]
[Route("api/blog")]
public class ApiBlogController(
    ILogger<ApiBlogController> logger,
    IClusterClient clusterClient,
    IRedisDatabase redisDatabase,
    IOpenSearchClient openSearchClient
) : ControllerBase
{
    [HttpPost("search")]
    [ProducesResponseType(typeof(int), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SearchBlogAsync([FromQuery(Name = "q")] string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return BadRequest(34);
        var uid = Guid.NewGuid().ToString();
        return AcceptedAtAction(nameof(GetBlogAsync), new { }, new { uid });
    }

    [HttpGet("search/{id}")]
    public async Task<IActionResult> GetBlogAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest(12);
        return Ok();
    }
}
