public class Book
{
    public int Id { get; set; }
    public string BookTitle { get; set; }
    public string BookNo { get; set; }
    public string IsbnNo { get; set; }
    public string Subject { get; set; }
    public string RackNo { get; set; }
    public string Publish { get; set; }
    public string Author { get; set; }
    public int? Qty { get; set; }
    public decimal? PerUnitCost { get; set; }
    public DateTime? PostDate { get; set; }
    public string Description { get; set; }
    public string Available { get; set; } = "yes";
    public string IsActive { get; set; } = "no";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
