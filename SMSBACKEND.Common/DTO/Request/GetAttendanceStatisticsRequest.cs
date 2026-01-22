using System;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GetAttendanceStatisticsRequest
    {
        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}