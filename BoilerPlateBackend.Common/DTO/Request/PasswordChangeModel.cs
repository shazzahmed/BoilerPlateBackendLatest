using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class PasswordChangeModel
    {
        [Required]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New password")]
        [Compare("NewPassword", ErrorMessage = "The new password and repeat password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
