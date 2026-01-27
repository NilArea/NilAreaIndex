using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace NilArea.Api.Controllers;

public partial class ApiController
{
    [HttpGet("admin")]
    public async Task<IActionResult> GetAccountsAsync()
    {
        return Ok();
    }
}
