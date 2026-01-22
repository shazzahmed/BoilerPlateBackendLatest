using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a staff member (teacher, administrator, support staff)
    /// </summary>
    public class Staff : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int LangId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [MaxLength(50)]
        public string? StaffId { get; set; }

        public string? Image { get; set; }

        [MaxLength(20)]
        public string? CNIC { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FatherName { get; set; }

        [MaxLength(100)]
        public string? MotherName { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        public DateTime? Dob { get; set; }

        public DateTime? DateOfJoining { get; set; }

        [MaxLength(20)]
        public string? ContactNo { get; set; }

        [MaxLength(20)]
        public string? PhoneNo { get; set; }

        [MaxLength(50)]
        public string? Religion { get; set; }

        [MaxLength(20)]
        public string? Zipcode { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? EmergencyContactNo { get; set; }

        [MaxLength(20)]
        public string? MaritalStatus { get; set; }

        [MaxLength(500)]
        public string? LocalAddress { get; set; }

        [MaxLength(500)]
        public string? PermanentAddress { get; set; }

        [MaxLength(200)]
        public string? Qualification { get; set; }

        [MaxLength(200)]
        public string? WorkExp { get; set; }

        [MaxLength(200)]
        public string? PreviousOrganization { get; set; }

        public DateTime? DateOfLeaving { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        // Additional Address Information
        [MaxLength(500)]
        public string? StreetAddress { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        // Employment Information
        [MaxLength(200)]
        public string? Specialization { get; set; }

        [MaxLength(100)]
        public string? Designation { get; set; }

        [MaxLength(50)]
        public string? EmployeeCode { get; set; }

        // System Access
        [MaxLength(50)]
        public string? BiometricId { get; set; }

        [MaxLength(50)]
        public string? RfidCode { get; set; }

        // Banking Information
        [MaxLength(200)]
        public string? AccountTitle { get; set; }

        [MaxLength(50)]
        public string? BankAccountNo { get; set; }

        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(20)]
        public string? IfscCode { get; set; }

        [MaxLength(100)]
        public string? BankBranch { get; set; }

        [MaxLength(50)]
        public string? Payscale { get; set; }

        [MaxLength(20)]
        public string? BasicSalary { get; set; }

        [MaxLength(50)]
        public string? EpfNo { get; set; }

        [MaxLength(50)]
        public string? ContractType { get; set; }

        [MaxLength(50)]
        public string? Shift { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        [MaxLength(1000)]
        public string? Allowances { get; set; }

        [MaxLength(1000)]
        public string? Deductions { get; set; }

        // Social Media
        [MaxLength(200)]
        public string? Facebook { get; set; }

        [MaxLength(200)]
        public string? Twitter { get; set; }

        [MaxLength(200)]
        public string? Linkedin { get; set; }

        [MaxLength(200)]
        public string? Instagram { get; set; }

        // Documents
        public string? Resume { get; set; }

        public string? JoiningLetter { get; set; }

        public string? ResignationLetter { get; set; }

        [MaxLength(200)]
        public string? OtherDocumentName { get; set; }

        public string? OtherDocumentFile { get; set; }

        public string? UserId { get; set; }

        [MaxLength(50)]
        public string? VerificationCode { get; set; }

        [MaxLength(200)]
        public string? ZoomApiKey { get; set; }

        [MaxLength(200)]
        public string? ZoomApiSecret { get; set; }

        public DateTime? DisableAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("DepartmentId")]
        [JsonIgnore]
        public virtual Department Department { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<TeacherSubjects> TeacherSubjects { get; set; } = new List<TeacherSubjects>();

        [JsonIgnore]
        public virtual ICollection<ClassTeacher> ClassTeachers { get; set; } = new List<ClassTeacher>();
    }
}
