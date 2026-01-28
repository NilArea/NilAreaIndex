using StackExchange.Redis;

namespace NilArea.Contracts;

public static class StaticValues
{
    public static readonly RedisKey BfAccount = "BF:Account";

    /// <summary>
    ///     NilArea:ConfirmKey
    /// </summary>
    public static readonly RedisKey ConfirmKeyPrefix = "NA:CK";

    public static class AccountSystemGroupNames
    {
        public const string User = "User";
        public const string Admin = "Group";
        public static string[] SystemGroupNames = [User, Admin];
    }
}
