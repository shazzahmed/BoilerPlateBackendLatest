
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class FeeTransactionModel : BaseClass
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? ApplicationId { get; set; }  // For pre-admission/provisional fees
        public int FeeGroupFeeTypeId { get; set; }
        public int FeeAssignmentId { get; set; }
        public int Month { get; set; } // e.g., 7 for July
        public int Year { get; set; } // e.g., 2025
        public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; } // Cash, Bank, Online
        public string? ReferenceNo { get; set; }
        public string? Note { get; set; }

        public decimal DiscountApplied { get; set; }
        public decimal FineApplied { get; set; }

        [ForeignKey("StudentId")]
        [JsonIgnore]
        public virtual StudentModel? Student { get; set; }

        [ForeignKey("FeeGroupFeeTypeId")]
        [JsonIgnore]
        public virtual FeeGroupFeeTypeModel? FeeGroupFeeType { get; set; }

        [ForeignKey("FeeAssignmentId")]
        [JsonIgnore]
        public virtual FeeAssignmentModel? FeeAssignment { get; set; }
    }
}
