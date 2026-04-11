using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NilArea.Common;
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
    string GenerateAccessToken(Guid userId, string email);

    /// <summary>
    ///     生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    string GenerateRefreshToken();

    /// <summary>
    ///     验证令牌
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <returns>令牌的声明主体</returns>
    /// <exception cref="SecurityTokenException">令牌无效时抛出</exception>
    ClaimsPrincipal ValidateToken(string token);

    /// <summary>
    ///     从令牌中获取用户ID
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <returns>用户ID</returns>
    /// <exception cref="InvalidOperationException">令牌无效时抛出</exception>
    Guid GetUserIdFromToken(string token);
}

/// <summary>
///     令牌管理服务实现
/// </summary>
[EnvironmentVariableNameFormat(Suffix = "_FILE")]
public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly int _accessTokenExpiryMinutes =
        int.Parse(configuration["JWT_ACCESS_TOKEN_EXPIRY_MINUTES"] ?? "15");

    [RequireEnvironmentVariable("JWT_AUDIENCE", ErrorMessage = "Jwt:Audience is required")]
    private readonly string _audience = configuration.GetSecretFromFile("JWT_AUDIENCE");

    [RequireEnvironmentVariable("JWT_ISSUER", ErrorMessage = "Jwt:Issuer is required")]
    private readonly string _issuer = configuration.GetSecretFromFile("JWT_ISSUER");

    [RequireEnvironmentVariable("JWT_SECRET_KEY", ErrorMessage = "Jwt:SecretKey is required", FailFast = false)]
    private readonly string _secretKey = configuration.GetSecretFromFile("JWT_SECRET_KEY");

    /// <summary>
    ///     生成访问令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="email">用户邮箱</param>
    /// <returns>访问令牌</returns>
    public string GenerateAccessToken(Guid userId, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    ///     生成刷新令牌
    /// </summary>
    /// <returns>刷新令牌</returns>
    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    ///     验证令牌
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <returns>令牌的声明主体</returns>
    /// <exception cref="SecurityTokenException">令牌无效时抛出</exception>
    public ClaimsPrincipal ValidateToken(string token)
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

        return tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
    }

    /// <summary>
    ///     从令牌中获取用户ID
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <returns>用户ID</returns>
    /// <exception cref="InvalidOperationException">令牌无效时抛出</exception>
    public Guid GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
        return Guid.Parse(userIdClaim?.Value ??
                          throw new InvalidOperationException("Invalid token: missing userId claim"));
    }
}