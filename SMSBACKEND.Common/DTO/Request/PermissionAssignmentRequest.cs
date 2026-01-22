
namespace Common.DTO.Request
{
    public class PermissionAssignmentRequest
    {
        public string RoleId { get; set; }
        public int ModuleId { get; set; }
        public string PermissionName { get; set; }
        public string ModuleName { get; set; }
    }
    public class RolePermissionUpdateRequest
    {
        public string RoleId { get; set; }
        public List<PermissionAssignmentRequest> UpdatedPermissions { get; set; }
    }
}
