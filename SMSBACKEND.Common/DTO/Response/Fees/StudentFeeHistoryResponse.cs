using System;
using System.Collections.Generic;

namespace Common.DTO.Response.Fees
{
    /// <summary>
    /// Complete fee history for a student (paid + unpaid)
    /// </summary>
    public class StudentFeeHistoryResponse
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string ClassName { get; set; }
        
        // Summary
        public decimal TotalAssignedAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalPendingAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalFineAmount { get; set; }
        
        // Fees breakdown
        public List<StudentFeeItem> PaidFees { get; set; } = new List<StudentFeeItem>();
        public List<StudentFeeItem> UnpaidFees { get; set; } = new List<StudentFeeItem>();
        public List<StudentFeeItem> PartiallyPaidFees { get; set; } = new List<StudentFeeItem>();
        public List<StudentFeeItem> OverdueFees { get; set; } = new List<StudentFeeItem>();
        
        // Recent transactions
        public List<FeeTransactionItem> RecentTransactions { get; set; } = new List<FeeTransactionItem>();
    }

    public class StudentFeeItem
    {
        public int FeeAssignmentId { get; set; }
        public int? FeeGroupFeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public string FeeGroupName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime DueDate { get; set; }
        
        // Amounts
        public decimal AssignedAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FineAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceDue { get; set; }
        
        // Status
        public string Status { get; set; } // Paid, Unpaid, Partial, Overdue
        public bool IsOverdue { get; set; }
        public int DaysOverdue { get; set; }
        
        // Transactions for this fee
        public List<FeeTransactionItem> Transactions { get; set; } = new List<FeeTransactionItem>();
    }

    public class FeeTransactionItem
    {
        public int TransactionId { get; set; }
        public int FeeAssignmentId { get; set; }
        public string FeeTypeName { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNo { get; set; }
        public string Note { get; set; }
        public string CollectedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// System-wide payment history with filters
    /// </summary>
    public class PaymentHistoryReportResponse
    {
        public List<PaymentReportItem> Payments { get; set; } = new List<PaymentReportItem>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        
        // Summary
        public decimal TotalAmountCollected { get; set; }
        public decimal TotalDiscountGiven { get; set; }
        public decimal TotalFineCollected { get; set; }
        
        // Grouping
        public Dictionary<string, decimal> PaymentsByMethod { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> PaymentsByClass { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, decimal> PaymentsByFeeType { get; set; } = new Dictionary<string, decimal>();
    }

    public class PaymentReportItem
    {
        public int TransactionId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string ClassName { get; set; }
        public string FeeTypeName { get; set; }
        public string FeeGroupName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNo { get; set; }
        public string CollectedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Filter parameters for payment history
    /// </summary>
    public class PaymentHistoryFilterRequest
    {
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public int? FeeTypeId { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; } // Paid, Unpaid, Partial, Overdue, All
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string SortBy { get; set; } = "PaymentDate";
        public string SortDirection { get; set; } = "DESC";
    }
}

