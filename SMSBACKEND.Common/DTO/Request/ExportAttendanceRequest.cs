using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class ExportAttendanceRequest
    {
        [Required(ErrorMessage = "Format is required")]
        [RegularExpression("^(Excel|PDF)$", ErrorMessage = "Format must be either 'Excel' or 'PDF'")]
        public string Format { get; set; }  // "Excel" or "PDF"

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public int? ClassId { get; set; }
        
        public int? SectionId { get; set; }
        
        public int? SessionId { get; set; }
        
        public bool IncludeStatistics { get; set; } = true;
    }
}

