using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Common.DTO.Response;
using Common.DTO.Response.Analytics;
using Infrastructure.Database;
using Infrastructure.Services.Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Infrastructure.Services.Services
{
    public class PaymentAnalyticsService : IPaymentAnalyticsService
    {
        private readonly SqlServerDbContext _context;
        private readonly ILogger<PaymentAnalyticsService> _logger;
        private readonly ICacheProvider _cacheProvider;

        public PaymentAnalyticsService(
            SqlServerDbContext context,
            ILogger<PaymentAnalyticsService> logger,
            ICacheProvider cacheProvider)
        {
            _context = context;
            _logger = logger;
            _cacheProvider = cacheProvider;
        }

        public async Task<BaseModel> GetRevenueAnalyticsAsync(DateTime startDate, DateTime endDate, string groupBy)
        {
            try
            {
                _logger.LogInformation("📊 Getting revenue analytics from {StartDate} to {EndDate}, grouped by {GroupBy}",
                    startDate, endDate, groupBy);

                var transactions = await _context.FeeTransaction
                    .Where(t => t.PaymentDate >= startDate && 
                               t.PaymentDate <= endDate && 
                               !t.IsDeleted)
                    .ToListAsync();

                var dataPoints = new List<RevenueDataPoint>();

                switch (groupBy.ToLower())
                {
                    case "daily":
                        dataPoints = transactions
                            .GroupBy(t => t.PaymentDate.Date)
                            .Select(g => new RevenueDataPoint
                            {
                                Period = g.Key.ToString("yyyy-MM-dd"),
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(d => d.Period)
                            .ToList();
                        break;

                    case "weekly":
                        dataPoints = transactions
                            .GroupBy(t => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                t.PaymentDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                            .Select(g => new RevenueDataPoint
                            {
                                Period = $"Week {g.Key}",
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(d => d.Period)
                            .ToList();
                        break;

                    case "monthly":
                        dataPoints = transactions
                            .GroupBy(t => new { t.PaymentDate.Year, t.PaymentDate.Month })
                            .Select(g => new RevenueDataPoint
                            {
                                Period = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}",
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(d => d.Period)
                            .ToList();
                        break;

                    case "yearly":
                        dataPoints = transactions
                            .GroupBy(t => t.PaymentDate.Year)
                            .Select(g => new RevenueDataPoint
                            {
                                Period = g.Key.ToString(),
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(d => d.Period)
                            .ToList();
                        break;

                    default:
                        dataPoints = transactions
                            .GroupBy(t => t.PaymentDate.Date)
                            .Select(g => new RevenueDataPoint
                            {
                                Period = g.Key.ToString("yyyy-MM-dd"),
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(d => d.Period)
                            .ToList();
                        break;
                }

                var response = new RevenueAnalyticsResponse
                {
                    TotalRevenue = transactions.Sum(t => t.AmountPaid),
                    TotalTransactions = transactions.Count,
                    AverageTransactionAmount = transactions.Any() ? transactions.Average(t => t.AmountPaid) : 0,
                    DataPoints = dataPoints
                };

                return new BaseModel
                {
                    Success = true,
                    Data = response,
                    Message = "Revenue analytics retrieved successfully",
                    Total = dataPoints.Count,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting revenue analytics");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get revenue analytics: {ex.Message}",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetRevenueByFeeTypeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("📊 Getting revenue by fee type");

                var feeTypeRevenue = await _context.FeeTransaction
                    .Where(t => t.PaymentDate >= startDate && 
                               t.PaymentDate <= endDate && 
                               !t.IsDeleted)
                    .Include(t => t.FeeGroupFeeType)
                        .ThenInclude(fgft => fgft.FeeType)
                    .GroupBy(t => new { 
                        FeeTypeId = t.FeeGroupFeeType.FeeTypeId,
                        FeeTypeName = t.FeeGroupFeeType.FeeType.Name 
                    })
                    .Select(g => new FeeTypeRevenueData
                    {
                        FeeTypeId = g.Key.FeeTypeId,
                        FeeTypeName = g.Key.FeeTypeName,
                        Revenue = g.Sum(t => t.AmountPaid),
                        TransactionCount = g.Count()
                    })
                    .ToListAsync();

                var totalRevenue = feeTypeRevenue.Sum(f => f.Revenue);

                // Calculate percentages
                foreach (var item in feeTypeRevenue)
                {
                    item.Percentage = totalRevenue > 0 ? (item.Revenue / totalRevenue) * 100 : 0;
                }

                var response = new FeeTypeRevenueResponse
                {
                    FeeTypes = feeTypeRevenue.OrderByDescending(f => f.Revenue).ToList(),
                    TotalRevenue = totalRevenue
                };

                return new BaseModel
                {
                    Success = true,
                    Data = response,
                    Message = "Fee type revenue retrieved successfully",
                    Total = feeTypeRevenue.Count,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting fee type revenue");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get fee type revenue: {ex.Message}",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetRevenueByPaymentMethodAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("📊 Getting revenue by payment method");

                var methodData = await _context.FeeTransaction
                    .Where(t => t.PaymentDate >= startDate && 
                               t.PaymentDate <= endDate && 
                               !t.IsDeleted)
                    .GroupBy(t => t.PaymentMethod)
                    .Select(g => new PaymentMethodData
                    {
                        PaymentMethod = g.Key,
                        Amount = g.Sum(t => t.AmountPaid),
                        Count = g.Count()
                    })
                    .ToListAsync();

                var totalAmount = methodData.Sum(m => m.Amount);

                // Calculate percentages
                foreach (var method in methodData)
                {
                    method.Percentage = totalAmount > 0 ? (method.Amount / totalAmount) * 100 : 0;
                }

                var response = new PaymentMethodDistributionResponse
                {
                    Methods = methodData.OrderByDescending(m => m.Amount).ToList(),
                    TotalAmount = totalAmount,
                    TotalTransactions = methodData.Sum(m => m.Count)
                };

                return new BaseModel
                {
                    Success = true,
                    Data = response,
                    Message = "Payment method distribution retrieved successfully",
                    Total = methodData.Count,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting payment method distribution");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get payment method distribution: {ex.Message}",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetDefaultersListAsync(int minDaysOverdue = 1, decimal minAmount = 0)
        {
            try
            {
                _logger.LogInformation("📊 Getting defaulters list (minDays: {MinDays}, minAmount: {MinAmount})",
                    minDaysOverdue, minAmount);

                var today = DateTime.Now.Date;

                var overdueAssignments = await _context.FeeAssignment
                    .Where(fa => fa.DueDate < today && 
                                (fa.FinalAmount - fa.PaidAmount) > minAmount && 
                                !fa.IsDeleted)
                    .Include(fa => fa.Student)
                        .ThenInclude(s => s.StudentInfo)
                    .Include(fa => fa.Student)
                        .ThenInclude(s => s.StudentParent)
                    .Include(fa => fa.Student)
                        .ThenInclude(s => s.ClassAssignments)
                            .ThenInclude(ca => ca.Class)
                    .Include(fa => fa.Student)
                        .ThenInclude(s => s.ClassAssignments)
                            .ThenInclude(ca => ca.Section)
                    .Include(fa => fa.FeeGroupFeeType)
                        .ThenInclude(fgft => fgft.FeeType)
                    .ToListAsync();

                var defaulters = overdueAssignments
                    .Where(fa => (today - fa.DueDate).Days >= minDaysOverdue)
                    .GroupBy(fa => fa.StudentId)
                    .Select(g =>
                    {
                        var student = g.First().Student;
                        var studentInfo = student.StudentInfo;
                        var studentParent = student.StudentParent;
                        var classAssignment = student.ClassAssignments?.FirstOrDefault();

                        return new DefaulterInfo
                        {
                            StudentId = student.Id,
                            StudentName = studentInfo?.FirstName + ' ' + studentInfo?.LastName ?? "Unknown",
                            AdmissionNo = student.AdmissionNo ?? "N/A",
                            ClassName = classAssignment != null ? 
                                $"{classAssignment.Class?.Name} {classAssignment.Section?.Name}" : "N/A",
                            ContactNumber = studentInfo?.MobileNo ?? studentParent?.GuardianPhone ?? "N/A",
                            Email = studentInfo?.Email ?? studentParent?.GuardianEmail ?? "N/A",
                            OutstandingAmount = g.Sum(fa => fa.FinalAmount - fa.PaidAmount),
                            FineAmount = g.Sum(fa => fa.AmountFine),
                            DaysOverdue = g.Max(fa => (today - fa.DueDate).Days),
                            OldestDueDate = g.Min(fa => fa.DueDate),
                            OverdueFeeTypes = g.Select(fa => fa.FeeGroupFeeType?.FeeType?.Name ?? "Unknown")
                                .Distinct()
                                .ToList()
                        };
                    })
                    .OrderByDescending(d => d.OutstandingAmount)
                    .ToList();

                var response = new DefaultersListResponse
                {
                    Defaulters = defaulters,
                    TotalDefaulters = defaulters.Count,
                    TotalOutstanding = defaulters.Sum(d => d.OutstandingAmount),
                    TotalFines = defaulters.Sum(d => d.FineAmount)
                };

                return new BaseModel
                {
                    Success = true,
                    Data = response,
                    Message = "Defaulters list retrieved successfully",
                    Total = defaulters.Count,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting defaulters list");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get defaulters list: {ex.Message}",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetDefaultersSummaryAsync()
        {
            try
            {
                var today = DateTime.Now.Date;

                var summary = new
                {
                    TotalDefaulters = await _context.FeeAssignment
                        .Where(fa => fa.DueDate < today && (fa.FinalAmount - fa.PaidAmount) > 0 && !fa.IsDeleted)
                        .Select(fa => fa.StudentId)
                        .Distinct()
                        .CountAsync(),

                    TotalOutstanding = await _context.FeeAssignment
                        .Where(fa => fa.DueDate < today && !fa.IsDeleted)
                        .SumAsync(fa => fa.FinalAmount - fa.PaidAmount),

                    TotalFines = await _context.FeeAssignment
                        .Where(fa => fa.DueDate < today && !fa.IsDeleted)
                        .SumAsync(fa => fa.AmountFine)
                };

                return new BaseModel
                {
                    Success = true,
                    Data = summary,
                    Message = "Defaulters summary retrieved successfully",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting defaulters summary");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get defaulters summary: {ex.Message}",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetCollectionTrendsAsync(string period, DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("📊 Getting collection trends - Period: {Period}", period);

                var transactions = await _context.FeeTransaction
                    .Where(t => t.PaymentDate >= startDate && 
                               t.PaymentDate <= endDate && 
                               !t.IsDeleted)
                    .ToListAsync();

                var trends = new List<TrendDataPoint>();

                switch (period.ToLower())
                {
                    case "daily":
                        trends = transactions
                            .GroupBy(t => t.PaymentDate.Date)
                            .Select(g => new TrendDataPoint
                            {
                                PeriodLabel = g.Key.ToString("ddd, MMM dd"),
                                PeriodStart = g.Key,
                                PeriodEnd = g.Key.AddDays(1).AddSeconds(-1),
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(t => t.PeriodStart)
                            .ToList();
                        break;

                    case "weekly":
                        trends = transactions
                            .GroupBy(t => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                t.PaymentDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                            .Select(g => new TrendDataPoint
                            {
                                PeriodLabel = $"Week {g.Key}",
                                PeriodStart = g.Min(t => t.PaymentDate).Date,
                                PeriodEnd = g.Max(t => t.PaymentDate).Date,
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(t => t.PeriodStart)
                            .ToList();
                        break;

                    case "monthly":
                        trends = transactions
                            .GroupBy(t => new { t.PaymentDate.Year, t.PaymentDate.Month })
                            .Select(g => new TrendDataPoint
                            {
                                PeriodLabel = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}",
                                PeriodStart = new DateTime(g.Key.Year, g.Key.Month, 1),
                                PeriodEnd = new DateTime(g.Key.Year, g.Key.Month, DateTime.DaysInMonth(g.Key.Year, g.Key.Month)),
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(t => t.PeriodStart)
                            .ToList();
                        break;

                    default:
                        trends = transactions
                            .GroupBy(t => t.PaymentDate.Date)
                            .Select(g => new TrendDataPoint
                            {
                                PeriodLabel = g.Key.ToString("MMM dd"),
                                PeriodStart = g.Key,
                                PeriodEnd = g.Key.AddDays(1).AddSeconds(-1),
                                Amount = g.Sum(t => t.AmountPaid),
                                TransactionCount = g.Count()
                            })
                            .OrderBy(t => t.PeriodStart)
                            .ToList();
                        break;
                }

                var response = new CollectionTrendsResponse
                {
                    Period = period,
                    Trends = trends,
                    TotalAmount = trends.Sum(t => t.Amount),
                    AveragePerPeriod = trends.Any() ? trends.Average(t => t.Amount) : 0,
                    HighestCollection = trends.Any() ? trends.Max(t => t.Amount) : 0,
                    LowestCollection = trends.Any() ? trends.Min(t => t.Amount) : 0
                };

                return new BaseModel
                {
                    Success = true,
                    Data = response,
                    Message = "Collection trends retrieved successfully",
                    Total = trends.Count,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting collection trends");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get collection trends: {ex.Message}",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("📊 Getting dashboard summary");

                // Get all students count
                var totalStudents = await _context.Set<Domain.Entities.Student>()
                    .Where(s => !s.IsDeleted)
                    .CountAsync();

                // Get students with payments
                var payingStudents = await _context.FeeTransaction
                    .Where(t => t.PaymentDate >= startDate && t.PaymentDate <= endDate && !t.IsDeleted)
                    .Select(t => t.StudentId)
                    .Distinct()
                    .CountAsync();

                // Get defaulting students
                var today = DateTime.Now.Date;
                var defaultingStudents = await _context.FeeAssignment
                    .Where(fa => fa.DueDate < today && (fa.FinalAmount - fa.PaidAmount) > 0 && !fa.IsDeleted)
                    .Select(fa => fa.StudentId)
                    .Distinct()
                    .CountAsync();

                // Get revenue
                var totalRevenue = await _context.FeeTransaction
                    .Where(t => t.PaymentDate >= startDate && t.PaymentDate <= endDate && !t.IsDeleted)
                    .SumAsync(t => t.AmountPaid);

                // Get pending
                var totalPending = await _context.FeeAssignment
                    .Where(fa => !fa.IsDeleted)
                    .SumAsync(fa => fa.FinalAmount - fa.PaidAmount);

                // Get fines
                var totalFines = await _context.FeeAssignment
                    .Where(fa => !fa.IsDeleted)
                    .SumAsync(fa => fa.AmountFine);

                // Get discounts
                var totalDiscounts = await _context.FeeAssignment
                    .Where(fa => !fa.IsDeleted)
                    .SumAsync(fa => fa.AmountDiscount);

                // Calculate collection rate
                var totalAssigned = await _context.FeeAssignment
                    .Where(fa => !fa.IsDeleted)
                    .SumAsync(fa => fa.Amount);

                var collectionRate = totalAssigned > 0 ? ((totalAssigned - totalPending) / totalAssigned) * 100 : 0;

                // Get revenue analytics
                var revenueAnalytics = await GetRevenueAnalyticsAsync(startDate, endDate, "monthly");
                
                // Get payment method distribution
                var paymentMethodDist = await GetRevenueByPaymentMethodAsync(startDate, endDate);

                var summary = new DashboardSummaryResponse
                {
                    TotalRevenue = totalRevenue,
                    TotalPending = totalPending,
                    TotalFines = totalFines,
                    TotalDiscounts = totalDiscounts,
                    TotalStudents = totalStudents,
                    PayingStudents = payingStudents,
                    DefaultingStudents = defaultingStudents,
                    CollectionRate = collectionRate,
                    RevenueAnalytics = revenueAnalytics.Success ? (RevenueAnalyticsResponse)revenueAnalytics.Data : new RevenueAnalyticsResponse(),
                    PaymentMethodDistribution = paymentMethodDist.Success ? (PaymentMethodDistributionResponse)paymentMethodDist.Data : new PaymentMethodDistributionResponse()
                };

                return new BaseModel
                {
                    Success = true,
                    Data = summary,
                    Message = "Dashboard summary retrieved successfully",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting dashboard summary");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get dashboard summary: {ex.Message}",
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }
    }
}

