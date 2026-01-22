public class StaffRating
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public string Comment { get; set; }
    public int Rate { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; }
    public int Status { get; set; } // 0 - decline, 1 - approve
    public DateTime Entrydt { get; set; }
}
