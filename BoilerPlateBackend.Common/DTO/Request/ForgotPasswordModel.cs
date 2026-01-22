using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
