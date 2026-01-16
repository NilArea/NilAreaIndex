using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NilArea.Contracts.Dto;
using NilArea.Interfaces.IGrains;

namespace NilArea.Api.Controllers;

[Route("api/[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    IClusterClient clusterClient,
    IValidator<RegisterRequest> registerRequestValidator
) : ControllerBase
{


    // ============ 接口定义 ============

    /// <summary>
    /// 用户注册（使用邮箱）
    /// </summary>
    /// <param name="request">注册信息</param>
    /// <returns>注册结果</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await registerRequestValidator.ValidateAsync(request);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
        var ag = clusterClient.GetGrain<IAccountGrain>(Guid.NewGuid());
        // 冥等实现：验证邮箱格式、检查邮箱是否已注册、创建用户记录
        // 返回用户ID（唯一）、邮箱、初始用户名
        return Ok(new RegisterResponse());
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录凭证</param>
    /// <returns>登录结果和令牌</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(401)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // 冥等实现：验证邮箱和密码、检查邮箱是否已验证、生成JWT令牌
        return Ok(new LoginResponse());
    }

    /// <summary>
    /// 验证邮箱
    /// </summary>
    /// <param name="request">验证令牌</param>
    /// <returns>验证结果</returns>
    [HttpPost("verify-email")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        // 冥等实现：验证令牌有效性、更新邮箱验证状态
        return Ok();
    }

    /// <summary>
    /// 重发验证邮件
    /// </summary>
    /// <param name="request">邮箱地址</param>
    /// <returns>操作结果</returns>
    [HttpPost("resend-verification")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult ResendVerification([FromBody] ResendVerificationRequest request)
    {
        // 冥等实现：检查邮箱是否存在且未验证、生成新令牌、发送验证邮件
        return Ok();
    }

    /// <summary>
    /// 更新用户名（需要认证）
    /// </summary>
    /// <param name="request">新用户名</param>
    /// <returns>更新结果</returns>
    [HttpPut("username")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public IActionResult UpdateUsername([FromBody] UpdateUsernameRequest request)
    {
        // 冥等实现：验证用户身份（通过JWT）、更新用户名
        // 注意：用户名可以随意更改，但需要检查唯一性（如果系统要求）
        return Ok();
    }

    /// <summary>
    /// 获取当前用户信息（需要认证）
    /// </summary>
    /// <returns>用户信息</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(RegisterResponse), 200)]
    [ProducesResponseType(401)]
    public IActionResult GetCurrentUser()
    {
        // 冥等实现：从JWT中获取用户ID、查询并返回用户信息
        return Ok(new RegisterResponse());
    }
}
