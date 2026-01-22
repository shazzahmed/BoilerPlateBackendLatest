using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Admission : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string AdmissionNumber { get; set; } = string.Empty;

        [Required]
        public int EntryTestId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Inactive, Transferred, Withdrawn

        [StringLength(500)]
        public string? Remarks { get; set; }

        // Student tracking fields
        public bool IsStudentCreated { get; set; } = false;
        public int? StudentId { get; set; }

        // Navigation properties
        [ForeignKey("EntryTestId")]
        [JsonIgnore]
        public virtual EntryTest EntryTest { get; set; } = null!;

        [ForeignKey("ClassId")]
        [JsonIgnore]
        public virtual Class Class { get; set; } = null!;

        [ForeignKey("SectionId")]
        [JsonIgnore]
        public virtual Section Section { get; set; } = null!;

        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student? Student { get; set; }
    }
}
