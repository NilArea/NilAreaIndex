using NilArea.Account.DTOs;
using NilArea.Contracts.Exceptions;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     权限管理数据库接口
/// </summary>
public interface IPermissionRepository
{
    #region Permission Management

    /// <summary>
    ///     获取所有权限
    /// </summary>
    /// <returns>所有权限列表</returns>
    ValueTask<PermissionTagInfo[]> GetAllPermissionsAsync();

    /// <summary>
    ///     创建权限
    /// </summary>
    /// <param name="permissionName">权限名称</param>
    /// <param name="description">权限描述</param>
    /// <returns>创建的权限信息</returns>
    ValueTask<PermissionTagInfo> CreatePermissionAsync(string permissionName, string description);

    /// <summary>
    ///     更新权限描述
    /// </summary>
    /// <param name="permissionId">权限ID</param>
    /// <param name="description">权限描述</param>
    /// <returns>更新后的权限信息</returns>
    ValueTask<PermissionTagInfo> UpdatePermissionDescriptionAsync(short permissionId, string description);

    /// <summary>
    ///     删除权限
    /// </summary>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否删除成功</returns>
    ValueTask<bool> DeletePermissionAsync(short permissionId);

    /// <summary>
    ///     获取所有用户组
    /// </summary>
    /// <returns>所有用户组列表</returns>
    ValueTask<AccountGroupInfo[]> GetAllGroupsAsync();

    /// <summary>
    ///     创建用户组
    /// </summary>
    /// <param name="groupName">用户组名称</param>
    /// <param name="description">用户组描述</param>
    /// <param name="isSystemGroup">是否为系统组</param>
    /// <returns>创建的用户组信息</returns>
    ValueTask<AccountGroupInfo> CreateGroupAsync(string groupName, string description, bool isSystemGroup);

    /// <summary>
    ///     更新用户组描述
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <param name="description">用户组描述</param>
    /// <returns>更新后的用户组信息</returns>
    ValueTask<AccountGroupInfo> UpdateGroupDescriptionAsync(int groupId, string description);

    /// <summary>
    ///     删除用户组
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否删除成功</returns>
    ValueTask<bool> DeleteGroupAsync(int groupId);

    /// <summary>
    ///     为用户组分配权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否分配成功</returns>
    ValueTask<bool> AssignPermissionToGroupAsync(int groupId, short permissionId);

    /// <summary>
    ///     从用户组移除权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否移除成功</returns>
    ValueTask<bool> RemovePermissionFromGroupAsync(int groupId, short permissionId);

    /// <summary>
    ///     将用户添加到用户组
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否添加成功</returns>
    ValueTask<bool> AddUserToGroupAsync(Guid userId, int groupId);

    /// <summary>
    ///     将用户从用户组移除
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否移除成功</returns>
    ValueTask<bool> RemoveUserFromGroupAsync(Guid userId, int groupId);

    /// <summary>
    ///     为用户直接分配权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否分配成功</returns>
    ValueTask<bool> AssignPermissionToUserAsync(Guid userId, short permissionId);

    /// <summary>
    ///     从用户移除直接权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否移除成功</returns>
    ValueTask<bool> RemovePermissionFromUserAsync(Guid userId, short permissionId);

    /// <summary>
    ///     获取用户组的所有权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <returns>用户组的所有权限</returns>
    ValueTask<PermissionTagInfo[]> GetGroupPermissionsAsync(int groupId);

    /// <summary>
    ///     获取用户的所有用户组
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的所有用户组</returns>
    ValueTask<AccountGroupInfo[]> GetUserGroupsAsync(Guid userId);

    /// <summary>
    ///     根据用户ID获取所有权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的所有权限</returns>
    /// <exception cref="AccountException">账号不可用时抛出</exception>
    ValueTask<PermissionTagInfo[]> GetAllPermissionAsync(Guid userId);

    #endregion
}