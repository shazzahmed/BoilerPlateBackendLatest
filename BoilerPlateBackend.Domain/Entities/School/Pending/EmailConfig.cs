
// Model for EmailConfig
public class EmailConfig
{
    public int Id { get; set; }
    public string? EmailType { get; set; }
    public string? SmtpServer { get; set; }
    public string? SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public string? SslTls { get; set; }
    public string IsActive { get; set; } = "no";
    public DateTime CreatedAt { get; set; }
}
