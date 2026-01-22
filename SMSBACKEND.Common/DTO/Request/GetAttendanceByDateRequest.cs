using System;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class GetAttendanceByDateRequest
    {
        [Required]
        public DateTime AttendanceDate { get; set; }
    }
}

