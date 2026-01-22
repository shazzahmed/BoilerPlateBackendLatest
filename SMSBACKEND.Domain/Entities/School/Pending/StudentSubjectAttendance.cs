public class StudentSubjectAttendance
{
    public int Id { get; set; }
    public int? StudentSessionId { get; set; }
    public int? SubjectTimetableId { get; set; }
    public int? AttendenceTypeId { get; set; }
    public DateTime? Date { get; set; }
    public string Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}
