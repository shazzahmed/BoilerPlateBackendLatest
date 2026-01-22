public class Question
{
    public int Id { get; set; }
    public int? SubjectId { get; set; }
    public string QuestionText { get; set; }
    public string OptA { get; set; }
    public string OptB { get; set; }
    public string OptC { get; set; }
    public string OptD { get; set; }
    public string OptE { get; set; }
    public string Correct { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
