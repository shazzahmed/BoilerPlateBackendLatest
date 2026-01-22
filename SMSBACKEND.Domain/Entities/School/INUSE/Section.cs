using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a section (division) within classes
    /// </summary>
    public class Section : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
        
        [JsonIgnore]
        public virtual ICollection<StudentClassAssignment> StudentAssignments { get; set; } = new List<StudentClassAssignment>();
    }
}
