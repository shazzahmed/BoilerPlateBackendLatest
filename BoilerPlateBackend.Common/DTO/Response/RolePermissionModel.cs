
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class RolePermissionModel
    {
        public string RoleId { get; set; }
        public ApplicationRoleModel Role { get; set; }

        public int PermissionId { get; set; }
        public PermissionModel Permission { get; set; }
    }
}
