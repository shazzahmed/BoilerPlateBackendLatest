public class StaffPayslip
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public float Basic { get; set; }
    public float TotalAllowance { get; set; }
    public float TotalDeduction { get; set; }
    public int LeaveDeduction { get; set; }
    public string Tax { get; set; }
    public float NetSalary { get; set; }
    public string Status { get; set; }
    public string Month { get; set; }
    public string Year { get; set; }
    public string PaymentMode { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Remark { get; set; }
}
