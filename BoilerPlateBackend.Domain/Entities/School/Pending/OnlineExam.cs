public class OnlineExam
{
    public int Id { get; set; }
    public string Exam { get; set; }
    public int Attempt { get; set; }
    public DateTime? ExamFrom { get; set; }
    public DateTime? ExamTo { get; set; }
    public TimeSpan? TimeFrom { get; set; }
    public TimeSpan? TimeTo { get; set; }
    public TimeSpan Duration { get; set; }
    public float PassingPercentage { get; set; } = 0;
    public string Description { get; set; }
    public int? SessionId { get; set; }
    public int PublishResult { get; set; } = 0;
    public string IsActive { get; set; } = "0";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
