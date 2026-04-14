namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     令牌存储服务
/// </summary>
public interface ITokenRepository
{
    /// <summary>
    ///     存储刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="expiry">过期时间</param>
    Task StoreRefreshTokenAsync(long userId, string refreshToken, DateTimeOffset expiry);

    /// <summary>
    ///     验证刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="revoke">验证成功是否撤销刷新令牌</param>
    /// <returns>刷新令牌是否有效</returns>
    Task<bool> ValidateRefreshTokenAsync(long userId, string refreshToken, bool revoke = false);

    /// <summary>
    ///     撤销刷新令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    Task RevokeRefreshTokenAsync(long userId, string refreshToken);

    /// <summary>
    ///     撤销所有令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    Task RevokeAllTokensAsync(long userId);
}