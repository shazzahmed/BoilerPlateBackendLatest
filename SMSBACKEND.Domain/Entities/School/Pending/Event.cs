
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class Event
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public int SectionId { get; set; }
    public int ClassId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Note { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventColor { get; set; } = string.Empty;
    public string EventFor { get; set; } = string.Empty;
    public int RoleId { get; set; }
    [MaxLength(255)]
    public string Photo { get; set; }
    public string EventNotificationMessage { get; set; }
    public bool ShowOnWebsite { get; set; }
}
