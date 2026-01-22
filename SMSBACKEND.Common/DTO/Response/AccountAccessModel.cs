using Common.DTO.Request;
using System.Collections.Generic;

namespace Common.DTO.Response
{
    public class AccountAccessModel
	{
		public static AccessTokenModel? TokenInfo { get; set; }
		public static ApplicationUserModel? UserInfo { get; set; }
		public static LoginModel? LoginInfo { get; set; }

		public static List<RoleModel>? Roles { get; set; }
		public static List<ExternalLoginViewModel>? ExternalLoginUrls { get; set; }
	}
}
