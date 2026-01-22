using System;
using System.Collections.Generic;

namespace Common.DTO.Response.Analytics
{
    /// <summary>
    /// Revenue Analytics Response
    /// </summary>
    public class RevenueAnalyticsResponse
    {
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public List<RevenueDataPoint> DataPoints { get; set; } = new List<RevenueDataPoint>();
    }

    public class RevenueDataPoint
    {
        public string Period { get; set; } = string.Empty; // "2025-10-17" or "Oct 2025"
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }

    /// <summary>
    /// Defaulters List Response
    /// </summary>
    public class DefaultersListResponse
    {
        public List<DefaulterInfo> Defaulters { get; set; } = new List<DefaulterInfo>();
        public int TotalDefaulters { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalFines { get; set; }
    }

    public class DefaulterInfo
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string AdmissionNo { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal OutstandingAmount { get; set; }
        public decimal FineAmount { get; set; }
        public int DaysOverdue { get; set; }
        public DateTime OldestDueDate { get; set; }
        public List<string> OverdueFeeTypes { get; set; } = new List<string>();
    }

    /// <summary>
    /// Payment Method Distribution Response
    /// </summary>
    public class PaymentMethodDistributionResponse
    {
        public List<PaymentMethodData> Methods { get; set; } = new List<PaymentMethodData>();
        public decimal TotalAmount { get; set; }
        public int TotalTransactions { get; set; }
    }

    public class PaymentMethodData
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Fee Type Revenue Response
    /// </summary>
    public class FeeTypeRevenueResponse
    {
        public List<FeeTypeRevenueData> FeeTypes { get; set; } = new List<FeeTypeRevenueData>();
        public decimal TotalRevenue { get; set; }
    }

    public class FeeTypeRevenueData
    {
        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Dashboard Summary Response
    /// </summary>
    public class DashboardSummaryResponse
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalPending { get; set; }
        public decimal TotalFines { get; set; }
        public decimal TotalDiscounts { get; set; }
        public int TotalStudents { get; set; }
        public int PayingStudents { get; set; }
        public int DefaultingStudents { get; set; }
        public decimal CollectionRate { get; set; } // Percentage
        public RevenueAnalyticsResponse RevenueAnalytics { get; set; } = new RevenueAnalyticsResponse();
        public PaymentMethodDistributionResponse PaymentMethodDistribution { get; set; } = new PaymentMethodDistributionResponse();
    }

    /// <summary>
    /// Collection Trends Response
    /// </summary>
    public class CollectionTrendsResponse
    {
        public string Period { get; set; } = string.Empty; // daily, weekly, monthly
        public List<TrendDataPoint> Trends { get; set; } = new List<TrendDataPoint>();
        public decimal TotalAmount { get; set; }
        public decimal AveragePerPeriod { get; set; }
        public decimal HighestCollection { get; set; }
        public decimal LowestCollection { get; set; }
    }

    public class TrendDataPoint
    {
        public string PeriodLabel { get; set; } = string.Empty; // "Mon", "Week 1", "Jan"
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }
}


