public class StudentApplyLeave
{
    public int Id { get; set; }
    public int StudentSessionId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime ApplyDate { get; set; }
    public int Status { get; set; }
    public string Docs { get; set; }
    public string Reason { get; set; }
    public int ApproveBy { get; set; }
    public int RequestType { get; set; }  // 0 = student, 1 = staff
}
