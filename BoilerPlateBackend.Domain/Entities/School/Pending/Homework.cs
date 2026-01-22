using System.ComponentModel.DataAnnotations;

public class Homework
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int SessionId { get; set; }

    [Required]
    public DateTime HomeworkDate { get; set; }

    [Required]
    public DateTime SubmitDate { get; set; }

    public int StaffId { get; set; }
    public int? SubjectGroupSubjectId { get; set; }
    public int SubjectId { get; set; }
    public string Description { get; set; }

    [Required]
    public DateTime CreateDate { get; set; }

    public DateTime EvaluationDate { get; set; }

    [Required, MaxLength(200)]
    public string Document { get; set; }

    public int CreatedBy { get; set; }
    public int EvaluatedBy { get; set; }
}
