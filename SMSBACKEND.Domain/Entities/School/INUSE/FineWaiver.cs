using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    /// <summary>
    /// Fine Waiver Entity - Tracks requests to waive (forgive) fines
    /// Requires admin approval for accountability
    /// </summary>
    public class FineWaiver : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // ==================== REFERENCE DATA ====================
        
        /// <summary>
        /// The fee assignment that has the fine
        /// </summary>
        [Required]
        public int FeeAssignmentId { get; set; }

        /// <summary>
        /// Student requesting the waiver
        /// </summary>
        [Required]
        public int StudentId { get; set; }

        // ==================== FINE DETAILS ====================

        /// <summary>
        /// Original fine amount before waiver
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalFineAmount { get; set; }

        /// <summary>
        /// Amount of fine to be waived (can be partial or full)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal WaiverAmount { get; set; }

        /// <summary>
        /// Computed: Remaining fine after waiver
        /// </summary>
        [NotMapped]
        public decimal RemainingFineAmount => OriginalFineAmount - WaiverAmount;

        // ==================== REQUEST DETAILS ====================

        /// <summary>
        /// Reason for requesting the fine waiver
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; }

        /// <summary>
        /// Status: Pending, Approved, Rejected
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Who requested the waiver (username or user ID)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RequestedBy { get; set; }

        /// <summary>
        /// When the waiver was requested
        /// </summary>
        [Required]
        public DateTime RequestedDate { get; set; }

        // ==================== APPROVAL DETAILS ====================

        /// <summary>
        /// Who approved/rejected the waiver (admin username or user ID)
        /// </summary>
        [MaxLength(100)]
        public string ApprovedBy { get; set; }

        /// <summary>
        /// When the waiver was approved/rejected
        /// </summary>
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// Admin's note during approval/rejection
        /// </summary>
        [MaxLength(500)]
        public string ApprovalNote { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================

        /// <summary>
        /// Navigation to the fee assignment
        /// </summary>
        [ForeignKey("FeeAssignmentId")]
        [JsonIgnore]
        public virtual FeeAssignment FeeAssignment { get; set; }

        /// <summary>
        /// Navigation to the student
        /// </summary>
        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual Student Student { get; set; }
    }
}

