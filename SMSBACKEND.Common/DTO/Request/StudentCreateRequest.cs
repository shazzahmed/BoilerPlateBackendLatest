using System.ComponentModel.DataAnnotations;
using Common.DTO.Response;

namespace Common.DTO.Request
{
    /// <summary>
    /// Unified flat structure for all student creation flows
    /// Supports: Direct creation, Admission flow, Workflow flow
    /// </summary>
    public class StudentCreateRequest : BaseClass
    {
        public int Id { get; set; }
        // ==================== FLOW IDENTIFICATION ====================
        /// <summary>
        /// Source of student creation: 'direct', 'admission', 'workflow', 'preadmission'
        /// </summary>
        public string Source { get; set; } = "direct";
        
        /// <summary>
        /// Admission ID if creating from admission flow
        /// </summary>
        public int? AdmissionId { get; set; }
        
        /// <summary>
        /// PreAdmission/Application ID if creating from pre-admission flow
        /// </summary>
        public int? ApplicationId { get; set; }
        
        /// <summary>
        /// Workflow data if creating from workflow
        /// </summary>
        public string? WorkflowData { get; set; }

        // ==================== PERSONAL INFORMATION ====================
        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; } = string.Empty;
        
        /// <summary>
        /// Full name of the student (computed from FirstName + LastName)
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        [MaxLength(200)]
        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? BFormNumber { get; set; }

        [MaxLength(50)]
        public string? Religion { get; set; }

        [MaxLength(50)]
        public string? Cast { get; set; }

        public string? Image { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; } = DateTime.Now; // Made mandatory, defaults to current date

        [MaxLength(20)]
        public string? Height { get; set; }

        [MaxLength(20)]
        public string? Weight { get; set; }

        [Required]
        public DateTime MeasurementDate { get; set; } = DateTime.Now; // Made mandatory, defaults to current date

        [MaxLength(10)]
        public string? BloodGroup { get; set; }

        [MaxLength(1000)]
        public string? MedicalNotes { get; set; }
        
        [MaxLength(1000)]
        public string? Note { get; set; }

        // ==================== ACADEMIC INFORMATION ====================
        [Required]
        public int ClassId { get; set; }

        [Required]
        public int SectionId { get; set; }

        public int? CategoryId { get; set; }

        public int? SchoolHouseId { get; set; }

        [MaxLength(1000)]
        public string? PreviousSchool { get; set; }

        [MaxLength(1000)]
        public string? MedicalHistory { get; set; }

        [Required]
        [MaxLength(200)]
        public string PickupPerson { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? AdmissionNo { get; set; }

        [MaxLength(50)]
        public string? RollNo { get; set; }

        // StudentId removed - not shown in form

        // ==================== CONTACT INFORMATION ====================
        [MaxLength(20)]
        public string? PhoneNo { get; set; }

        [MaxLength(20)]
        public string? MobileNo { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(500)]
        public string CurrentAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? AreaAddress { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? ZipCode { get; set; }

        [MaxLength(500)]
        public string? PermanentAddress { get; set; }

        // ==================== PARENT/GUARDIAN INFORMATION ====================
        // Father Information
        [Required]
        [MaxLength(20)]
        public string FatherCNIC { get; set; } = string.Empty; // Made mandatory

        [Required]
        [MaxLength(200)]
        public string FatherName { get; set; } = string.Empty; // Made mandatory

        [MaxLength(20)]
        public string? FatherPhone { get; set; }

        [MaxLength(100)]
        public string? FatherOccupation { get; set; }

        public string? FatherPic { get; set; }

        // Mother Information
        [MaxLength(20)]
        public string? MotherCNIC { get; set; }

        [MaxLength(200)]
        public string? MotherName { get; set; }

        [MaxLength(20)]
        public string? MotherPhone { get; set; }

        [MaxLength(100)]
        public string? MotherOccupation { get; set; }

        public string? MotherPic { get; set; }

        // Guardian Information
        [Required]
        [MaxLength(20)]
        public string GuardianType { get; set; } = string.Empty; // Made mandatory

        [Required]
        [MaxLength(200)]
        public string GuardianName { get; set; } = string.Empty; // Made mandatory

        [Required]
        [MaxLength(20)]
        public string GuardianPhone { get; set; } = string.Empty; // Made mandatory

        [MaxLength(200)]
        public string? GuardianRelation { get; set; }

        [MaxLength(100)]
        public string? GuardianEmail { get; set; }

        [MaxLength(100)]
        public string? GuardianOccupation { get; set; }

        [MaxLength(500)]
        public string? GuardianAddress { get; set; }

        [MaxLength(500)]
        public string? GuardianAreaAddress { get; set; }

        public string? GuardianPic { get; set; }

        // ==================== ACCOUNT INFORMATION ====================
        [MaxLength(450)]
        public string? ParentId { get; set; }

        [MaxLength(450)]
        public string? StudentUserId { get; set; }

        // ==================== FEES INFORMATION ====================
        public decimal? MonthlyFees { get; set; }

        public decimal? AdmissionFees { get; set; }

        public decimal? SecurityDeposit { get; set; }

        // ==================== FEE PACKAGE CONFIGURATION ====================
        /// <summary>
        /// Selected fee package for automatic fee assignment
        /// </summary>
        public int? FeeGroupFeeTypeId { get; set; }

        /// <summary>
        /// Fee configuration snapshot (JSON) for audit trail
        /// </summary>
        public string? FeeConfigurationSnapshot { get; set; }

        /// <summary>
        /// For monthly fees: which month to apply (1-12)
        /// </summary>
        public int? ApplyMonth { get; set; }

        /// <summary>
        /// For monthly fees: which year to apply (e.g., 2025)
        /// </summary>
        public int? ApplyYear { get; set; }

        /// <summary>
        /// For monthly fees: create full year plan (12 months)
        /// </summary>
        public bool CreateFullYearPlan { get; set; } = false;

        /// <summary>
        /// For monthly fees: number of months to create (if not full year)
        /// </summary>
        public int? NumberOfMonths { get; set; }

        /// <summary>
        /// Package ID to remove (for edit mode - removes all assignments for this package)
        /// </summary>
        public string? PackageIdToRemove { get; set; }

        // ==================== SYSTEM FIELDS ====================
        public int SessionId { get; set; } = 1; // Default session

        [MaxLength(50)]
        public string CreatedBy { get; set; } = "System";
    }
}

