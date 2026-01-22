using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.DTO.Response
{
    public class ApplicationRoleModel : IdentityRole
    {
        public bool IsSystem { get; set; }
        public ICollection<ApplicationUserRoleModel> UserRoles { get; set; }
        public ICollection<RolePermissionModel> RolePermissions { get; set; }
    }
}
