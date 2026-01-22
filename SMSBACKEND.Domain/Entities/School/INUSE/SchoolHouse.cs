using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a house/team within the school for competitions and activities
    /// </summary>
    public class SchoolHouse : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; }  // House color for branding

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<StudentInfo> Students { get; set; } = new List<StudentInfo>();
    }
}
