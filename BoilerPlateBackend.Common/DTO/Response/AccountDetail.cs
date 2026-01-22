using Common.DTO.Request;

namespace Common.DTO.Response
{
    public class AccountDetail
    {
        public static Token? TokenInfo { get; set; }

        public static UserClaim? UserInfo { get; set; }

        public static LoginModel? LoginInfo { get; set; }

        public static List<Role>? Roles { get; set; }

        public static List<ExternalLoginViewModel>? ExternalLoginUrls { get; set; }
    }
}
