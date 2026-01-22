namespace Common.DTO.Response
{
    public class DefaultersReportModel
    {
        public decimal Threshold { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDefaulters { get; set; }
        public int TotalWorkingDays { get; set; }
        public List<DefaulterStudent> Defaulters { get; set; }
    }

    public class DefaulterStudent
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
        public decimal AttendancePercentage { get; set; }
        public string FatherName { get; set; }
        public string FatherPhone { get; set; }
        public string MotherName { get; set; }
        public string MotherPhone { get; set; }
        public string GuardianName { get; set; }
        public string GuardianPhone { get; set; }
        public string GuardianEmail { get; set; }
    }
}

