using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ProcessWorkflowRequest
    {
        [Required]
        public List<int> EnquiryIds { get; set; } = new List<int>();

        [Required]
        public string ProcessedBy { get; set; } = string.Empty;
    }
}

