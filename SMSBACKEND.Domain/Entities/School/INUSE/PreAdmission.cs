using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class PreAdmission : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string ApplicationNumber { get; set; }

        [Required]
        public int EnquiryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        public DateTime? Dob { get; set; }

        [Required]
        public int ApplyingClass { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(200)]
        public string? FatherName { get; set; }

        [MaxLength(20)]
        public string? FatherPhone { get; set; }

        [MaxLength(200)]
        public string? MotherName { get; set; }

        [MaxLength(20)]
        public string? MotherPhone { get; set; }

        [MaxLength(1000)]
        public string? PreviousSchool { get; set; }

        [MaxLength(1000)]
        public string? PreviousClass { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Submitted"; // 'Submitted', 'UnderReview', 'Approved', 'Rejected', 'Converted'

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DateTime2")]
        public DateTime? ReviewDate { get; set; }

        // Fee Configuration
        public int? FeeGroupFeeTypeId { get; set; }  // Selected fee master configuration

        [MaxLength(2000)]
        public string? FeeConfigurationSnapshot { get; set; }  // JSON snapshot of fee structure at time of application

        // Navigation properties
        [ForeignKey("EnquiryId")]
        [JsonIgnore]
        public virtual Enquiry Enquiry { get; set; } = null!;

        [ForeignKey("ApplyingClass")]
        [JsonIgnore]
        public virtual Class Class { get; set; } = null!;

        [ForeignKey("FeeGroupFeeTypeId")]
        [JsonIgnore]
        public virtual FeeGroupFeeType? FeeGroupFeeType { get; set; }

        // Reverse navigation for workflow tracking
        [JsonIgnore]
        public virtual EntryTest? EntryTest { get; set; }
    }
}