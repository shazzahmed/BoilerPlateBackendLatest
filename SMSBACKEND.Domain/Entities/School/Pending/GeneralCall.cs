using System.ComponentModel.DataAnnotations;

public class GeneralCall
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required, MaxLength(12)]
    public string Contact { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required, MaxLength(500)]
    public string Description { get; set; }

    [Required]
    public DateTime FollowUpDate { get; set; }

    [Required, MaxLength(50)]
    public string CallDureation { get; set; }

    public string Note { get; set; }

    [Required, MaxLength(20)]
    public string CallType { get; set; }
}
