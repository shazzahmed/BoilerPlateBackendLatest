public class ItemIssue
{
    public int Id { get; set; }
    public string IssueType { get; set; }
    public string IssueTo { get; set; }
    public string IssueBy { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int? ItemCategoryId { get; set; }
    public int? ItemId { get; set; }
    public int Quantity { get; set; }
    public string Note { get; set; }
    public bool IsReturned { get; set; }
    public DateTime CreatedAt { get; set; }
    public string IsActive { get; set; }
}
