using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NilArea.Common;
using NilArea.Common.Services;
using NilArea.Contracts.Annotation;

namespace NilArea.Account.Infrastructure.Services;

/// <summary>
///     令牌管理服务
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///     生成访问令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="email">用户邮箱</param>
    /// <returns>访问令牌</returns>
    (string AccessToken, DateTimeOffset AccessTokenExpiry) GenerateAccessToken(long userId, string email);

    /// <summary>
    ///     生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    (string RefreshToken, DateTimeOffset RefreshTokenExpiry) GenerateRefreshToken();

    /// <summary>
    ///     验证令牌
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <returns>令牌的声明主体</returns>
    /// <exception cref="SecurityTokenException">令牌无效时抛出</exception>
    ClaimsPrincipal ValidateAccessToken(string accessToken);

    /// <summary>
    ///     从令牌中获取用户ID
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <returns>用户ID</returns>
    /// <exception cref="InvalidOperationException">令牌无效时抛出</exception>
    long GetUserIdFromAccessToken(string accessToken);
}

/// <summary>
///     令牌管理服务实现
/// </summary>
public class TokenService(
    ILogger<TokenService> logger,
    IConfiguration configuration
) : ITokenService, IAsyncLifetime
{
    [RequireEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRY_MINUTES", DefaultValue = "15")]
    private readonly int _accessTokenExpiryMinutes =
        configuration.SafeGetConfigureValue("JWT_ACCESS_TOKEN_EXPIRY_MINUTES", 15);

    [RequireEnvironmentVariable("JWT_AUDIENCE", ErrorMessage = "Jwt:Audience is required")]
    [EnvironmentVariableNameFormat(Suffix = "_FILE")]
    private readonly string _audience = configuration.GetSecretFromFile("JWT_AUDIENCE");

    [RequireEnvironmentVariable("JWT_ISSUER", ErrorMessage = "Jwt:Issuer is required")]
    [EnvironmentVariableNameFormat(Suffix = "_FILE")]
    private readonly string _issuer = configuration.GetSecretFromFile("JWT_ISSUER");

    [RequireEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRY_DAYS", DefaultValue = "3")]
    private readonly int _refreshTokenExpiryDays =
        configuration.SafeGetConfigureValue("JWT_REFRESH_TOKEN_EXPIRY_DAYS", 3);

    [RequireEnvironmentVariable("JWT_SECRET_KEY", ErrorMessage = "Jwt:SecretKey is required")]
    [EnvironmentVariableNameFormat(Suffix = "_FILE")]
    private readonly string _secretKey = configuration.GetSecretFromFile("JWT_SECRET_KEY");

    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing token service...");
    }

    public async Task DisposeAsync()
    {
        logger.LogInformation("Disposing token service...");
    }

    /// <summary>
    ///     生成访问令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="email">用户邮箱</param>
    /// <returns>访问令牌</returns>
    public (string AccessToken, DateTimeOffset AccessTokenExpiry) GenerateAccessToken(long userId, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiry = DateTimeOffset.UtcNow.AddMinutes(_accessTokenExpiryMinutes);
        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: expiry.DateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }

    /// <summary>
    ///     生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    public (string RefreshToken, DateTimeOffset RefreshTokenExpiry) GenerateRefreshToken()
    {
        return (Guid.NewGuid().ToString(), DateTimeOffset.UtcNow.AddDays(_refreshTokenExpiryDays));
    }

    /// <summary>
    ///     验证令牌
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <returns>令牌的声明主体</returns>
    /// <exception cref="SecurityTokenException">令牌无效时抛出</exception>
    public ClaimsPrincipal ValidateAccessToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = key
        };

        return tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out _);
    }

    /// <summary>
    ///     从令牌中获取用户ID
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <returns>用户ID</returns>
    /// <exception cref="InvalidOperationException">令牌无效时抛出</exception>
    public long GetUserIdFromAccessToken(string accessToken)
    {
        var principal = ValidateAccessToken(accessToken);
        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
        return long.Parse(userIdClaim?.Value ??
                          throw new InvalidOperationException("Invalid token: missing userId claim"));
    }
}