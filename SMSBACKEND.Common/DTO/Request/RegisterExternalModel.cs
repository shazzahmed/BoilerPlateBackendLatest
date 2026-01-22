using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class RegisterExternalModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public string ProviderDisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}
