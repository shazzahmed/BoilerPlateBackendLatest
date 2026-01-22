using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between Subjects and Classes
    /// </summary>
    public class SubjectClass : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int ClassId { get; set; }

        // Computed properties
        [NotMapped]
        public string? SubjectName => Subject?.Name;

        [NotMapped]
        public string? ClassName => Class?.Name;

        // Navigation properties
        [ForeignKey("ClassId")]
        [JsonIgnore]
        public virtual Class Class { get; set; } = null!;

        [ForeignKey("SubjectId")]
        [JsonIgnore]
        public virtual Subject Subject { get; set; } = null!;
    }
}
