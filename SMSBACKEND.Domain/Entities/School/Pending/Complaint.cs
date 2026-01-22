public class Complaint
{
    public int Id { get; set; }
    public string ComplaintType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ActionTaken { get; set; } = string.Empty;
    public string Assigned { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}

