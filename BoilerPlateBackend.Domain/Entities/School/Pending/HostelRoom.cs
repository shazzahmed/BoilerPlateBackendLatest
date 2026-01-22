using System.ComponentModel.DataAnnotations;

public class HostelRoom
{
    public int Id { get; set; }
    public int? HostelId { get; set; }
    public int? RoomTypeId { get; set; }
    [MaxLength(200)]
    public string RoomNo { get; set; }
    public int? NoOfBed { get; set; }
    public float? CostPerBed { get; set; } = 0.0f;
    [MaxLength(200)]
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
