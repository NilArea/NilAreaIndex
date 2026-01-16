namespace NilArea.Contracts.Dto;

/// <summary>
///     用户注册请求
/// </summary>
public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
}

/// <summary>
///     用户注册响应
/// </summary>
public class RegisterResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
///     登录请求
/// </summary>
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

/// <summary>
///     登录响应
/// </summary>
public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }
    public DateTime TokenExpiry { get; set; }
}

/// <summary>
///     更新用户名请求
/// </summary>
public class UpdateUsernameRequest
{
    public string NewUsername { get; set; }
}

/// <summary>
///     邮箱验证请求
/// </summary>
public class VerifyEmailRequest
{
    public string VerificationToken { get; set; }
}

/// <summary>
///     重发验证邮件请求
/// </summary>
public class ResendVerificationRequest
{
    public string Email { get; set; }
}
