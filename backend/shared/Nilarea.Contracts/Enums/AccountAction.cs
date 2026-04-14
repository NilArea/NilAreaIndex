namespace NilArea.Contracts.Enums;

[GenerateSerializer]
public enum AccountAction
{
    Default,
    Register,
    Delete,
    ChangePassword,
    ChangeEmail,
    Update
}