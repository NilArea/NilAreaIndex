using StackExchange.Redis;

namespace NilArea.Contracts;

public static class StaticValues
{
    public static readonly RedisKey BfAcount = "BF:Account";

    public static class AccountSystemGroupNames
    {
        public const string User = "User";
        public const string Admin = "Group";
        public static string[] SystemGroupNames = [User, Admin];
    }
}
