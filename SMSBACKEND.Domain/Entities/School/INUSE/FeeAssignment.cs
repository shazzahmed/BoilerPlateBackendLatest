using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a fee assigned to a specific student for a specific month/year
    /// </summary>
    public class FeeAssignment : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int? StudentId { get; set; }  // Nullable to support provisional assignments

        public int? ApplicationId { get; set; }  // For pre-admission/provisional fees

        public bool IsProvisional { get; set; } = false;  // True until student admission is confirmed

        public int? FeeGroupFeeTypeId { get; set; }

        public int? FeeDiscountId { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; } // 1-12 (January-December)

        [Required]
        public int Year { get; set; } // e.g., 2025

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public FeeStatus Status { get; set; } = FeeStatus.Pending;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountDiscount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountFine { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FinalAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaidAmount { get; set; } = 0;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime DueDate { get; set; }

        // Computed properties
        [NotMapped]
        public bool IsPaid => PaidAmount >= (Amount + (DateTime.Now > DueDate ? AmountFine : 0) - AmountDiscount);

        [NotMapped]
        public bool IsPartial => PaidAmount > 0 && PaidAmount < (Amount + (DateTime.Now > DueDate ? AmountFine : 0) - AmountDiscount);

        [NotMapped]
        public bool IsOverdue => !IsPaid && DueDate.Date < DateTime.UtcNow.Date;

        public bool IsPartialPaymentAllowed { get; set; } = false;

        // ✅ Fee Package Tracking (for student creation flow)
        /// <summary>
        /// Indicates if this fee assignment was created during student admission (from fee package)
        /// vs manually added later by admin
        /// </summary>
        public bool IsInitialAssignment { get; set; } = false;

        /// <summary>
        /// Groups initial fee assignments by package (stores FeeGroupFeeTypeId as string)
        /// All fees from same package selection have same InitialPackageId
        /// Used to replace/edit package-level fees before payment
        /// </summary>
        [MaxLength(50)]
        public string? InitialPackageId { get; set; }

        // Navigation properties
        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student? Student { get; set; }

        [ForeignKey("ApplicationId")]
        [JsonIgnore]
        public virtual PreAdmission? Application { get; set; }

        [ForeignKey("FeeGroupFeeTypeId")]
        [JsonIgnore]
        public virtual FeeGroupFeeType? FeeGroupFeeType { get; set; }

        [ForeignKey("FeeDiscountId")]
        [JsonIgnore]
        public virtual FeeDiscount? AppliedDiscount { get; set; }

        [JsonIgnore]
        public virtual ICollection<FeeTransaction> FeeTransactions { get; set; } = new List<FeeTransaction>();
    }
}
