using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ResetModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
