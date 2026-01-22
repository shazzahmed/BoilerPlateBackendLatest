using System.ComponentModel.DataAnnotations;

public class HomeworkEvaluation
{
    public int Id { get; set; }
    public int HomeworkId { get; set; }
    public int StudentId { get; set; }
    public int? StudentSessionId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required, MaxLength(100)]
    public string Status { get; set; }
}
