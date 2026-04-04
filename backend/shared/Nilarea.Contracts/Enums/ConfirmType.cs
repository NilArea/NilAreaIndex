namespace NilArea.Contracts.Enums;

/// <summary>
///     验证码作用类型<br />
///     <see cref="Default" />默认类型或其他类型<br />
///     <see cref="Initial" />初始化操作<br />
///     <see cref="Reversible" />可逆操作<br />
///     <see cref="Irreversible" />不可逆操作<br />
/// </summary>
[GenerateSerializer]
public enum ConfirmType
{
    Default,
    Initial,
    Reversible,
    Irreversible
}