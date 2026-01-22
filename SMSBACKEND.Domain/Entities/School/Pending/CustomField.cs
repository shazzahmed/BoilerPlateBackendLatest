public class CustomField
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? BelongTo { get; set; }
    public string? Type { get; set; }
    public int? BsColumn { get; set; }
    public int Validation { get; set; } = 0;
    public string? FieldValues { get; set; }
    public string? ShowTable { get; set; }
    public int VisibleOnTable { get; set; }
    public int IsActive { get; set; } = 0;
    public int? Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

