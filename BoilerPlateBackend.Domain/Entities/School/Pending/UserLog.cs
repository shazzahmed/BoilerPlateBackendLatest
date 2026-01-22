public class UserLog
{
    public int Id { get; set; }
    public string User { get; set; }
    public string Role { get; set; }
    public int? ClassSectionId { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime LoginDateTime { get; set; }
}
