using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a department within the school (e.g., Science, Arts, Administration)
    /// </summary>
    public class Department : BaseEntity
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
        public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
    }
}
