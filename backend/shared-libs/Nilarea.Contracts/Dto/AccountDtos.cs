using Orleans;

namespace NilArea.Contracts.Dto;

/// <summary>
///     用户注册请求
/// </summary>
[GenerateSerializer]
[Alias("NilArea.Contracts.Dto.RegisterRequest")]
public class RegisterRequest
{
    [Id(0)] public string Email { get; set; } = string.Empty;
    [Id(1)] public string Password { get; set; } = string.Empty;
    [Id(2)] public string Username { get; set; } = string.Empty;
}

/// <summary>
///     用户注册响应
/// </summary>
[GenerateSerializer]
[Alias("NilArea.Contracts.Dto.RegisterResponse")]
public class RegisterResponse
{
    [Id(0)] public long UserId { get; set; }
    [Id(1)] public string Email { get; set; } = string.Empty;
    [Id(2)] public string Username { get; set; } = string.Empty;
    [Id(3)] public DateTime CreatedAt { get; set; }
}

/// <summary>
///     登录请求
/// </summary>
[GenerateSerializer]
[Alias("NilArea.Contracts.Dto.LoginRequest")]
public class LoginRequest
{
    [Id(0)] public string Email { get; set; } = string.Empty;
    [Id(1)] public string Password { get; set; } = string.Empty;
}

/// <summary>
///     登录响应
/// </summary>
[GenerateSerializer]
[Alias("NilArea.Contracts.Dto.LoginResponse")]
public class LoginResponse
{
    [Id(0)] public long UserId { get; set; }
    [Id(1)] public string AccessToken { get; set; } = string.Empty;
    [Id(2)] public DateTime AccessTokenExpiry { get; set; }
    [Id(3)] public string RefreshToken { get; set; } = string.Empty;
    [Id(4)] public DateTime RefreshTokenExpiry { get; set; }
}

/// <summary>
///     更新用户名请求
/// </summary>
public class UpdateUsernameRequest
{
    public string NewUsername { get; set; } = string.Empty;
}

/// <summary>
///     邮箱验证请求
/// </summary>
public class VerifyEmailRequest
{
    public string VerificationToken { get; set; } = string.Empty;
}

/// <summary>
///     重发验证邮件请求
/// </summary>
public class ResendVerificationRequest
{
    public string Email { get; set; } = string.Empty;
}
