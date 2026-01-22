using Application.ServiceContracts.IServices;
using Common.DTO.Response.Dashboard;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static Common.Utilities.Enums;

namespace Infrastructure.Services.Services
{
    /// <summary>
    /// Dashboard Service Implementation
    /// Provides comprehensive dashboard data for different user roles
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly SqlServerDbContext _context;

        public DashboardService(SqlServerDbContext context)
        {
            _context = context;
        }

        #region Admin Dashboard

        public async Task<AdminDashboardResponse> GetAdminDashboardAsync(string userId, int tenantId, string dateRange = "month")
        {
            var response = new AdminDashboardResponse
            {
                User = await GetUserInfoAsync(userId),
                Metrics = await GetKeyMetricsAsync(tenantId),
                Modules = await GetModuleCardsAsync(tenantId, "Admin"),
                FeeChart = await GetFeeCollectionChartAsync(tenantId, dateRange),
                AttendanceChart = await GetAttendanceChartAsync(tenantId, "today"),
                RecentActivities = await GetRecentActivitiesAsync(tenantId, 10),
                PendingTasks = await GetPendingTasksAsync(tenantId)
            };

            return response;
        }

        private async Task<KeyMetrics> GetKeyMetricsAsync(int tenantId)
        {
            // ✅ Convert UTC to Pakistan timezone (since server is in Pacific Time)
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            var utcNow = DateTime.UtcNow;
            var now = TimeZoneInfo.ConvertTimeFromUtc(utcNow, pakistanTimeZone);
            
            // Log timezone info for debugging
            var logMessage = $"[{now:yyyy-MM-dd HH:mm:ss}] 🕐 Dashboard: Pakistan Time: {now}, UTC Time: {utcNow}, Server Timezone: {TimeZoneInfo.Local.DisplayName}, TenantId: {tenantId}";
            
            try
            {
                // Write to a log file in the root directory (accessible via FTP)
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dashboard_debug.log");
                File.AppendAllText(logPath, logMessage + Environment.NewLine);
            }
            catch 
            {
                // If file write fails, fallback to console
                Console.WriteLine(logMessage);
            }
            
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastMonth = firstDayOfMonth.AddMonths(-1);
            var lastMonthEnd = firstDayOfMonth.AddDays(-1);

            // Get current month data
            var totalStudents = await _context.Students
                .Where(s => !s.IsDeleted && s.TenantId == tenantId)
                .CountAsync();

            var totalTeachers = await _context.Staff
                .Where(s => !s.IsDeleted && s.TenantId == tenantId)
                .CountAsync();

            var totalClasses = await _context.Classes
                .Where(c => !c.IsDeleted && c.TenantId == tenantId)
                .CountAsync();

            var totalRevenue = await _context.Set<Domain.Entities.FeeTransaction>()
                .Where(f => f.TenantId == tenantId && 
                           f.PaymentDate >= firstDayOfMonth &&
                           f.PaymentDate <= now)
                .SumAsync(f => f.AmountPaid);

            // Get last month data for trends
            var lastMonthStudents = await _context.Students
                .Where(s => !s.IsDeleted && s.TenantId == tenantId && 
                           s.CreatedAt < firstDayOfMonth)
                .CountAsync();

            var lastMonthTeachers = await _context.Staff
                .Where(s => !s.IsDeleted && s.TenantId == tenantId && 
                           s.CreatedAt < firstDayOfMonth)
                .CountAsync();

            var lastMonthRevenue = await _context.Set<Domain.Entities.FeeTransaction>()
                .Where(f => f.TenantId == tenantId && 
                           f.PaymentDate >= lastMonth &&
                           f.PaymentDate <= lastMonthEnd)
                .SumAsync(f => f.AmountPaid);

            // Calculate trends
            var studentsTrend = CalculateTrend(totalStudents, lastMonthStudents);
            var teachersTrend = CalculateTrend(totalTeachers, lastMonthTeachers);
            var revenueTrend = CalculateTrend((double)totalRevenue, (double)lastMonthRevenue);

            // Today's attendance
            // ✅ Use Now instead of Today to ensure we get current date with time, then compare dates only
            var todayDate = now.Date;
            var attendanceToday = await _context.Set<StudentAttendance>()
                .Where(a => a.TenantId == tenantId && 
                           a.AttendanceDate.Date == todayDate)
                .ToListAsync();

            var presentToday = attendanceToday.Count(a => a.AttendanceType == AttendanceType.Present);
            var absentToday = attendanceToday.Count(a => a.AttendanceType == AttendanceType.Absent);
            var totalMarked = attendanceToday.Count();
            var attendancePercentage = totalMarked > 0 ? (decimal)presentToday / totalMarked * 100 : 0;

            // Today's collection
            var todaysCollection = await _context.Set<Domain.Entities.FeeTransaction>()
                .Where(f => f.TenantId == tenantId && 
                           f.PaymentDate.Date == todayDate)
                .SumAsync(f => f.AmountPaid);

            // Pending fees count
            var pendingFeeCount = await _context.Set<Domain.Entities.FeeAssignment>()
                .Where(f => f.TenantId == tenantId && 
                           !f.IsDeleted &&
                           f.FeeTransactions.Sum(t => t.AmountPaid) < f.Amount)
                .CountAsync();

            return new KeyMetrics
            {
                TotalStudents = totalStudents,
                StudentsTrend = studentsTrend,
                StudentsTrendDirection = GetTrendDirection(studentsTrend),

                TotalTeachers = totalTeachers,
                TeachersTrend = teachersTrend,
                TeachersTrendDirection = GetTrendDirection(teachersTrend),

                TotalRevenue = totalRevenue,
                RevenueTrend = revenueTrend,
                RevenueTrendDirection = GetTrendDirection(revenueTrend),

                TotalClasses = totalClasses,
                ClassesTrend = 0, // Classes don't change frequently
                ClassesTrendDirection = "neutral",

                TodaysAttendancePercentage = Math.Round(attendancePercentage, 2),
                PresentToday = presentToday,
                AbsentToday = absentToday,

                TodaysCollection = todaysCollection,
                PendingFeeCount = pendingFeeCount
            };
        }

