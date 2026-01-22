using System;
using System.Collections.Generic;

namespace Common.DTO.Response.Fees
{
    public class MultiFeePaymentResponse
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string CollectedBy { get; set; } = string.Empty;
        
        public List<FeePaymentResult> PaymentResults { get; set; } = new List<FeePaymentResult>();
        
        public decimal TotalAmountPaid { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalFine { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal GrandTotal { get; set; }
        
        public string ReceiptNumber { get; set; } = string.Empty;
        public bool AllSuccessful { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        
        // Receipt data for printing
        public ReceiptResponse? ReceiptData { get; set; }
    }

    public class FeePaymentResult
    {
        public int FeeAssignmentId { get; set; }
        public int TransactionId { get; set; }
        public string FeeTypeName { get; set; } = string.Empty;
        public string FeeGroupName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

