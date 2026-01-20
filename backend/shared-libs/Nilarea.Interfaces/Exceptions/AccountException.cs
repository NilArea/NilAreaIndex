using Orleans;
using Orleans.Runtime;

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

    [Id(0)] public AccountAction AccountAction { get; init; }
}

public enum AccountAction
{
    Default,
    Register,
    Login,
    ChangePassword,
    VerifyAccount
}
