using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Responses.Account;

namespace NilArea.Contracts.Grains.Account;

/// <summary>
///     权限管理 Grain 接口
/// </summary>
/// <remarks>
///     此接口仅供后端子系统调用，用于管理权限和用户组
///     用户权限由用户所在组和子系统赋予，用户本身不能直接操作权限列表
/// </remarks>
public interface IPermissionGrain : IGrainWithIntegerKey
{
    #region 权限管理

    /// <summary>
    ///     获取所有权限
    /// </summary>
    /// <returns>所有权限列表</returns>
    ValueTask<PermissionInfoResponse[]> GetAllPermissionsAsync();

    /// <summary>
    ///     创建权限
    /// </summary>
    /// <param name="command">创建权限命令</param>
    /// <returns>创建的权限信息</returns>
    ValueTask<PermissionInfoResponse> CreatePermissionAsync(CreatePermissionCommand command);

    /// <summary>
    ///     更新权限
    /// </summary>
    /// <param name="command">更新权限命令</param>
    /// <returns>更新后的权限信息</returns>
    ValueTask<PermissionInfoResponse> UpdatePermissionAsync(UpdatePermissionCommand command);

    /// <summary>
    ///     删除权限
    /// </summary>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否删除成功</returns>
    ValueTask<bool> DeletePermissionAsync(short permissionId);

    #endregion

    #region 用户组管理

    /// <summary>
    ///     获取所有用户组
    /// </summary>
    /// <returns>所有用户组列表</returns>
    ValueTask<GroupInfoResponse[]> GetAllGroupsAsync();

    /// <summary>
    ///     创建用户组
    /// </summary>
    /// <param name="command">创建用户组命令</param>
    /// <returns>创建的用户组信息</returns>
    ValueTask<GroupInfoResponse> CreateGroupAsync(CreateGroupCommand command);

    /// <summary>
    ///     更新用户组
    /// </summary>
    /// <param name="command">更新用户组命令</param>
    /// <returns>更新后的用户组信息</returns>
    ValueTask<GroupInfoResponse> UpdateGroupAsync(UpdateGroupCommand command);

    /// <summary>
    ///     删除用户组
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否删除成功</returns>
    ValueTask<bool> DeleteGroupAsync(int groupId);

    #endregion

    #region 权限分配

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
    ///     为用户添加单独权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否添加成功</returns>
    ValueTask<bool> AddUserPermissionAsync(Guid userId, short permissionId);

    /// <summary>
    ///     从用户移除单独权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否移除成功</returns>
    ValueTask<bool> RemoveUserPermissionAsync(Guid userId, short permissionId);

    #endregion

    #region 查询

    /// <summary>
    ///     获取用户的所有权限（包括直接权限和组权限）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的所有权限</returns>
    ValueTask<PermissionInfoResponse[]> GetUserPermissionsAsync(Guid userId);

    /// <summary>
    ///     获取用户组的所有权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <returns>用户组的所有权限</returns>
    ValueTask<PermissionInfoResponse[]> GetGroupPermissionsAsync(int groupId);

    /// <summary>
    ///     获取用户的所有用户组
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的所有用户组</returns>
    ValueTask<GroupInfoResponse[]> GetUserGroupsAsync(Guid userId);

    /// <summary>
    ///     检查用户是否拥有指定的权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionIds">权限ID列表</param>
    /// <returns>用户是否拥有所有指定的权限</returns>
    ValueTask<bool> CheckUserPermissionsAsync(Guid userId, params ICollection<short> permissionIds);

    #endregion
}