        private async Task<List<ModuleCard>> GetModuleCardsAsync(int tenantId, string role)
        {
            var modules = await _context.Modules
                .Where(m => m.IsDashboard)
                .ToListAsync();

            var moduleCards = new List<ModuleCard>();

            foreach (var module in modules)
            {
                var card = new ModuleCard
                {
                    Name = module.Name,
                    Icon = module.Icon ?? "school",
                    Route = module.Path ?? "#",
                    Description = module.Description ?? "",
                    Color = GetModuleColor(module.Name),
                    Count = await GetModuleCountAsync(module.Name, tenantId),
                    BadgeCount = await GetModuleBadgeCountAsync(module.Name, tenantId)
                };

                moduleCards.Add(card);
            }

            return moduleCards;
        }

        private async Task<int> GetModuleCountAsync(string moduleName, int tenantId)
        {
            return moduleName.ToLower() switch
            {
                "students" => await _context.Students.Where(s => !s.IsDeleted && s.TenantId == tenantId).CountAsync(),
                "teachers" or "staff" => await _context.Staff.Where(s => !s.IsDeleted && s.TenantId == tenantId).CountAsync(),
                "classes" => await _context.Classes.Where(c => !c.IsDeleted && c.TenantId == tenantId).CountAsync(),
                "fee collection" or "fees" => await _context.Set<Domain.Entities.FeeTransaction>()
                    .Where(f => f.TenantId == tenantId && f.PaymentDate.Month == DateTime.Now.Month)
                    .CountAsync(),
                _ => 0
            };
        }

        private async Task<int> GetModuleBadgeCountAsync(string moduleName, int tenantId)
        {
            return moduleName.ToLower() switch
            {
                "students" => await _context.Set<Domain.Entities.PreAdmission>()
                    .Where(p => p.TenantId == tenantId && p.Status == "Submitted")
                    .CountAsync(),
                "fee collection" or "fees" => await _context.Set<Domain.Entities.FeeAssignment>()
                    .Where(f => f.TenantId == tenantId && 
                               !f.IsDeleted &&
                               f.FeeTransactions.Sum(t => t.AmountPaid) < f.Amount)
                    .CountAsync(),
                _ => 0
            };
        }

        private string GetModuleColor(string moduleName)
        {
            return moduleName.ToLower() switch
            {
                "students" => "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
                "teachers" or "staff" => "linear-gradient(135deg, #f093fb 0%, #f5576c 100%)",
                "fee collection" or "fees" => "linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)",
                "attendance" => "linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)",
                "classes" => "linear-gradient(135deg, #fa709a 0%, #fee140 100%)",
                "timetable" => "linear-gradient(135deg, #30cfd0 0%, #330867 100%)",
                "exams" => "linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)",
                "library" => "linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%)",
                _ => "linear-gradient(135deg, #667eea 0%, #764ba2 100%)"
            };
        }

        private async Task<FeeCollectionChart> GetFeeCollectionChartAsync(int tenantId, string dateRange)
        {
            // ✅ Use Pakistan timezone
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);
            
