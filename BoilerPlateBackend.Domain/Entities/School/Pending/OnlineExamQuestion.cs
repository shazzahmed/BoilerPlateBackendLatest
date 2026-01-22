public class OnlineExamQuestion
{
    public int Id { get; set; }
    public int? QuestionId { get; set; }
    public int? OnlineExamId { get; set; }
    public int? SessionId { get; set; }
    public string IsActive { get; set; } = "0";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
