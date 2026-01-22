using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GetDefaultersRequest
    {
        [Required(ErrorMessage = "Threshold is required")]
        [Range(0, 100, ErrorMessage = "Threshold must be between 0 and 100")]
        public decimal Threshold { get; set; }  // e.g., 75.0 for 75%

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public int? ClassId { get; set; }
        
        public int? SectionId { get; set; }
        
        public int? SessionId { get; set; }
    }
}

