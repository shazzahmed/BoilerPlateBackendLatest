public class StaffLeaveRequest
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime LeaveFrom { get; set; }
    public DateTime LeaveTo { get; set; }
    public int LeaveDays { get; set; }
    public string EmployeeRemark { get; set; }
    public string AdminRemark { get; set; }
    public string Status { get; set; }
    public string AppliedBy { get; set; }
    public string DocumentFile { get; set; }
    public DateTime Date { get; set; }
}
