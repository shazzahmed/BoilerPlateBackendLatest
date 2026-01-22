public class SendNotification
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? Date { get; set; }
    public string Message { get; set; }
    public string VisibleStudent { get; set; } = "no";
    public string VisibleStaff { get; set; } = "no";
    public string VisibleParent { get; set; } = "no";
    public string CreatedBy { get; set; }
    public int? CreatedId { get; set; }
    public string IsActive { get; set; } = "no";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
