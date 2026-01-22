using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Represents an academic subject
    /// </summary>
    public class Subject : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public SubjectTypes Type { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<TeacherSubjects> TeacherSubjects { get; set; } = new List<TeacherSubjects>();
        
        [JsonIgnore]
        public virtual ICollection<SubjectClass> SubjectClasses { get; set; } = new List<SubjectClass>();
    }
}
