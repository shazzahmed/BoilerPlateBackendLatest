using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a timetable entry for a specific class section
    /// </summary>
    public class Timetable : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClassSectionId { get; set; }

        [Required]
        [Range(1, 7)]
        public int DayOfWeek { get; set; } // 1-7 (Monday-Sunday)

        public int? SubjectId { get; set; }

        public int? TeacherId { get; set; }

        [Required]
        [MaxLength(10)]
        public string StartTime { get; set; } = string.Empty;  // Format: "08:00"

        [Required]
        [MaxLength(10)]
        public string EndTime { get; set; } = string.Empty;  // Format: "08:45"

        [Required]
        public int Duration { get; set; } // in minutes

        [MaxLength(50)]
        public string? RoomNumber { get; set; }

        public bool IsBreak { get; set; } = false;

        [MaxLength(100)]
        public string? BreakName { get; set; }

        [Required]
        public int OrderIndex { get; set; } // For ordering entries within a day

        [Required]
        public int SessionId { get; set; }

        // Navigation properties
        [ForeignKey("ClassSectionId")]
        [JsonIgnore]
        public virtual ClassSection ClassSection { get; set; } = null!;

        [ForeignKey("SubjectId")]
        [JsonIgnore]
        public virtual Subject? Subject { get; set; }

        [ForeignKey("TeacherId")]
        [JsonIgnore]
        public virtual Staff? Teacher { get; set; }

        [ForeignKey("SessionId")]
        [JsonIgnore]
        public virtual Session Session { get; set; } = null!;
    }
}
