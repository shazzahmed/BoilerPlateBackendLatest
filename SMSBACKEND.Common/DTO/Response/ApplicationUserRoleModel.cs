using Common.DTO.Request;
using Microsoft.AspNetCore.Identity;

namespace Common.DTO.Response
{
    public class ApplicationUserRoleModel : IdentityUserRole<string>
    {
        public virtual ApplicationUserModel User { get; set; }
        public virtual ApplicationRoleModel Role { get; set; }
    }
}
