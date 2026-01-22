using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class AddPhoneNumberModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
