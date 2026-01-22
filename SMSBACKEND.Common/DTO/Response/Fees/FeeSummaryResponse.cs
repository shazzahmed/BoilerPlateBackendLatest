using static Common.Utilities.Enums;

namespace Common.DTO.Response.Fees
{
    /// <summary>
    /// Comprehensive Fee Summary for Payment Modal
    /// Shows all details needed for payment collection
    /// </summary>
    public class FeeSummaryResponse
    {
        public int FeeAssignmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        
        // Fee Details
        public string FeeGroupName { get; set; }
        public string FeeTypeName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsOverdue { get; set; }
        public int OverdueDays { get; set; }
        
        // Amount Breakdown
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountName { get; set; }
        public decimal FineAmount { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal AlreadyPaid { get; set; }
        public decimal BalanceDue { get; set; }
        
        // Status
        public string Status { get; set; } // "Paid", "Partial", "Pending", "Overdue"
        public bool IsPartialAllowed { get; set; }
        public bool IsPaid { get; set; }
        public bool IsPartial { get; set; }
        
        // Payment History
        public List<PaymentHistoryItem> PaymentHistory { get; set; }
        
        // Additional Info
        public string Description { get; set; }
        public int FeeGroupFeeTypeId { get; set; }
    }

    /// <summary>
    /// Payment History Item
    /// </summary>
    public class PaymentHistoryItem
    {
        public int TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNo { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceivedBy { get; set; }
        public string Note { get; set; }
    }

    /// <summary>
    /// Student Pending Fees Response
    /// Returns all pending fees for a student
    /// </summary>
    public class StudentPendingFeesResponse
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public List<FeeSummaryResponse> PendingFees { get; set; }
        public decimal TotalPendingAmount { get; set; }
        public decimal TotalOverdueAmount { get; set; }
        public int PendingCount { get; set; }
        public int OverdueCount { get; set; }
    }

    // Note: MultiFeePaymentRequest, FeePaymentItem, and AdvancePaymentRequest
    // are now in Common.DTO.Request namespace to avoid ambiguity

    /// <summary>
    /// Receipt Generation Response
    /// </summary>
    public class ReceiptResponse
    {
        public string ReceiptNo { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string ClassName { get; set; }
        public string FatherName { get; set; }
        
        // School Details
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolPhone { get; set; }
        public string SchoolEmail { get; set; }
        public string SchoolLogo { get; set; }
        
        // Payment Details
        public List<ReceiptFeeItem> FeeItems { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalFine { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNo { get; set; }
        
        // Additional
        public string ReceivedBy { get; set; }
        public string AmountInWords { get; set; }
        public string Note { get; set; }
    }

    /// <summary>
    /// Receipt Fee Item
    /// </summary>
    public class ReceiptFeeItem
    {
        public string FeeType { get; set; }
        public string Period { get; set; } // "October 2025"
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public decimal TotalPaid { get; set; }
    }

    /// <summary>
    /// Payment Gateway Request (for online payments)
    /// </summary>
    public class PaymentGatewayRequest
    {
        public int StudentId { get; set; }
        public List<int> FeeAssignmentIds { get; set; }
        public decimal Amount { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }

    /// <summary>
    /// Payment Gateway Response
    /// </summary>
    public class PaymentGatewayResponse
    {
        public string TransactionId { get; set; }
        public string PaymentUrl { get; set; }
        public string Status { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}

