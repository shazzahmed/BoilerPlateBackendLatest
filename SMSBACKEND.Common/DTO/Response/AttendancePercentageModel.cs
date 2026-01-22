namespace Common.DTO.Response
{
    public class AttendancePercentageModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string AdmissionNo { get; set; }
        public int TotalWorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int ExcusedDays { get; set; }
        public decimal AttendancePercentage { get; set; }
        public string Status { get; set; }
        public List<DailyAttendanceRecord> DailyRecords { get; set; }
    }

    public class DailyAttendanceRecord
    {
        public DateTime Date { get; set; }
        public string AttendanceType { get; set; }
        public string Remark { get; set; }
    }
}

