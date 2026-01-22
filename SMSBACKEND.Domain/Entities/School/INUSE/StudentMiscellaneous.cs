using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities
{
    public class StudentMiscellaneous : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [MaxLength(100)]
        public string? PreviousSchool { get; set; }

        [MaxLength(500)]
        public string? MedicalHistory { get; set; }

        [MaxLength(100)]
        public string? PickupPerson { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        public int? DisReason { get; set; }

        [MaxLength(1000)]
        public string? DisNote { get; set; }

        public DateTime? DisableAt { get; set; }

        // Navigation property
        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; } = null!;
    }
}
