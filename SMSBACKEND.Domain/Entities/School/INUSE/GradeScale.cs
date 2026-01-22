using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class GradeScale : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string GradeScaleName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsDefault { get; set; } = false;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}


