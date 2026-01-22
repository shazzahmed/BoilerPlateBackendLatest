using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.DTO.Response;
using static Common.Utilities.Enums;

namespace Common.DTO.Request
{
    public class MarkAttendanceRequest
    {
        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        public int SessionId { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }

        [Required]
        public List<AttendanceRecordRequest> Records { get; set; } = new List<AttendanceRecordRequest>();
    }

    /// <summary>
    /// Simplified request - removed redundant fields that parent already has
    /// Only essential fields needed per student
    /// </summary>
    public class AttendanceRecordRequest
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public AttendanceType AttendanceType { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}