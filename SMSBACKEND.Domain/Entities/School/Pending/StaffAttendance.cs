public class StaffAttendance
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int StaffId { get; set; }
    public int StaffAttendanceTypeId { get; set; }
    public string Remark { get; set; }
    public int IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
