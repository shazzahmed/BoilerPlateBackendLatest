
using Common.DTO.Request;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IRolePermissionRepository : IBaseRepository<RolePermission, int>
    {
        Task SyncRolePermissionsAsync(string roleId, List<PermissionAssignmentRequest> assignments);
    }
}
