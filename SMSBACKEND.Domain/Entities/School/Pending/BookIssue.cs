public class BookIssue
{
    public int Id { get; set; }
    public int? BookId { get; set; }
    public DateTime? DueReturnDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime? IssueDate { get; set; }
    public int IsReturned { get; set; } = 0;
    public int? MemberId { get; set; }
    public string IsActive { get; set; } = "no";
    public DateTime CreatedAt { get; set; }
}
