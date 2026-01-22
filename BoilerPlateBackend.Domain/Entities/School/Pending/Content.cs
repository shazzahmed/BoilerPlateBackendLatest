public class Content
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string IsPublic { get; set; } = "No";
    public int? ClassId { get; set; }
    public int ClsSecId { get; set; }
    public string? File { get; set; }
    public int CreatedBy { get; set; }
    public string? Note { get; set; }
    public string IsActive { get; set; } = "no";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime Date { get; set; }
}

