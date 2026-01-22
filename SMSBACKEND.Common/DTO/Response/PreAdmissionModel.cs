using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Common.DTO.Response
{
    public class PreAdmissionModel :  BaseClass
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? ApplicationNumber { get; set; }

        [Required]
        public int EnquiryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string? FullName { get; set; }

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
        public string? Status { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTime ApplicationDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        
        // Fee Configuration
        public int? FeeGroupFeeTypeId { get; set; }
        public string? FeeConfigurationSnapshot { get; set; }

        // Display properties (populated by AutoMapper)
        public string? ClassName { get; set; }
        public string? EnquiryFullName { get; set; }
        public string? FeeGroupName { get; set; }

        [ForeignKey("EnquiryId")]
        [JsonIgnore] // Prevent circular reference (Enquiry already has PreAdmission)
        public virtual EnquiryModel? Enquiry { get; set; }

        [ForeignKey("ApplyingClass")]
        public virtual ClassModel? Class { get; set; }

        [ForeignKey("FeeGroupFeeTypeId")]
        public virtual FeeGroupFeeTypeModel? FeeGroupFeeType { get; set; }

        // Reverse navigation for workflow tracking
        public virtual EntryTestModel? EntryTest { get; set; }
    }
}