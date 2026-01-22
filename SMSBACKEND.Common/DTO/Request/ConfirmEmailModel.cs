using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Common.DTO.Request
{
    public class ConfirmEmailModel
    {
        [Required]
        [FromQuery]
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [FromQuery]
        [Required(ErrorMessage = "Verification code can't be null")]
        public string Code { get; set; } = string.Empty;
    }
}
