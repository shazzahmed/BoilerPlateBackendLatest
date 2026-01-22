using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class VerifyPhoneNumberModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
