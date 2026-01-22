using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents the assignment of a student to a specific class, section, and session
    /// Uses composite primary key (StudentId, ClassId, SectionId, SessionId)
    /// </summary>
    public class StudentClassAssignment : BaseEntity
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public int SessionId { get; set; }

        // Additional Details
        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime AssignedDate { get; set; } = DateTime.Now;  // When the assignment was made

        [MaxLength(500)]
        public string? Remarks { get; set; }  // Optional remarks

        // Navigation Properties
        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey("ClassId")]
        [JsonIgnore]
        public virtual Class Class { get; set; } = null!;

        [ForeignKey("SectionId")]
        [JsonIgnore]
        public virtual Section Section { get; set; } = null!;

        [ForeignKey("SessionId")]
        [JsonIgnore]
        public virtual Session Session { get; set; } = null!;
    }
}
