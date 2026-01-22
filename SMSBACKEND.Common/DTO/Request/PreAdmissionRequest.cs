using Common.DTO.Response;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Request
{
    public class PreAdmissionRequest : BaseClass
    {
        [Required]
        [MaxLength(50)]
        public string ApplicationNumber { get; set; }

        [Required]
        public int EnquiryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        public DateTime? Dob { get; set; }

        [Required]
        public int ApplyingClass { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(200)]
        public string FatherName { get; set; }

        [MaxLength(20)]
        public string FatherPhone { get; set; }

        [MaxLength(200)]
        public string MotherName { get; set; }

        [MaxLength(20)]
        public string MotherPhone { get; set; }

        [MaxLength(1000)]
        public string PreviousSchool { get; set; }

        [MaxLength(1000)]
        public string PreviousClass { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Submitted";

        [MaxLength(1000)]
        public string Notes { get; set; }

        // Fee Configuration
        public int? FeeGroupFeeTypeId { get; set; }  // Selected fee master configuration
        public string FeeConfigurationSnapshot { get; set; }  // JSON snapshot
    }

    public class PreAdmissionCreateRequest : PreAdmissionRequest
    {
        // Inherits all properties from PreAdmissionRequest
    }

    public class PreAdmissionUpdateRequest : PreAdmissionRequest
    {
        [Required]
        public int Id { get; set; }
    }
}
