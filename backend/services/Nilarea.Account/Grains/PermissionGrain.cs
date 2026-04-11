using FluentValidation;
using Microsoft.Extensions.Logging;
using NilArea.Account.Infrastructure.Repositories;
using NilArea.Contracts.Commands.Account;
using NilArea.Contracts.Exceptions;
using NilArea.Contracts.Grains.Account;
using NilArea.Contracts.Responses.Account;
using Orleans.Concurrency;

namespace NilArea.Account.Grains;

/// <summary>
///     权限管理 Grain 实现
/// </summary>
[Reentrant]
[StatelessWorker]
public class PermissionGrain(
    ILogger<PermissionGrain> logger,
    IPermissionRepository permissionRepository,
    IValidator<CreatePermissionCommand> createPermissionValidator,
    IValidator<UpdatePermissionCommand> updatePermissionValidator,
    IValidator<CreateGroupCommand> createGroupValidator,
    IValidator<UpdateGroupCommand> updateGroupValidator
) : Grain, IPermissionGrain
{
    /// <summary>
    ///     获取所有权限
    /// </summary>
    /// <returns>所有权限列表</returns>
    public async ValueTask<PermissionInfoResponse[]> GetAllPermissionsAsync()
    {
        logger.LogDebug("Getting all permissions");

        var permissions = await permissionRepository.GetAllPermissionsAsync();
        var permissionInfos = new List<PermissionInfoResponse>();

        foreach (var permission in permissions)
            // 这里需要从数据库获取完整的权限信息，包括描述和时间戳
            // 暂时使用空值，实际实现需要修改 GetAllPermissionsAsync 方法
            permissionInfos.Add(new PermissionInfoResponse
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                Description = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            });

        logger.LogDebug("Got {Count} permissions", permissionInfos.Count);
        return permissionInfos.ToArray();
    }

    /// <summary>
    ///     创建权限
    /// </summary>
    /// <param name="command">创建权限命令</param>
    /// <returns>创建的权限信息</returns>
    /// <exception cref="PermissionException">权限验证失败时抛出</exception>
    public async ValueTask<PermissionInfoResponse> CreatePermissionAsync(CreatePermissionCommand command)
    {
        logger.LogDebug("Creating permission: {PermissionName}", command.PermissionName);

        var validate = await createPermissionValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Create permission validation failed: {Errors}", validate.ToString());
            throw new PermissionException(validate.ToString());
        }

        var permission = await permissionRepository.CreatePermissionAsync(command.PermissionName, command.Description);

        logger.LogDebug("Permission created successfully: {PermissionId}, {PermissionName}", permission.PermissionId,
            permission.PermissionName);
        return new PermissionInfoResponse
        {
            PermissionId = permission.PermissionId,
            PermissionName = permission.PermissionName,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    /// <summary>
    ///     更新权限
    /// </summary>
    /// <param name="command">更新权限命令</param>
    /// <returns>更新后的权限信息</returns>
    /// <exception cref="PermissionException">权限验证失败时抛出</exception>
    public async ValueTask<PermissionInfoResponse> UpdatePermissionAsync(UpdatePermissionCommand command)
    {
        logger.LogDebug("Updating permission: {PermissionId}", command.PermissionId);

        var validate = await updatePermissionValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Update permission validation failed: {Errors}", validate.ToString());
            throw new PermissionException(validate.ToString());
        }

        var permission =
            await permissionRepository.UpdatePermissionDescriptionAsync(command.PermissionId, command.Description);

        logger.LogDebug("Permission updated successfully: {PermissionId}", permission.PermissionId);
        return new PermissionInfoResponse
        {
            PermissionId = permission.PermissionId,
            PermissionName = permission.PermissionName,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    ///     删除权限
    /// </summary>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否删除成功</returns>
    public async ValueTask<bool> DeletePermissionAsync(short permissionId)
    {
        logger.LogDebug("Deleting permission: {PermissionId}", permissionId);

        var result = await permissionRepository.DeletePermissionAsync(permissionId);

        if (result)
            logger.LogDebug("Permission deleted successfully: {PermissionId}", permissionId);
        else
            logger.LogWarning("Failed to delete permission: {PermissionId}", permissionId);

        return result;
    }

    /// <summary>
    ///     获取所有用户组
    /// </summary>
    /// <returns>所有用户组列表</returns>
    public async ValueTask<GroupInfoResponse[]> GetAllGroupsAsync()
    {
        logger.LogDebug("Getting all groups");

        var groups = await permissionRepository.GetAllGroupsAsync();
        var groupInfos = groups.Select(g => new GroupInfoResponse
        {
            GroupId = g.GroupId,
            GroupName = g.GroupName,
            Description = g.Description,
            IsSystemGroup = g.IsSystemGroup,
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt,
            DeletedAt = g.DeletedAt
        }).ToArray();

        logger.LogDebug("Got {Count} groups", groupInfos.Length);
        return groupInfos;
    }

    /// <summary>
    ///     创建用户组
    /// </summary>
    /// <param name="command">创建用户组命令</param>
    /// <returns>创建的用户组信息</returns>
    /// <exception cref="PermissionException">用户组验证失败时抛出</exception>
    public async ValueTask<GroupInfoResponse> CreateGroupAsync(CreateGroupCommand command)
    {
        logger.LogDebug("Creating group: {GroupName}", command.GroupName);

        var validate = await createGroupValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Create group validation failed: {Errors}", validate.ToString());
            throw new PermissionException(validate.ToString());
        }

        var group = await permissionRepository.CreateGroupAsync(command.GroupName, command.Description,
            command.IsSystemGroup);

        logger.LogDebug("Group created successfully: {GroupId}, {GroupName}", group.GroupId, group.GroupName);
        return new GroupInfoResponse
        {
            GroupId = group.GroupId,
            GroupName = group.GroupName,
            Description = group.Description,
            IsSystemGroup = group.IsSystemGroup,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt,
            DeletedAt = group.DeletedAt
        };
    }

    /// <summary>
    ///     更新用户组
    /// </summary>
    /// <param name="command">更新用户组命令</param>
    /// <returns>更新后的用户组信息</returns>
    /// <exception cref="PermissionException">用户组验证失败时抛出</exception>
    public async ValueTask<GroupInfoResponse> UpdateGroupAsync(UpdateGroupCommand command)
    {
        logger.LogDebug("Updating group: {GroupId}", command.GroupId);

        var validate = await updateGroupValidator.ValidateAsync(command);
        if (!validate.IsValid)
        {
            logger.LogWarning("Update group validation failed: {Errors}", validate.ToString());
            throw new PermissionException(validate.ToString());
        }

        var group = await permissionRepository.UpdateGroupDescriptionAsync(command.GroupId, command.Description);

        logger.LogDebug("Group updated successfully: {GroupId}, {GroupName}", group.GroupId, group.GroupName);
        return new GroupInfoResponse
        {
            GroupId = group.GroupId,
            GroupName = group.GroupName,
            Description = group.Description,
            IsSystemGroup = group.IsSystemGroup,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdatedAt,
            DeletedAt = group.DeletedAt
        };
    }

    /// <summary>
    ///     删除用户组
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否删除成功</returns>
    public async ValueTask<bool> DeleteGroupAsync(int groupId)
    {
        logger.LogDebug("Deleting group: {GroupId}", groupId);

        var result = await permissionRepository.DeleteGroupAsync(groupId);

        if (result)
            logger.LogDebug("Group deleted successfully: {GroupId}", groupId);
        else
            logger.LogWarning("Failed to delete group: {GroupId}", groupId);

        return result;
    }

    /// <summary>
    ///     为用户组分配权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否分配成功</returns>
    /// <exception cref="PermissionException">参数验证失败时抛出</exception>
    public async ValueTask<bool> AssignPermissionToGroupAsync(int groupId, short permissionId)
    {
        logger.LogDebug("Assigning permission {PermissionId} to group {GroupId}", permissionId, groupId);

        // 验证参数
        if (groupId <= 0)
        {
            logger.LogWarning("Invalid groupId: {GroupId}", groupId);
            throw new PermissionException("Invalid groupId");
        }

        if (permissionId <= 0)
        {
            logger.LogWarning("Invalid permissionId: {PermissionId}", permissionId);
            throw new PermissionException("Invalid permissionId");
        }

        var result = await permissionRepository.AssignPermissionToGroupAsync(groupId, permissionId);

        if (result)
            logger.LogDebug("Permission {PermissionId} assigned to group {GroupId} successfully", permissionId,
                groupId);
        else
            logger.LogWarning("Failed to assign permission {PermissionId} to group {GroupId}", permissionId, groupId);

        return result;
    }

    /// <summary>
    ///     从用户组移除权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否移除成功</returns>
    /// <exception cref="PermissionException">参数验证失败时抛出</exception>
    public async ValueTask<bool> RemovePermissionFromGroupAsync(int groupId, short permissionId)
    {
        logger.LogDebug("Removing permission {PermissionId} from group {GroupId}", permissionId, groupId);

        // 验证参数
        if (groupId <= 0)
        {
            logger.LogWarning("Invalid groupId: {GroupId}", groupId);
            throw new PermissionException("Invalid groupId");
        }

        if (permissionId <= 0)
        {
            logger.LogWarning("Invalid permissionId: {PermissionId}", permissionId);
            throw new PermissionException("Invalid permissionId");
        }

        var result = await permissionRepository.RemovePermissionFromGroupAsync(groupId, permissionId);

        if (result)
            logger.LogDebug("Permission {PermissionId} removed from group {GroupId} successfully", permissionId,
                groupId);
        else
            logger.LogWarning("Failed to remove permission {PermissionId} from group {GroupId}", permissionId, groupId);

        return result;
    }

    /// <summary>
    ///     将用户添加到用户组
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否添加成功</returns>
    /// <exception cref="PermissionException">参数验证失败时抛出</exception>
    public async ValueTask<bool> AddUserToGroupAsync(Guid userId, int groupId)
    {
        logger.LogDebug("Adding user {UserId} to group {GroupId}", userId, groupId);

        // 验证参数
        if (userId == Guid.Empty)
        {
            logger.LogWarning("Invalid userId: {UserId}", userId);
            throw new PermissionException("Invalid userId");
        }

        if (groupId <= 0)
        {
            logger.LogWarning("Invalid groupId: {GroupId}", groupId);
            throw new PermissionException("Invalid groupId");
        }

        var result = await permissionRepository.AddUserToGroupAsync(userId, groupId);

        if (result)
            logger.LogDebug("User {UserId} added to group {GroupId} successfully", userId, groupId);
        else
            logger.LogWarning("Failed to add user {UserId} to group {GroupId}", userId, groupId);

        return result;
    }

    /// <summary>
    ///     将用户从用户组移除
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否移除成功</returns>
    /// <exception cref="PermissionException">参数验证失败时抛出</exception>
    public async ValueTask<bool> RemoveUserFromGroupAsync(Guid userId, int groupId)
    {
        logger.LogDebug("Removing user {UserId} from group {GroupId}", userId, groupId);

        // 验证参数
        if (userId == Guid.Empty)
        {
            logger.LogWarning("Invalid userId: {UserId}", userId);
            throw new PermissionException("Invalid userId");
        }

        if (groupId <= 0)
        {
            logger.LogWarning("Invalid groupId: {GroupId}", groupId);
            throw new PermissionException("Invalid groupId");
        }

        var result = await permissionRepository.RemoveUserFromGroupAsync(userId, groupId);

        if (result)
            logger.LogDebug("User {UserId} removed from group {GroupId} successfully", userId, groupId);
        else
            logger.LogWarning("Failed to remove user {UserId} from group {GroupId}", userId, groupId);

        return result;
    }

    /// <summary>
    ///     为用户添加单独权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否添加成功</returns>
    /// <exception cref="PermissionException">参数验证失败时抛出</exception>
    public async ValueTask<bool> AddUserPermissionAsync(Guid userId, short permissionId)
    {
        logger.LogDebug("Adding permission {PermissionId} to user {UserId}", permissionId, userId);

        // 验证参数
        if (userId == Guid.Empty)
        {
            logger.LogWarning("Invalid userId: {UserId}", userId);
            throw new PermissionException("Invalid userId");
        }

        if (permissionId <= 0)
        {
            logger.LogWarning("Invalid permissionId: {PermissionId}", permissionId);
            throw new PermissionException("Invalid permissionId");
        }

        var result = await permissionRepository.AssignPermissionToUserAsync(userId, permissionId);

        if (result)
            logger.LogDebug("Permission {PermissionId} added to user {UserId} successfully", permissionId, userId);
        else
            logger.LogWarning("Failed to add permission {PermissionId} to user {UserId}", permissionId, userId);

        return result;
    }

    /// <summary>
    ///     从用户移除单独权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否移除成功</returns>
    /// <exception cref="PermissionException">参数验证失败时抛出</exception>
    public async ValueTask<bool> RemoveUserPermissionAsync(Guid userId, short permissionId)
    {
        logger.LogDebug("Removing permission {PermissionId} from user {UserId}", permissionId, userId);

        // 验证参数
        if (userId == Guid.Empty)
        {
            logger.LogWarning("Invalid userId: {UserId}", userId);
            throw new PermissionException("Invalid userId");
        }

        if (permissionId <= 0)
        {
            logger.LogWarning("Invalid permissionId: {PermissionId}", permissionId);
            throw new PermissionException("Invalid permissionId");
        }

        var result = await permissionRepository.RemovePermissionFromUserAsync(userId, permissionId);

        if (result)
            logger.LogDebug("Permission {PermissionId} removed from user {UserId} successfully", permissionId, userId);
        else
            logger.LogWarning("Failed to remove permission {PermissionId} from user {UserId}", permissionId, userId);

        return result;
    }

    /// <summary>
    ///     检查用户是否拥有指定的权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionIds">权限ID列表</param>
    /// <returns>用户是否拥有所有指定的权限</returns>
    /// <exception cref="PermissionException">参数验证失败时抛出</exception>
    public async ValueTask<bool> CheckUserPermissionsAsync(Guid userId, ICollection<short> permissionIds)
    {
        logger.LogDebug("Checking permissions for user {UserId}", userId);

        // 验证参数
        if (userId == Guid.Empty)
        {
            logger.LogWarning("Invalid userId: {UserId}", userId);
            throw new PermissionException("Invalid userId");
        }

        if (permissionIds.Count == 0)
        {
            logger.LogWarning("Invalid permissionIds: {PermissionIds}", permissionIds);
            throw new PermissionException("Invalid permissionIds");
        }

        var userPermissions = await permissionRepository.GetAllPermissionAsync(userId);
        var userPermissionIds = userPermissions.Select(p => p.PermissionId).ToHashSet();

        var result = permissionIds.All(userPermissionIds.Contains);

        if (result)
            logger.LogDebug("User {UserId} has all required permissions", userId);
        else
            logger.LogDebug("User {UserId} missing some required permissions", userId);

        return result;
    }

    /// <summary>
    ///     获取用户的所有权限（包括直接权限和组权限）
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的所有权限</returns>
    public async ValueTask<PermissionInfoResponse[]> GetUserPermissionsAsync(Guid userId)
    {
        logger.LogDebug("Getting permissions for user: {UserId}", userId);

        var permissions = await permissionRepository.GetAllPermissionAsync(userId);
        var permissionInfos = new List<PermissionInfoResponse>();

        foreach (var permission in permissions)
            // 这里需要从数据库获取完整的权限信息，包括描述和时间戳
            // 暂时使用空值，实际实现需要修改 GetAllPermissionAsync 方法
            permissionInfos.Add(new PermissionInfoResponse
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                Description = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            });

        logger.LogDebug("Got {Count} permissions for user: {UserId}", permissionInfos.Count, userId);
        return permissionInfos.ToArray();
    }

    /// <summary>
    ///     获取用户组的所有权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <returns>用户组的所有权限</returns>
    public async ValueTask<PermissionInfoResponse[]> GetGroupPermissionsAsync(int groupId)
    {
        logger.LogDebug("Getting permissions for group: {GroupId}", groupId);

        var permissions = await permissionRepository.GetGroupPermissionsAsync(groupId);
        var permissionInfos = new List<PermissionInfoResponse>();

        foreach (var permission in permissions)
            // 这里需要从数据库获取完整的权限信息，包括描述和时间戳
            // 暂时使用空值，实际实现需要修改 GetGroupPermissionsAsync 方法
            permissionInfos.Add(new PermissionInfoResponse
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                Description = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            });

        logger.LogDebug("Got {Count} permissions for group: {GroupId}", permissionInfos.Count, groupId);
        return permissionInfos.ToArray();
    }

    /// <summary>
    ///     获取用户的所有用户组
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的所有用户组</returns>
    public async ValueTask<GroupInfoResponse[]> GetUserGroupsAsync(Guid userId)
    {
        logger.LogDebug("Getting groups for user: {UserId}", userId);

        var groups = await permissionRepository.GetUserGroupsAsync(userId);
        var groupInfos = groups.Select(g => new GroupInfoResponse
        {
            GroupId = g.GroupId,
            GroupName = g.GroupName,
            Description = g.Description,
            IsSystemGroup = g.IsSystemGroup,
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt,
            DeletedAt = g.DeletedAt
        }).ToArray();

        logger.LogDebug("Got {Count} groups for user: {UserId}", groupInfos.Length, userId);
        return groupInfos;
    }
}