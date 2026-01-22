using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between Teachers (Staff) and Subjects
    /// </summary>
    public class TeacherSubjects : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        // Navigation properties
        [ForeignKey("SubjectId")]
        [JsonIgnore]
        public virtual Subject Subject { get; set; } = null!;

        [ForeignKey("TeacherId")]
        [JsonIgnore]
        public virtual Staff Teacher { get; set; } = null!;
    }
}
