using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between Classes and Sections
    /// </summary>
    public class ClassSection : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        // Computed properties
        [NotMapped]
        public string? ClassName => Class?.Name;

        [NotMapped]
        public string? SectionName => Section?.Name;

        // Navigation properties
        [ForeignKey("ClassId")]
        [JsonIgnore]
        public virtual Class Class { get; set; } = null!;

        [ForeignKey("SectionId")]
        [JsonIgnore]
        public virtual Section Section { get; set; } = null!;
    }
}
