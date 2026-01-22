
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Common.Utilities.Enums;

namespace Common.DTO.Response
{
    public class FeeAssignmentModel : BaseClass
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }  // Nullable for provisional fees
        public int? ApplicationId { get; set; }  // For pre-admission fees
        public string? StudentName { get; set; }
        public string? ClassName { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }
        public int? FeeGroupFeeTypeId { get; set; }
        public decimal MonthlyFees { get; set; }
        public int? FeeDiscountId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public FeeStatus Status { get; set; } = FeeStatus.Pending;
        public decimal AmountDiscount { get; set; }
        public decimal AmountFine { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid => PaidAmount >= (Amount + (DateTime.Now > DueDate ? AmountFine : 0) - AmountDiscount);
        public bool IsPartial => PaidAmount > 0 && PaidAmount < (Amount + (DateTime.Now > DueDate ? AmountFine : 0) - AmountDiscount);
        public bool IsOverdue => !IsPaid && DueDate.Date < DateTime.UtcNow.Date;
        public bool IsPartialPaymentAllowed { get; set; } = false;
        public bool IsSelected { get; set; } = false;
        public bool IsDisabled { get; set; } = false;

        // ✅ Fee Package Tracking
        public bool IsInitialAssignment { get; set; } = false;
        public string? InitialPackageId { get; set; }

        [ForeignKey("StudentId")]
        public virtual StudentModel? Student { get; set; }

        [ForeignKey("FeeGroupFeeTypeId")]
        public virtual FeeGroupFeeTypeModel? FeeGroupFeeType { get; set; }
        [ForeignKey("FeeDiscountId")]
        public FeeDiscountModel? AppliedDiscount { get; set; } // optional
        [JsonIgnore]
        public virtual ICollection<FeeTransactionModel>? FeeTransactions { get; set; }
    }
    public class StatusCount
    {
        public string? Status { get; set; }
        public int Count { get; set; }
    }
}
