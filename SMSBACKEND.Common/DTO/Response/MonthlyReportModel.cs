namespace Common.DTO.Response
{
    public class MonthlyReportModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalWorkingDays { get; set; }
        public int TotalStudents { get; set; }
        public List<ClassSectionSummary> ClassSummaries { get; set; }
        public List<StudentAttendanceSummary> StudentSummaries { get; set; }
    }

    public class ClassSectionSummary
    {
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public int TotalStudents { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }
        public int TotalExcused { get; set; }
        public decimal AverageAttendancePercentage { get; set; }
    }

    public class StudentAttendanceSummary
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string AdmissionNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int ExcusedDays { get; set; }
        public decimal AttendancePercentage { get; set; }
        public string Status { get; set; }  // "Excellent", "Good", "Average", "Poor"
    }
}

