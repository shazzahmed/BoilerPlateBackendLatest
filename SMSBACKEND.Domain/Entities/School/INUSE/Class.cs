using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents an academic class/grade level
    /// </summary>
    public class Class : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
        
        [JsonIgnore]
        public virtual ICollection<SubjectClass> SubjectClasses { get; set; } = new List<SubjectClass>();
        
        [JsonIgnore]
        public virtual ICollection<StudentClassAssignment> StudentAssignments { get; set; } = new List<StudentClassAssignment>();
    }
}
