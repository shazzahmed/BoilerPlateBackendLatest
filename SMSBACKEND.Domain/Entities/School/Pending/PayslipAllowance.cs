public class PayslipAllowance
{
    public int Id { get; set; }
    public int PayslipId { get; set; }
    public string AllowanceType { get; set; }
    public float Amount { get; set; }
    public int StaffId { get; set; }
    public string CalType { get; set; }
}
