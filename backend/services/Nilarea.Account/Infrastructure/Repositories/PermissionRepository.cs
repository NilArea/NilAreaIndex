using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NilArea.Account.DTOs;
using NilArea.Account.Infrastructure.Data;
using NilArea.Account.Infrastructure.Data.Entities;
using NilArea.Contracts.Exceptions;

namespace NilArea.Account.Infrastructure.Repositories;

/// <summary>
///     权限管理数据库实现
/// </summary>
public sealed class PermissionRepository(
    ILogger<PermissionRepository> logger,
    AccountDbContext dbContext
) : IPermissionRepository
{
    /// <summary>
    ///     获取所有权限
    /// </summary>
    /// <returns>所有权限列表</returns>
    public async ValueTask<PermissionTagInfo[]> GetAllPermissionsAsync()
    {
        var permissions = await dbContext.PermissionTags.AsNoTracking()
            .Select(p => new PermissionTagInfo(p.PermissionId, p.PermissionName))
            .ToArrayAsync();
        return permissions;
    }

    /// <summary>
    ///     创建权限
    /// </summary>
    /// <param name="permissionName">权限名称</param>
    /// <param name="description">权限描述</param>
    /// <returns>创建的权限信息</returns>
    /// <exception cref="AccountException">权限名称已存在或创建失败时抛出</exception>
    public async ValueTask<PermissionTagInfo> CreatePermissionAsync(string permissionName, string description)
    {
        // 检查权限名称是否已存在
        if (await dbContext.PermissionTags.AsNoTracking()
                .AnyAsync(p => p.PermissionName == permissionName))
            throw new AccountException("Permission name already exists");

        var permission = new PermissionTag
        {
            PermissionName = permissionName,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await dbContext.PermissionTags.AddAsync(permission);
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new AccountException("Permission name already exists");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create permission: {PermissionName}", permissionName);
            throw new AccountException("Failed to create permission");
        }

        return new PermissionTagInfo(permission.PermissionId, permission.PermissionName);
    }

    /// <summary>
    ///     更新权限描述
    /// </summary>
    /// <param name="permissionId">权限ID</param>
    /// <param name="description">权限描述</param>
    /// <returns>更新后的权限信息</returns>
    /// <exception cref="AccountException">权限不存在或更新失败时抛出</exception>
    public async ValueTask<PermissionTagInfo> UpdatePermissionDescriptionAsync(short permissionId, string description)
    {
        var permission = await dbContext.PermissionTags.FirstOrDefaultAsync(p => p.PermissionId == permissionId);
        if (permission == null)
            throw new AccountException("Permission not found");

        permission.Description = description;
        permission.UpdatedAt = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update permission description: {PermissionId}", permissionId);
            throw new AccountException("Failed to update permission description");
        }

        return new PermissionTagInfo(permission.PermissionId, permission.PermissionName);
    }

    /// <summary>
    ///     删除权限
    /// </summary>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否删除成功</returns>
    public async ValueTask<bool> DeletePermissionAsync(short permissionId)
    {
        var permission = await dbContext.PermissionTags.FirstOrDefaultAsync(p => p.PermissionId == permissionId);
        if (permission == null)
            return false;

        try
        {
            dbContext.PermissionTags.Remove(permission);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            // 处理外键约束错误
            logger.LogError(ex, "Failed to delete permission: {PermissionId}, foreign key constraint violation",
                permissionId);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete permission: {PermissionId}", permissionId);
            return false;
        }
    }

    /// <summary>
    ///     获取所有用户组
    /// </summary>
    /// <returns>所有用户组列表</returns>
    public async ValueTask<AccountGroupInfo[]> GetAllGroupsAsync()
    {
        var groups = await dbContext.AccountGroups.AsNoTracking()
            .Select(g => new AccountGroupInfo
            {
                GroupId = g.GroupId,
                GroupName = g.GroupName,
                Description = g.Description ?? string.Empty,
                IsSystemGroup = g.IsSystemGroup,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdateAt,
                DeletedAt = g.DeleteAt
            })
            .ToArrayAsync();
        return groups;
    }

    /// <summary>
    ///     创建用户组
    /// </summary>
    /// <param name="groupName">用户组名称</param>
    /// <param name="description">用户组描述</param>
    /// <param name="isSystemGroup">是否为系统组</param>
    /// <returns>创建的用户组信息</returns>
    /// <exception cref="AccountException">用户组名称已存在或创建失败时抛出</exception>
    public async ValueTask<AccountGroupInfo> CreateGroupAsync(string groupName, string description, bool isSystemGroup)
    {
        // 检查用户组名称是否已存在
        if (await dbContext.AccountGroups.AsNoTracking()
                .AnyAsync(g => g.GroupName == groupName))
            throw new AccountException("Group name already exists");

        var group = new AccountGroup
        {
            GroupName = groupName,
            Description = description,
            IsSystemGroup = isSystemGroup,
            CreatedAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        };

        try
        {
            await dbContext.AccountGroups.AddAsync(group);
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new AccountException("Group name already exists");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create group: {GroupName}", groupName);
            throw new AccountException("Failed to create group");
        }

        return new AccountGroupInfo
        {
            GroupId = group.GroupId,
            GroupName = group.GroupName,
            Description = group.Description ?? string.Empty,
            IsSystemGroup = group.IsSystemGroup,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdateAt,
            DeletedAt = group.DeleteAt
        };
    }

    /// <summary>
    ///     更新用户组描述
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <param name="description">用户组描述</param>
    /// <returns>更新后的用户组信息</returns>
    /// <exception cref="AccountException">用户组不存在或更新失败时抛出</exception>
    public async ValueTask<AccountGroupInfo> UpdateGroupDescriptionAsync(int groupId, string description)
    {
        var group = await dbContext.AccountGroups.FirstOrDefaultAsync(g => g.GroupId == groupId);
        if (group == null)
            throw new AccountException("Group not found");

        group.Description = description;
        group.UpdateAt = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update group description: {GroupId}", groupId);
            throw new AccountException("Failed to update group description");
        }

        return new AccountGroupInfo
        {
            GroupId = group.GroupId,
            GroupName = group.GroupName,
            Description = group.Description ?? string.Empty,
            IsSystemGroup = group.IsSystemGroup,
            CreatedAt = group.CreatedAt,
            UpdatedAt = group.UpdateAt,
            DeletedAt = group.DeleteAt
        };
    }

    /// <summary>
    ///     删除用户组（软删除）
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否删除成功</returns>
    public async ValueTask<bool> DeleteGroupAsync(int groupId)
    {
        var group = await dbContext.AccountGroups.FirstOrDefaultAsync(g => g.GroupId == groupId);
        if (group == null || group.IsSystemGroup)
            return false;

        group.DeleteAt = DateTime.UtcNow;
        group.UpdateAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     为用户组分配权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否分配成功</returns>
    public async ValueTask<bool> AssignPermissionToGroupAsync(int groupId, short permissionId)
    {
        var existing = await dbContext.GroupPermissions
            .FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId);
        if (existing != null)
            return true;

        var groupPermission = new GroupPermission
        {
            GroupId = groupId,
            PermissionId = permissionId
        };

        await dbContext.GroupPermissions.AddAsync(groupPermission);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     从用户组移除权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否移除成功</returns>
    public async ValueTask<bool> RemovePermissionFromGroupAsync(int groupId, short permissionId)
    {
        var groupPermission = await dbContext.GroupPermissions
            .FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId);
        if (groupPermission == null)
            return false;

        dbContext.GroupPermissions.Remove(groupPermission);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     将用户添加到用户组
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否添加成功</returns>
    public async ValueTask<bool> AddUserToGroupAsync(Guid userId, int groupId)
    {
        var existing = await dbContext.AccountUserGroups
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);
        if (existing != null)
            return true;

        var userGroup = new AccountUserGroup
        {
            UserId = userId,
            GroupId = groupId,
            JoinedAt = DateTime.UtcNow
        };

        await dbContext.AccountUserGroups.AddAsync(userGroup);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     将用户从用户组移除
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="groupId">用户组ID</param>
    /// <returns>是否移除成功</returns>
    public async ValueTask<bool> RemoveUserFromGroupAsync(Guid userId, int groupId)
    {
        var userGroup = await dbContext.AccountUserGroups
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);
        if (userGroup == null)
            return false;

        dbContext.AccountUserGroups.Remove(userGroup);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     为用户直接分配权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否分配成功</returns>
    public async ValueTask<bool> AssignPermissionToUserAsync(Guid userId, short permissionId)
    {
        var existing = await dbContext.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
        if (existing != null)
            return true;

        var userPermission = new UserPermission
        {
            UserId = userId,
            PermissionId = permissionId
        };

        await dbContext.UserPermissions.AddAsync(userPermission);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     从用户移除直接权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="permissionId">权限ID</param>
    /// <returns>是否移除成功</returns>
    public async ValueTask<bool> RemovePermissionFromUserAsync(Guid userId, short permissionId)
    {
        var userPermission = await dbContext.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
        if (userPermission == null)
            return false;

        dbContext.UserPermissions.Remove(userPermission);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     获取用户组的所有权限
    /// </summary>
    /// <param name="groupId">用户组ID</param>
    /// <returns>用户组的所有权限</returns>
    public async ValueTask<PermissionTagInfo[]> GetGroupPermissionsAsync(int groupId)
    {
        var permissions = await dbContext.GroupPermissions.AsNoTracking()
            .Include(gp => gp.Permission)
            .Where(gp => gp.GroupId == groupId)
            .Select(gp => new PermissionTagInfo(gp.Permission.PermissionId, gp.Permission.PermissionName))
            .ToArrayAsync();
        return permissions;
    }

    /// <summary>
    ///     获取用户的所有用户组
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的所有用户组</returns>
    public async ValueTask<AccountGroupInfo[]> GetUserGroupsAsync(Guid userId)
    {
        var groups = await dbContext.AccountUserGroups.AsNoTracking()
            .Include(ug => ug.Group)
            .Where(ug => ug.UserId == userId)
            .Select(ug => ug.Group)
            .Where(g => g.DeleteAt == null)
            .Select(g => new AccountGroupInfo
            {
                GroupId = g.GroupId,
                GroupName = g.GroupName,
                Description = g.Description ?? string.Empty,
                IsSystemGroup = g.IsSystemGroup,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdateAt,
                DeletedAt = g.DeleteAt
            })
            .ToArrayAsync();
        return groups;
    }

    /// <summary>
    ///     根据用户ID获取所有权限
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户的所有权限</returns>
    /// <exception cref="AccountException">账号不可用时抛出</exception>
    public async ValueTask<PermissionTagInfo[]> GetAllPermissionAsync(Guid userId)
    {
        // 验证用户是否存在
        if (!await dbContext.AccountUsers.AnyAsync(u => u.UserId == userId && u.DeleteAt == null))
            throw new AccountException("Account not available");

        // 并行查询直接权限和组权限
        var directPermissionsTask = dbContext.UserPermissions.AsNoTracking()
            .Where(up => up.UserId == userId)
            .Join(
                dbContext.PermissionTags.AsNoTracking(),
                up => up.PermissionId,
                p => p.PermissionId,
                (up, p) => new PermissionTagInfo(p.PermissionId, p.PermissionName)
            )
            .ToListAsync();

        var groupPermissionsTask = dbContext.AccountUserGroups.AsNoTracking()
            .Where(ug => ug.UserId == userId)
            .Join(
                dbContext.AccountGroups.AsNoTracking().Where(g => g.DeleteAt == null),
                ug => ug.GroupId,
                g => g.GroupId,
                (ug, g) => g.GroupId
            )
            .Join(
                dbContext.GroupPermissions.AsNoTracking(),
                groupId => groupId,
                gp => gp.GroupId,
                (groupId, gp) => gp.PermissionId
            )
            .Join(
                dbContext.PermissionTags.AsNoTracking(),
                permissionId => permissionId,
                p => p.PermissionId,
                (permissionId, p) => new PermissionTagInfo(p.PermissionId, p.PermissionName)
            )
            .ToListAsync();

        // 等待两个查询完成
        await Task.WhenAll(directPermissionsTask, groupPermissionsTask);

        // 合并并去重权限
        var allPermissions = directPermissionsTask.Result
            .Union(groupPermissionsTask.Result)
            .DistinctBy(p => p.PermissionId)
            .ToArray();

        return allPermissions;
    }
}