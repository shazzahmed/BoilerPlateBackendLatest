using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities
{
    public class StudentInfo : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string AdmissionNo { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }


        [MaxLength(100)]
        public string? LastName { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? BFormNumber { get; set; }

        [MaxLength(50)]
        public string? Religion { get; set; }

        [MaxLength(50)]
        public string? Cast { get; set; }

        [MaxLength(20)]
        public string? PhoneNo { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? MobileNo { get; set; }

        [MaxLength(20)]
        public string? BloodGroup { get; set; }

        [MaxLength(10)]
        public string? Height { get; set; }

        [MaxLength(10)]
        public string? Weight { get; set; }

        public DateTime? MeasurementDate { get; set; }

        public string? Image { get; set; }

        public DateTime? AdmissionDate { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string? RollNo { get; set; }

        public int? SchoolHouseId { get; set; }

        public int? CategoryId { get; set; }

        public string? ParentId { get; set; }

        public string? StudentUserId { get; set; }

        // Navigation properties
        [ForeignKey("SchoolHouseId")]
        [JsonIgnore]
        public virtual SchoolHouse? SchoolHouse { get; set; }

        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public virtual StudentCategory? StudentCategory { get; set; }

        [ForeignKey("ParentId")]
        [JsonIgnore]
        public virtual ApplicationUser? Parent { get; set; }

        [ForeignKey("StudentUserId")]
        [JsonIgnore]
        public virtual ApplicationUser? StudentUser { get; set; }

        // One-to-One relationship with Student
        [JsonIgnore]
        public virtual Student Student { get; set; } = null!;
    }
}