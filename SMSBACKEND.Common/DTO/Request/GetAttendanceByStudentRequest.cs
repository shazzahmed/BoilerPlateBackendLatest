using System;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GetAttendanceByStudentRequest
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}

