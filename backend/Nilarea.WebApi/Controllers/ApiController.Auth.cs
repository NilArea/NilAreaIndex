using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NilArea.Contracts;
using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Enums;
using NilArea.Contracts.Grains.Account;
using NilArea.Contracts.Responses.Account;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace NilArea.Web.Controllers;

[AllowAnonymous]
[Route("/api/auth")]
public class ApiAuthController(
    ILogger<ApiAuthController> logger,
    IClusterClient clusterClient,
    IRedisDatabase redisDatabase,
    IValidator<RegisterAccountCommand> registerRequestValidator,
    IValidator<LoginAccountCommand> loginRequestValidator
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
        var ag = clusterClient.GetGrain<IAccountGrain>(0);
        await ag.CallConfirmKey(email, ConfirmType.Initial);
        return NoContent();
    }

    /// <summary>
    ///     用户注册（使用邮箱）
    /// </summary>
    /// <param name="command">注册信息</param>
    /// <returns>注册结果</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RegisterAccountResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterAccountCommand command)
    {
        await registerRequestValidator.ValidateAndThrowAsync(command);
        var ag = clusterClient.GetGrain<IAccountGrain>(0);
        if (await ag.ExistAccountAsync(command.Email))
            return BadRequest("Email already registered");
        return Ok(await ag.RegisterUserAsync(command));
    }

    /// <summary>
    ///     用户登录
    /// </summary>
    /// <param name="command">登录凭证</param>
    /// <returns>登录结果和令牌</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LoginAccountResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginAccountCommand command)
    {
        var validate = await loginRequestValidator.ValidateAsync(command);
        if (!validate.IsValid)
            throw new ValidationException(validate.Errors);
        if (!await redisDatabase.Database.BloomExistsAsync(BfAccount, command.Email))
            return BadRequest("Email is not registered");
        var ag = clusterClient.GetGrain<IAuthenticationGrain>(0);
        return Ok(await ag.LoginAsync(command));
    }
}