using System.ComponentModel.DataAnnotations;

public class FeeReminder
{
    [Key]
    public int Id { get; set; }
    [MaxLength(10)]
    public string ReminderType { get; set; }
    public int? Day { get; set; }
    public bool IsActive { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
