using Application.ServiceContracts.IServices;
using Common.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("Api/[controller]")]
    [ApiController]
    public class PaymentAnalyticsController : ControllerBase
    {
        private readonly IPaymentAnalyticsService _analyticsService;
        private readonly ILogger<PaymentAnalyticsController> _logger;

        public PaymentAnalyticsController(
            IPaymentAnalyticsService analyticsService,
            ILogger<PaymentAnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        /// <summary>
        /// Get revenue analytics with grouping
        /// </summary>
        /// <param name="startDate">Start date (yyyy-MM-dd)</param>
        /// <param name="endDate">End date (yyyy-MM-dd)</param>
        /// <param name="groupBy">Grouping: daily, weekly, monthly, yearly</param>
        [HttpGet]
        [Route("Revenue")]
        [Produces("application/json")]
        public async Task<IActionResult> GetRevenueAnalytics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string groupBy = "monthly")
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now;

                _logger.LogInformation("📊 Analytics: Revenue from {Start} to {End}, grouped by {GroupBy}",
                    start, end, groupBy);

                var result = await _analyticsService.GetRevenueAnalyticsAsync(start, end, groupBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PaymentAnalyticsController] GetRevenueAnalytics error");
                return BadRequest(BaseModel.Failed(message: $"Error getting revenue analytics: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get revenue breakdown by fee type
        /// </summary>
        [HttpGet]
        [Route("RevenueByFeeType")]
        [Produces("application/json")]
        public async Task<IActionResult> GetRevenueByFeeType(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now;

                _logger.LogInformation("📊 Analytics: Revenue by fee type");

                var result = await _analyticsService.GetRevenueByFeeTypeAsync(start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PaymentAnalyticsController] GetRevenueByFeeType error");
                return BadRequest(BaseModel.Failed(message: $"Error getting fee type revenue: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get revenue breakdown by payment method
        /// </summary>
        [HttpGet]
        [Route("PaymentMethodDistribution")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPaymentMethodDistribution(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now;

                _logger.LogInformation("📊 Analytics: Payment method distribution");

                var result = await _analyticsService.GetRevenueByPaymentMethodAsync(start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PaymentAnalyticsController] GetPaymentMethodDistribution error");
                return BadRequest(BaseModel.Failed(message: $"Error getting payment method distribution: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get list of defaulting students
        /// </summary>
        /// <param name="minDaysOverdue">Minimum days overdue (default: 1)</param>
        /// <param name="minAmount">Minimum outstanding amount (default: 0)</param>
        [HttpGet]
        [Route("Defaulters")]
        [Produces("application/json")]
        public async Task<IActionResult> GetDefaultersList(
            [FromQuery] int minDaysOverdue = 1,
            [FromQuery] decimal minAmount = 0)
        {
            try
            {
                _logger.LogInformation("📊 Analytics: Defaulters list (minDays: {MinDays}, minAmount: {MinAmount})",
                    minDaysOverdue, minAmount);

                var result = await _analyticsService.GetDefaultersListAsync(minDaysOverdue, minAmount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PaymentAnalyticsController] GetDefaultersList error");
                return BadRequest(BaseModel.Failed(message: $"Error getting defaulters list: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get defaulters summary statistics
        /// </summary>
        [HttpGet]
        [Route("DefaultersSummary")]
        [Produces("application/json")]
        public async Task<IActionResult> GetDefaultersSummary()
        {
            try
            {
                _logger.LogInformation("📊 Analytics: Defaulters summary");

                var result = await _analyticsService.GetDefaultersSummaryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PaymentAnalyticsController] GetDefaultersSummary error");
                return BadRequest(BaseModel.Failed(message: $"Error getting defaulters summary: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get collection trends over time
        /// </summary>
        /// <param name="period">Period: daily, weekly, monthly</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        [HttpGet]
        [Route("CollectionTrends")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCollectionTrends(
            [FromQuery] string period = "daily",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now;

                _logger.LogInformation("📊 Analytics: Collection trends - {Period}", period);

                var result = await _analyticsService.GetCollectionTrendsAsync(period, start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PaymentAnalyticsController] GetCollectionTrends error");
                return BadRequest(BaseModel.Failed(message: $"Error getting collection trends: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get comprehensive dashboard summary
        /// </summary>
        [HttpGet]
        [Route("DashboardSummary")]
        [Produces("application/json")]
        public async Task<IActionResult> GetDashboardSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddMonths(-1);
                var end = endDate ?? DateTime.Now;

                _logger.LogInformation("📊 Analytics: Dashboard summary from {Start} to {End}", start, end);

                var result = await _analyticsService.GetDashboardSummaryAsync(start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[PaymentAnalyticsController] GetDashboardSummary error");
                return BadRequest(BaseModel.Failed(message: $"Error getting dashboard summary: {ex.Message}"));
            }
        }
    }
}


