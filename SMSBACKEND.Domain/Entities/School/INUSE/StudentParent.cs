using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Domain.Entities
{
    /// <summary>
    /// Stores parent/guardian information for students
    /// 
    /// CURRENT IMPLEMENTATION NOTE:
    /// FatherCNIC and MotherCNIC fields currently store TWO different types of data:
    /// 1. Actual CNIC (string): "12345-1234567-1" (standard format)
    /// 2. Parent Account ID (integer as string): "151", "14" (for schools without parent CNIC)
    /// 
    /// FRONTEND VALIDATION:
    /// - Custom validator accepts EITHER valid CNIC format OR numeric ParentId
    /// - See: student-form-stepper.component.ts -> cnicOrParentIdValidator()
    /// 
    /// FUTURE REFACTORING PLAN (Option 2):
    /// To improve data modeling and type safety, consider adding:
    /// 
    /// public int? ParentAccountId { get; set; } // NEW: Reference to ApplicationUser.Id for parent account
    /// [ForeignKey("ParentAccountId")]
    /// public virtual ApplicationUser? ParentAccount { get; set; } // Navigation property
    /// 
    /// Migration Steps:
    /// 1. Add ParentAccountId column to StudentParent table
    /// 2. Create data migration script:
    ///    - Extract numeric values from FatherCNIC/MotherCNIC
    ///    - Move to ParentAccountId if they're integers
    ///    - Keep only actual CNIC strings in FatherCNIC/MotherCNIC
    /// 3. Update StudentCreateRequest DTO to include ParentAccountId
    /// 4. Update repositories (StudentRepository.CreateStudent, UpdateStudent)
    /// 5. Update services (StudentService validation logic)
    /// 6. Update AutoMapper mappings (StudentModel <-> StudentParent)
    /// 7. Update frontend form to send ParentAccountId separately
    /// 8. Apply regular CNIC validation on FatherCNIC/MotherCNIC
    /// 
    /// Benefits of Refactoring:
    /// - Type-safe: ParentAccountId is int?, CNIC is string?
    /// - Clear intent: No ambiguity about field purposes
    /// - Better queries: Direct navigation to parent ApplicationUser
    /// - Easier validation: Separate validation rules for each field
    /// - Database integrity: Foreign key constraint to ApplicationUser
    /// </summary>
    public class StudentParent : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        // Father Information
        [MaxLength(200)]
        public string? FatherName { get; set; }

        [MaxLength(20)]
        public string? FatherPhone { get; set; }

        [MaxLength(100)]
        public string? FatherOccupation { get; set; }

        /// <summary>
        /// Father's CNIC or Parent Account ID (string)
        /// CURRENT: Accepts EITHER "12345-1234567-1" (CNIC) OR "151" (ParentId)
        /// FUTURE: Should only contain actual CNIC after refactoring
        /// </summary>
        [MaxLength(20)]
        public string? FatherCNIC { get; set; }

        public string? FatherPic { get; set; }

        // Mother Information
        [MaxLength(200)]
        public string? MotherName { get; set; }

        [MaxLength(20)]
        public string? MotherPhone { get; set; }

        [MaxLength(100)]
        public string? MotherOccupation { get; set; }

        /// <summary>
        /// Mother's CNIC or Parent Account ID (string)
        /// CURRENT: Accepts EITHER "12345-1234567-1" (CNIC) OR "151" (ParentId)
        /// FUTURE: Should only contain actual CNIC after refactoring
        /// </summary>
        [MaxLength(20)]
        public string? MotherCNIC { get; set; }

        public string? MotherPic { get; set; }

        // Guardian Information
        [MaxLength(50)]
        public string? GuardianIs { get; set; }

        [MaxLength(200)]
        public string? GuardianName { get; set; }

        [MaxLength(100)]
        public string? GuardianRelation { get; set; }

        [MaxLength(100)]
        public string? GuardianEmail { get; set; }

        [MaxLength(20)]
        public string? GuardianPhone { get; set; }

        [MaxLength(100)]
        public string? GuardianOccupation { get; set; }

        [MaxLength(500)]
        public string? GuardianAddress { get; set; }

        [MaxLength(500)]
        public string? GuardianAreaAddress { get; set; }

        public string? GuardianPic { get; set; }

        // Navigation property
        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; } = null!;
    }
}