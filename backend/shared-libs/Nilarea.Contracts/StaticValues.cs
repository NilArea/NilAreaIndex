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
        public const string Users = "Users";
        public const string Administrators = "Administrators";
        public const string PowerUsers = "PowerUsers";
        public const string Guests = "Guests";
        public const string BackupOperators = "BackupOperators";
        public static string[] SystemGroupNames = [Administrators, PowerUsers, Users, Guests, BackupOperators];
    }
}
