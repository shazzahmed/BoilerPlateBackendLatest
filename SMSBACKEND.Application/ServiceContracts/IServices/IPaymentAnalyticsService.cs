using Common.DTO.Response;
using Common.DTO.Response.Analytics;

namespace Application.ServiceContracts.IServices
{
    /// <summary>
    /// Payment Analytics Service Interface
    /// Provides comprehensive analytics for payment tracking and revenue analysis
    /// </summary>
    public interface IPaymentAnalyticsService
    {
        /// <summary>
        /// Get revenue analytics for a date range with grouping
        /// </summary>
        /// <param name="startDate">Start date for analysis</param>
        /// <param name="endDate">End date for analysis</param>
        /// <param name="groupBy">Grouping: daily, weekly, monthly, yearly</param>
        Task<BaseModel> GetRevenueAnalyticsAsync(DateTime startDate, DateTime endDate, string groupBy);

        /// <summary>
        /// Get revenue breakdown by fee type
        /// </summary>
        Task<BaseModel> GetRevenueByFeeTypeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get revenue breakdown by payment method
        /// </summary>
        Task<BaseModel> GetRevenueByPaymentMethodAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get list of defaulting students
        /// </summary>
        /// <param name="minDaysOverdue">Minimum days overdue to be considered defaulter</param>
        /// <param name="minAmount">Minimum outstanding amount</param>
        Task<BaseModel> GetDefaultersListAsync(int minDaysOverdue = 1, decimal minAmount = 0);

        /// <summary>
        /// Get defaulters summary statistics
        /// </summary>
        Task<BaseModel> GetDefaultersSummaryAsync();

        /// <summary>
        /// Get collection trends over time
        /// </summary>
        /// <param name="period">Period: daily, weekly, monthly</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        Task<BaseModel> GetCollectionTrendsAsync(string period, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get comprehensive dashboard summary
        /// </summary>
        Task<BaseModel> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate);
    }
}


