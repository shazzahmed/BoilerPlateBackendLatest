
using Common.DTO.Response;
using static Common.Utilities.Enums;

namespace Common.DTO.Request
{
    public class FeeTransactionRequest
    {
        public int StudentId { get; set; }
        public int FeeGroupFeeTypeId { get; set; }
        public int FeeAssignmentId { get; set; }
        public int? FeeTransactionId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentMethod { get; set; } // Cash / Bank / Online
        public string? ReferenceNo { get; set; }
        public string? Note { get; set; }
        
        // Notification preferences
        public bool? SendSMS { get; set; }
        public bool? SendEmail { get; set; }
        
        // Audit trail
        public string? CreatedBy { get; set; }
    }
}
