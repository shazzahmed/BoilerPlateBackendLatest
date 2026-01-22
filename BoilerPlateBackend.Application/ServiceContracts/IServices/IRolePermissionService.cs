
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IRolePermissionService : IBaseService<RolePermissionModel, RolePermission, int>
    {
        Task SyncRolePermissionsAsync(string roleId, List<PermissionAssignmentRequest> assignments);
    }
}
