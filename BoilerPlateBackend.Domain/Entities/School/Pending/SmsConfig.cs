public class SmsConfig
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string ApiId { get; set; }
    public string Authkey { get; set; }
    public string SenderId { get; set; }
    public string Contact { get; set; }
    public string Username { get; set; }
    public string Url { get; set; }
    public string Password { get; set; }
    public string IsActive { get; set; } = "disabled";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
