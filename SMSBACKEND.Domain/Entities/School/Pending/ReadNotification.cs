public class ReadNotification
{
    public int Id { get; set; }
    public int? StudentId { get; set; }
    public int? ParentId { get; set; }
    public int? StaffId { get; set; }
    public int? NotificationId { get; set; }
    public string IsActive { get; set; } = "no";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
