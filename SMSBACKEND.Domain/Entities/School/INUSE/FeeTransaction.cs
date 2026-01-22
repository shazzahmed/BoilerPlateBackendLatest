using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a fee payment transaction from a student
    /// </summary>
    public class FeeTransaction : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // StudentId is nullable to support provisional fees (ApplicationId is used instead)
        public int? StudentId { get; set; }

        public int? ApplicationId { get; set; }  // For pre-admission/provisional fees

        [Required]
        public int FeeGroupFeeTypeId { get; set; }

        [Required]
        public int FeeAssignmentId { get; set; }

        // Month is optional (0 for non-monthly fees like one-time, annual)
        [Range(0, 12)]
        public int Month { get; set; } // 0 = non-monthly, 1-12 (January-December)

        public int Year { get; set; } // e.g., 2025

        [Required]
        public TransactionStatus Status { get; set; } = TransactionStatus.Completed;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountPaid { get; set; }

        [Required]
        [Column(TypeName = "DateTime2")]
        public DateTime PaymentDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;  // Cash, Bank, Online

        [MaxLength(100)]
        public string? ReferenceNo { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountApplied { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FineApplied { get; set; } = 0;

        // Advance Payment Support
        public bool IsAdvancePayment { get; set; } = false;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal RemainingBalance { get; set; } = 0; // For advance payments not yet applied

        // Navigation properties
        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student? Student { get; set; } // Nullable for provisional fees

        [ForeignKey("FeeGroupFeeTypeId")]
        [JsonIgnore]
        public virtual FeeGroupFeeType FeeGroupFeeType { get; set; } = null!;

        [ForeignKey("FeeAssignmentId")]
        [JsonIgnore]
        public virtual FeeAssignment FeeAssignment { get; set; } = null!;
    }
}
