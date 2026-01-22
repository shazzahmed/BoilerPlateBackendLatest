using System;
using System.Collections.Generic;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class AttendanceByClassSectionResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public DateTime AttendanceDate { get; set; }
        public List<AttendanceRecordResponse> Records { get; set; } = new List<AttendanceRecordResponse>();
    }

    /// <summary>
    /// Simplified response - only attendance data, no student info
    /// Frontend will merge this with cached student data
    /// </summary>
    public class AttendanceRecordResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }  // Frontend uses this to match with cached students
        public AttendanceType AttendanceType { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}