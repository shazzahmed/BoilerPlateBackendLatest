using Common.DTO.Response.Dashboard;

namespace Application.ServiceContracts.IServices
{
    /// <summary>
    /// Dashboard Service Interface
    /// Provides methods for fetching role-based dashboard data
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Get complete dashboard data for Admin role
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <param name="tenantId">Current tenant ID</param>
        /// <param name="dateRange">Date range for charts (optional, defaults to current month)</param>
        /// <returns>Admin dashboard response with metrics, charts, and activities</returns>
        Task<AdminDashboardResponse> GetAdminDashboardAsync(string userId, int tenantId, string dateRange = "month");

        /// <summary>
        /// Get dashboard data for Teacher role
        /// </summary>
        /// <param name="teacherId">Teacher's staff ID</param>
        /// <param name="tenantId">Current tenant ID</param>
        /// <returns>Teacher dashboard response with schedule and assignments</returns>
        Task<TeacherDashboardResponse> GetTeacherDashboardAsync(int teacherId, int tenantId);

        /// <summary>
        /// Get dashboard data for Parent role
        /// </summary>
        /// <param name="parentId">Parent's ID</param>
        /// <param name="tenantId">Current tenant ID</param>
        /// <returns>Parent dashboard response with children info and fees</returns>
        Task<ParentDashboardResponse> GetParentDashboardAsync(int parentId, int tenantId);

        /// <summary>
        /// Get chart data for specific chart type and date range
        /// </summary>
        /// <param name="chartType">Type of chart ("fee", "attendance", "exam")</param>
        /// <param name="dateRange">Date range ("day", "week", "month", "year")</param>
        /// <param name="tenantId">Current tenant ID</param>
        /// <returns>Chart data object</returns>
        Task<object> GetChartDataAsync(string chartType, string dateRange, int tenantId);

        /// <summary>
        /// Get real-time metric update for SignalR broadcast
        /// </summary>
        /// <param name="metricType">Type of metric to update</param>
        /// <param name="tenantId">Current tenant ID</param>
        /// <returns>Metric update object</returns>
        Task<MetricUpdate> GetMetricUpdateAsync(string metricType, int tenantId);
    }
}

