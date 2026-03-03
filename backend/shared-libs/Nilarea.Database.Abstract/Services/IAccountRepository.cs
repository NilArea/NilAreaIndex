using NilArea.Common.Services;
using NilArea.Contracts.Dto;
using Nilarea.Database.Abstract.Dto;
using NilArea.Interfaces.IGrains;

namespace Nilarea.Database.Abstract.Services;

public interface IAccountRepository : IAsyncLifetime
{
    ValueTask<bool> ExistsAccountAsync(string email);
    ValueTask CacheConfirmKeyAsync(string email, string key, ConfirmKey keyCode);
    ValueTask<bool> CheckConfirmKeyAsync(string email, string key);
    ValueTask<AccountUserInfo> InsertAccountAsync(string email, string passwordHash, string username);
    ValueTask<AccountUserInfo> FindAccountAsync(Guid uid);
    ValueTask<AccountUserInfo> FindAccountAsync(string email);
    ValueTask<(Guid UserId, string PasswordSaltHash)> GetAccountVerifyKeyAsync(string email);
    ValueTask<TokenPair> GenerateTokenAsync(Guid userId, bool overwrite = true);
    ValueTask<PermissionTagInfo[]> GetAllPermissionAsync(Guid userId);
}
