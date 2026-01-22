using System.ComponentModel.DataAnnotations;

public class Hostel
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string HostelName { get; set; }
    [MaxLength(50)]
    public string Type { get; set; }
    public string Address { get; set; }
    public int? Intake { get; set; }
    public string Description { get; set; }
    [MaxLength(255)]
    public string IsActive { get; set; } = "no";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
