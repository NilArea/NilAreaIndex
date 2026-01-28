using System.Runtime.Serialization;

namespace NilArea.Interfaces.Exceptions;

[Serializable]
[GenerateSerializer]
[Alias("NilArea.Interfaces.Exceptions.AccountException")]
public class AccountException : OrleansException
{
    public AccountException(AccountAction action = AccountAction.Default) => AccountAction = action;

    public AccountException(string message, AccountAction action = AccountAction.Default) : base(message) =>
        AccountAction = action;

    public AccountException(string message, Exception inner, AccountAction action = AccountAction.Default) : base(
        message, inner) =>
        AccountAction = action;

    [Obsolete]
    protected AccountException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    [Id(0)] public AccountAction AccountAction { get; init; }
}

public enum AccountAction
{
    Default,
    Register,
    Delete,
    ChangePassword,
    ChangePermissions
}
