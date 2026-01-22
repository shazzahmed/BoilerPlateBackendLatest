using System;
using System.ComponentModel.DataAnnotations;
using Common.DTO.Response;
using static Common.Utilities.Enums;

namespace Common.DTO.Request
{
    public class UpdateAttendanceRequest
    {
        [Required]
        public AttendanceType AttendanceType { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}