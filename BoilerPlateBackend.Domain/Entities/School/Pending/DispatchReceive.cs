
// Model for DispatchReceive
public class DispatchReceive
{
    public int Id { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public string ToTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string FromTitle { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Type { get; set; } = string.Empty;
}
