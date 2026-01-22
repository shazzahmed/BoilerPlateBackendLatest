public class Log
{
    public int Id { get; set; }
    public string Message { get; set; }
    public int RecordId { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; }
    public string IpAddress { get; set; }
    public string Platform { get; set; }
    public string Agent { get; set; }
    public DateTime Time { get; set; }
    public DateTime? CreatedAt { get; set; }
}
