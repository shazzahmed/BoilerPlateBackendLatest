
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class RolePermissionRepository : BaseRepository<RolePermission, int>, IRolePermissionRepository
    {
        public RolePermissionRepository(IMapper mapper, ISqlServerDbContext context) : base(mapper, context)
        {
        }
        public async Task SyncRolePermissionsAsync(string roleId, List<PermissionAssignmentRequest> assignments)
        {
            if (string.IsNullOrWhiteSpace(roleId) || assignments == null)
                throw new ArgumentException("Invalid input");

            // Step 1: Get all current permissions for this role
            var existingPermissions = await DbContext.RolePermission
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            // Step 2: Load all existing permissions
            var allPermissions = await DbContext.Permission.ToListAsync();

            // Step 3: Identify missing permission entries
            var missingPermissions = assignments
                .Where(a => !allPermissions.Any(p => p.ModuleId == a.ModuleId && p.Name == $"{a.PermissionName}_{a.ModuleName}"))
                .Select(a => new Permission
                {
                    Name = $"{a.PermissionName}_{a.ModuleName}",
                    Description = $"{a.PermissionName} permission for module {a.ModuleName} and {a.ModuleId}",
                    ModuleId = a.ModuleId
                })
                .ToList();

            // Step 3: Add missing permissions
            if (missingPermissions.Any())
            {
                await DbContext.Permission.AddRangeAsync(missingPermissions);
                await DbContext.SaveChangesAsync(); // persist new permissions
                // Step 4: Reload permissions after insert
                allPermissions = await DbContext.Permission.ToListAsync();
            }


            // Step 5: Resolve permission IDs from incoming assignments
            var requestedPermissionIds = allPermissions
                .Where(p => assignments.Any(a => a.ModuleId == p.ModuleId && $"{a.PermissionName}_{a.ModuleName}" == p.Name))
                .Select(p => p.Id)
                .ToList();

            // Step 6: Determine which to remove
            var toRemove = existingPermissions
                .Where(rp => !requestedPermissionIds.Contains(rp.PermissionId))
                .ToList();

            // Step 7: Determine which to add
            var existingIds = existingPermissions.Select(rp => rp.PermissionId).ToHashSet();
            var toAdd = requestedPermissionIds
                .Where(id => !existingIds.Contains(id))
                .Select(id => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = id
                })
                .ToList();

            // Step 5: Apply changes
            if (toRemove.Any())
                DbContext.RolePermission.RemoveRange(toRemove);

            if (toAdd.Any())
                await DbContext.RolePermission.AddRangeAsync(toAdd);

            await DbContext.SaveChangesAsync();
        }

    }
}
