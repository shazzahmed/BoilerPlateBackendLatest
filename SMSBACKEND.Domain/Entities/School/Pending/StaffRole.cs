public class StaffRole
{
    public int Id { get; set; }
    public int? RoleId { get; set; }
    public int? StaffId { get; set; }
    public int IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
