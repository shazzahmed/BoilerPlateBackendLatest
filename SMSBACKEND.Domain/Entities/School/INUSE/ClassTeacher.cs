using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents the assignment of a teacher to a specific class section
    /// </summary>
    public class ClassTeacher : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClassSectionId { get; set; }

        [Required]
        public int StaffId { get; set; }

        [Required]
        public int SessionId { get; set; }

        // Navigation properties
        [ForeignKey("ClassSectionId")]
        [JsonIgnore]
        public virtual ClassSection ClassSection { get; set; } = null!;

        [ForeignKey("StaffId")]
        [JsonIgnore]
        public virtual Staff Staff { get; set; } = null!;

        [ForeignKey("SessionId")]
        [JsonIgnore]
        public virtual Session Session { get; set; } = null!;
    }
}