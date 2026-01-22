public class SubjectSyllabus
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public int SessionId { get; set; }
    public int CreatedBy { get; set; }
    public int CreatedFor { get; set; }
    public DateTime Date { get; set; }
    public string TimeFrom { get; set; }
    public string TimeTo { get; set; }
    public string Presentation { get; set; }
    public string Attachment { get; set; }
    public string LactureYoutubeUrl { get; set; }
    public string LactureVideo { get; set; }
    public string SubTopic { get; set; }
    public string TeachingMethod { get; set; }
    public string GeneralObjectives { get; set; }
    public string PreviousKnowledge { get; set; }
    public string ComprehensiveQuestions { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
