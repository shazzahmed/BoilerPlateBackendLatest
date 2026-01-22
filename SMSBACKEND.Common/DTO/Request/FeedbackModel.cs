using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class FeedbackModel
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
