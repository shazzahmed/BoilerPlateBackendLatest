using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GetAttendancePercentageRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
    }
}