            var startDate = dateRange.ToLower() switch
            {
                "week" => now.AddDays(-7),
                "month" => now.AddMonths(-1),
                "year" => now.AddYears(-1),
                _ => now.AddMonths(-1) // default to month
            };

            var transactions = await _context.Set<Domain.Entities.FeeTransaction>()
                .Where(f => f.TenantId == tenantId && 
                           f.PaymentDate >= startDate &&
                           f.PaymentDate <= now)
                .GroupBy(f => f.PaymentDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Collected = g.Sum(f => f.AmountPaid)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var labels = new List<string>();
            var collectedData = new List<decimal>();
            var pendingData = new List<decimal>();
            
            try
            {
                // Load fee assignments with transactions (to avoid nested aggregates)
                var feeAssignments = await _context.Set<Domain.Entities.FeeAssignment>()
                   .Include(f => f.FeeTransactions)
                   .Where(f => f.TenantId == tenantId &&
                              f.DueDate >= startDate &&
                              f.DueDate <= now)
                   .ToListAsync();

                // Group and aggregate in memory
                var feeAssignmentsByDate = feeAssignments
                    .GroupBy(f => f.DueDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Total = g.Sum(f => f.Amount),
                        Paid = g.Sum(f => f.FeeTransactions
                            .Where(t => t.PaymentDate <= now)
                            .Sum(t => t.AmountPaid))
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                foreach (var item in feeAssignmentsByDate)
                {
                    labels.Add(item.Date.ToString("MMM dd"));
                    var collected = transactions.FirstOrDefault(t => t.Date == item.Date)?.Collected ?? 0;
                    collectedData.Add(collected);
                    pendingData.Add(item.Total - item.Paid);
                }
            }
            catch (Exception ex)
            {
                // Return empty chart on error
                labels = new List<string> { "No Data" };
                collectedData = new List<decimal> { 0 };
                pendingData = new List<decimal> { 0 };
            }
            return new FeeCollectionChart
            {
                Labels = labels,
                CollectedData = collectedData,
                PendingData = pendingData,
                TotalCollected = collectedData.Sum(),
                TotalPending = pendingData.Sum(),
                ChartType = dateRange.ToLower()
            };
        }

        private async Task<AttendanceChart> GetAttendanceChartAsync(int tenantId, string chartType)
        {
            // ✅ Use Pakistan timezone
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);
            var targetDate = now.Date;

            var attendance = await _context.Set<StudentAttendance>()
                .Where(a => a.TenantId == tenantId && 
                           a.AttendanceDate.Date == targetDate)
                .ToListAsync();

            var present = attendance.Count(a => a.AttendanceType == AttendanceType.Present);
            var absent = attendance.Count(a => a.AttendanceType == AttendanceType.Absent);
            var leave = attendance.Count(a => a.AttendanceType == AttendanceType.Excused);
            var late = attendance.Count(a => a.AttendanceType == AttendanceType.Late);

            var total = present + absent + leave + late;
            var percentage = total > 0 ? (decimal)present / total * 100 : 0;

            return new AttendanceChart
            {
                Present = present,
                Absent = absent,
                Leave = leave,
                Late = late,
                Percentage = Math.Round(percentage, 2),
                Date = targetDate,
                ChartType = chartType
            };
        }

