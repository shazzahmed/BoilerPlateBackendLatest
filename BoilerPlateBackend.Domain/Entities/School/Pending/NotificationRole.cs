public class NotificationRole
{
    public int Id { get; set; }
    public int? SendNotificationId { get; set; }
    public int? RoleId { get; set; }
    public int IsActive { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
}
