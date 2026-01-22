using System;
using System.Collections.Generic;

namespace Common.DTO.Request
{
    public class MultiFeePaymentRequest
    {
        public int StudentId { get; set; }
        public List<FeePaymentItem> FeePayments { get; set; } = new List<FeePaymentItem>();
        public DateTime PaymentDate { get; set; }
        public string CollectedBy { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        
        // Notification preferences
        public bool SendSMS { get; set; }
        public bool SendEmail { get; set; }
        
        // Advance payment support
        public bool IncludeAdvancePayment { get; set; }
        public decimal AdvanceAmount { get; set; }
    }

    public class FeePaymentItem
    {
        public int FeeAssignmentId { get; set; }
        public int FeeGroupFeeTypeId { get; set; }
        public decimal AmountPaying { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal FineApplied { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string ReferenceNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }

    public class AdvancePaymentRequest
    {
        public int StudentId { get; set; }
        public decimal AdvanceAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        
        // Notification preferences
        public bool SendSMS { get; set; }
        public bool SendEmail { get; set; }
        
        // Audit trail
        public string CreatedBy { get; set; } = string.Empty;
        
        // Allocate to specific fees or leave unallocated
        public List<int> FeeAssignmentIds { get; set; } = new List<int>();
        public bool AutoAllocate { get; set; }
    }
}

