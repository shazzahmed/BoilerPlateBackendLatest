using Common.DTO.Response;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        internal List<ExternalAuthenticationProvider>? ExternalAuthenticationProviders { get; set; }
    }
}
