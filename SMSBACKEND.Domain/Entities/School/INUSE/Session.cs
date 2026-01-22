using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents an academic session/year (e.g., 2024-2025)
    /// </summary>
    public class Session : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Sessions { get; set; } = string.Empty;  // e.g., "2024-2025"

        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime EndDate { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<StudentClassAssignment> StudentAssignments { get; set; } 
            = new List<StudentClassAssignment>();
    }
}
