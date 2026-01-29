using System.Runtime.Serialization;

namespace NilArea.Interfaces.Exceptions;

[Serializable]
[GenerateSerializer]
[Alias("NilArea.Interfaces.Exceptions.AuthenticationException")]
public class AuthenticationException : OrleansException
{
    public AuthenticationException(AuthenticationResult result = AuthenticationResult.Default) => Result = result;

    public AuthenticationException(string message, AuthenticationResult result = AuthenticationResult.Default) :
        base(message) =>
        Result = result;

    public AuthenticationException(string message, Exception innerException,
        AuthenticationResult result = AuthenticationResult.Default) : base(message, innerException) =>
        Result = result;

    [Obsolete]
    protected AuthenticationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    [Id(0)] public AuthenticationResult Result { get; set; }
}

public enum AuthenticationResult
{
    /// 默认状态/未定义
    Default,

    /// 认证失败（用户名或密码错误）
    Failed,

    ///未提供凭据
    Unauthorized,

    ///权限不足
    Forbidden
}
