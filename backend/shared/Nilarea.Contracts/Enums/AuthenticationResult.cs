namespace NilArea.Contracts.Enums;

[GenerateSerializer]
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