        private async Task<List<RecentActivity>> GetRecentActivitiesAsync(int tenantId, int limit)
        {
            try
            {
                var activities = new List<RecentActivity>();

                // Recent student admissions
                var recentStudents = await _context.Students
                    .Include(x => x.ClassAssignments).ThenInclude(c => c.Class)
                    .Where(s => s.TenantId == tenantId && !s.IsDeleted)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(3)
                    .Select(s => new RecentActivity
                    {
                        Type = "student_admission",
                        Title = "New Student Admitted",
                        Description = $"{s.FirstName} {s.LastName} admitted to {s.ClassAssignments.FirstOrDefault().Class.Name}",
                        Icon = "person_add",
                        IconColor = "#4caf50",
                        Timestamp = s.CreatedAt,
                        RelativeTime = GetRelativeTime(s.CreatedAt),
                        Link = "/students",
                        EntityId = s.Id.ToString()
                    })
                    .ToListAsync();

                activities.AddRange(recentStudents);

                // Recent fee payments
                var recentPayments = await _context.Set<Domain.Entities.FeeTransaction>()
                    .Where(f => f.TenantId == tenantId)
                    .OrderByDescending(f => f.PaymentDate)
                    .Take(3)
                    .Select(f => new RecentActivity
                    {
                        Type = "fee_payment",
                        Title = "Fee Payment Received",
                        Description = $"₨{f.AmountPaid} received from {f.FeeAssignment.Student.FirstName} {f.FeeAssignment.Student.LastName}",
                        Icon = "payments",
                        IconColor = "#2196f3",
                        Timestamp = f.PaymentDate,
                        RelativeTime = GetRelativeTime(f.PaymentDate),
                        Link = "/fees",
                        EntityId = f.Id.ToString()
                    })
                    .ToListAsync();

                activities.AddRange(recentPayments);

                // Sort by timestamp and return top N
                return activities
                    .OrderByDescending(a => a.Timestamp)
                    .Take(limit)
                    .ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task<List<PendingTask>> GetPendingTasksAsync(int tenantId)
        {
            var tasks = new List<PendingTask>();

            // Pending leave requests (if you have this module)
            // tasks.Add(new PendingTask { ... });

            // Pending fee payments
            var pendingFees = await _context.Set<Domain.Entities.FeeAssignment>()
                .Where(f => f.TenantId == tenantId && 
                           !f.IsDeleted &&
                           f.FeeTransactions.Sum(t => t.AmountPaid) < f.Amount)
                .CountAsync();

            if (pendingFees > 0)
            {
                tasks.Add(new PendingTask
                {
                    Title = "Pending Fee Payments",
                    Description = $"{pendingFees} students have pending fee payments",
                    Count = pendingFees,
                    Priority = "high",
                    Icon = "payments",
                    IconColor = "#f44336",
                    Link = "/fees/pending"
                });
            }

            // Pending admissions
            var pendingAdmissions = await _context.Set<Domain.Entities.PreAdmission>()
                .Where(p => p.TenantId == tenantId && p.Status == "Submitted")
                .CountAsync();

            if (pendingAdmissions > 0)
            {
                tasks.Add(new PendingTask
                {
                    Title = "Pending Admissions",
                    Description = $"{pendingAdmissions} applications awaiting review",
                    Count = pendingAdmissions,
                    Priority = "medium",
                    Icon = "pending_actions",
                    IconColor = "#ff9800",
                    Link = "/admissions/pending"
                });
            }

            return tasks;
        }

        #endregion

        #region Teacher Dashboard

        public async Task<TeacherDashboardResponse> GetTeacherDashboardAsync(int teacherId, int tenantId)
        {
            // TODO: Implement teacher dashboard logic
            throw new NotImplementedException("Teacher dashboard will be implemented in next phase");
        }

        #endregion

        #region Parent Dashboard

        public async Task<ParentDashboardResponse> GetParentDashboardAsync(int parentId, int tenantId)
        {
            // TODO: Implement parent dashboard logic
            throw new NotImplementedException("Parent dashboard will be implemented in next phase");
        }

        #endregion

        #region Helper Methods

        private async Task<UserInfo> GetUserInfoAsync(string userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
            
            return new UserInfo
            {
                FullName = user?.FullName ?? "User",
                Role = user?.UserRoles.FirstOrDefault()?.Role?.Name ?? "Admin",
                Avatar = user?.Picture ?? "/assets/images/default-avatar.png",
                Greeting = GetGreeting()
            };
        }

        private static string GetGreeting()
        {
            // ✅ Use Pakistan timezone for greeting
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);
            var hour = now.Hour;
            return hour < 12 ? "Good morning" : hour < 18 ? "Good afternoon" : "Good evening";
        }

        private decimal CalculateTrend(double current, double previous)
        {
            if (previous == 0) return current > 0 ? 100 : 0;
            return (decimal)((current - previous) / previous * 100);
        }

        private string GetTrendDirection(decimal trend)
        {
            if (trend > 0) return "up";
            if (trend < 0) return "down";
            return "neutral";
        }

        private static string GetRelativeTime(DateTime dateTime)
        {
            // ✅ Use Pakistan timezone for relative time calculation
            var pakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, pakistanTimeZone);
            var timeSpan = now - dateTime;

            if (timeSpan.TotalMinutes < 1) return "just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} months ago";
            return $"{(int)(timeSpan.TotalDays / 365)} years ago";
        }

        public Task<object> GetChartDataAsync(string chartType, string dateRange, int tenantId)
        {
            // TODO: Implement dynamic chart data retrieval
            throw new NotImplementedException();
        }

        public Task<MetricUpdate> GetMetricUpdateAsync(string metricType, int tenantId)
        {
            // TODO: Implement for SignalR real-time updates
            throw new NotImplementedException();
        }

        #endregion
    }
}

