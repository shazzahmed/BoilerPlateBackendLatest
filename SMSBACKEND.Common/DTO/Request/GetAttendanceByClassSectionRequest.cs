using System;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GetAttendanceByClassSectionRequest
    {
        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }
    }
}