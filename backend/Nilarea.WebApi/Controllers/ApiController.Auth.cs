using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NilArea.Contracts;
using NilArea.Contracts.Dto;
using NilArea.Interfaces.IGrains;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Web.Controllers;

[AllowAnonymous]
[Route("/api/auth")]
public class ApiAuthController(
    ILogger<ApiAuthController> logger,
    IClusterClient clusterClient,
    IRedisDatabase redisDatabase,
    IValidator<AccountRegisterRequest> registerRequestValidator,
    IValidator<AccountLoginRequest> loginRequestValidator
) : ControllerBase
{
    private static readonly InlineValidator<string> EmailValidator;

    static ApiAuthController()
    {
        var validator = new InlineValidator<string>();
        validator.RuleFor(x => x)
            .NotEmpty()
            .EmailAddress();
        EmailValidator = validator;
    }

    private static RedisKey BfAccount => StaticValues.BfAccount;

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("register/callconfirm")]
    public async Task<IActionResult> RegisterConfirm([FromQuery] string email)
    {
        await EmailValidator.ValidateAndThrowAsync(email);
        var ag = clusterClient.GetGrain<IAccountGrain>(Guid.Empty);
        await ag.CallConfirmKey(email, ConfirmType.Initial);
        return NoContent();
    }

    /// <summary>
    ///     用户注册（使用邮箱）
    /// </summary>
    /// <param name="request">注册信息</param>
    /// <returns>注册结果</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AccountRegisterResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] AccountRegisterRequest request)
    {
        await registerRequestValidator.ValidateAndThrowAsync(request);
        var ag = clusterClient.GetGrain<IAccountGrain>(Guid.Empty);
        if (await ag.ExistAccountAsync(request.Email))
            return BadRequest("Email already registered");
        return Ok(await ag.RegisterUserAsync(request));
    }

    /// <summary>
    ///     用户登录
    /// </summary>
    /// <param name="request">登录凭证</param>
    /// <returns>登录结果和令牌</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AccountLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AccountLoginResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] AccountLoginRequest request)
    {
        var validate = await loginRequestValidator.ValidateAsync(request);
        if (!validate.IsValid)
            throw new ValidationException(validate.Errors);
        if (!await redisDatabase.Database.BloomExistsAsync(BfAccount, request.Email))
            return BadRequest("Email is not registered");
        var ag = clusterClient.GetGrain<IAuthenticationGrain>(Guid.Empty);
        return Ok(await ag.LoginAsync(request));
    }
}
