using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents student categories (e.g., Regular, Scholarship, Special Needs)
    /// </summary>
    public class StudentCategory : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<StudentInfo> Students { get; set; } = new List<StudentInfo>();
    }
}
