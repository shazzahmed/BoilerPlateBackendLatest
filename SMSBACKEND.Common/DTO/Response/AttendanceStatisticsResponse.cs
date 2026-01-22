using System;
using System.Collections.Generic;

namespace Common.DTO.Response
{
    public class AttendanceStatisticsResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalStudents { get; set; }
        public int TotalRecords { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int HalfDayCount { get; set; }
        public double PresentPercentage { get; set; }
        public double AbsentPercentage { get; set; }
        public double LatePercentage { get; set; }
        public double HalfDayPercentage { get; set; }
        public List<DailyAttendanceResponse> DailyAttendance { get; set; } = new List<DailyAttendanceResponse>();
    }

    public class DailyAttendanceResponse
    {
        public DateTime Date { get; set; }
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int HalfDayCount { get; set; }
        public double PresentPercentage { get; set; }
        public double AbsentPercentage { get; set; }
    }
}