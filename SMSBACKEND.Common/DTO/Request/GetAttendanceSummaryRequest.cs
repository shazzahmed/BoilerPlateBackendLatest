using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GetAttendanceSummaryRequest
    {
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public int? ClassId { get; set; }
        
        public int? SectionId { get; set; }
        
        public int? SessionId { get; set; }
    }
}

