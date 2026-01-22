using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GetMonthlyReportRequest
    {
        [Required]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
        public int Month { get; set; }

        [Required]
        [Range(2020, 2100, ErrorMessage = "Year must be between 2020 and 2100")]
        public int Year { get; set; }

        public int? ClassId { get; set; }
        
        public int? SectionId { get; set; }
        
        public int? SessionId { get; set; }
    }
}

