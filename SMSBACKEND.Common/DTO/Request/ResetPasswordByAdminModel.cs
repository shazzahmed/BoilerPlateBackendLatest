using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ResetPasswordByAdminModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The NewPassword and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
