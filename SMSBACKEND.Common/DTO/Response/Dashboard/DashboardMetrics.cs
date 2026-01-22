namespace Common.DTO.Response.Dashboard
{
    /// <summary>
    /// Admin Dashboard Response - Complete dashboard data for administrators
    /// </summary>
    public class AdminDashboardResponse
    {
        public KeyMetrics Metrics { get; set; }
        public List<ModuleCard> Modules { get; set; }
        public FeeCollectionChart FeeChart { get; set; }
        public AttendanceChart AttendanceChart { get; set; }
        public List<RecentActivity> RecentActivities { get; set; }
        public List<PendingTask> PendingTasks { get; set; }
        public UserInfo User { get; set; }
    }

    /// <summary>
    /// Teacher Dashboard Response - Customized for teachers
    /// </summary>
    public class TeacherDashboardResponse
    {
        public TeacherMetrics Metrics { get; set; }
        public List<ModuleCard> Modules { get; set; }
        public List<TodaySchedule> TodaysSchedule { get; set; }
        public List<PendingAssignment> PendingAssignments { get; set; }
        public AttendanceChart TodaysAttendance { get; set; }
        public UserInfo User { get; set; }
    }

    /// <summary>
    /// Parent Dashboard Response - Customized for parents
    /// </summary>
    public class ParentDashboardResponse
    {
        public ParentMetrics Metrics { get; set; }
        public List<ChildSummary> Children { get; set; }
        public List<FeePaymentHistory> FeeHistory { get; set; }
        public List<UpcomingEvent> UpcomingEvents { get; set; }
        public UserInfo User { get; set; }
    }

    /// <summary>
    /// Key Metrics for Admin Dashboard
    /// </summary>
    public class KeyMetrics
    {
        public int TotalStudents { get; set; }
        public decimal StudentsTrend { get; set; } // % change from last month
        public string StudentsTrendDirection { get; set; } // "up", "down", "neutral"
        
        public int TotalTeachers { get; set; }
        public decimal TeachersTrend { get; set; }
        public string TeachersTrendDirection { get; set; }
        
        public decimal TotalRevenue { get; set; }
        public decimal RevenueTrend { get; set; }
        public string RevenueTrendDirection { get; set; }
        
        public int TotalClasses { get; set; }
        public decimal ClassesTrend { get; set; }
        public string ClassesTrendDirection { get; set; }
        
        public decimal TodaysAttendancePercentage { get; set; }
        public int PresentToday { get; set; }
        public int AbsentToday { get; set; }
        
        public decimal TodaysCollection { get; set; }
        public int PendingFeeCount { get; set; }
    }

    /// <summary>
    /// Teacher-specific Metrics
    /// </summary>
    public class TeacherMetrics
    {
        public int MyClasses { get; set; }
        public int MyStudents { get; set; }
        public int PendingAssignments { get; set; }
        public int TodaysLectures { get; set; }
        public decimal AverageAttendance { get; set; }
    }

    /// <summary>
    /// Parent-specific Metrics
    /// </summary>
    public class ParentMetrics
    {
        public int TotalChildren { get; set; }
        public decimal PendingFees { get; set; }
        public decimal AverageAttendance { get; set; }
        public int UpcomingExams { get; set; }
    }

    /// <summary>
    /// Module Card for Quick Access
    /// </summary>
    public class ModuleCard
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Route { get; set; }
        public int Count { get; set; }
        public int BadgeCount { get; set; } // Pending/notification count
        public string Color { get; set; } // Gradient color scheme
        public string Description { get; set; }
    }

    /// <summary>
    /// Fee Collection Chart Data
    /// </summary>
    public class FeeCollectionChart
    {
        public List<string> Labels { get; set; } // Dates or months
        public List<decimal> CollectedData { get; set; }
        public List<decimal> PendingData { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalPending { get; set; }
        public string ChartType { get; set; } // "daily", "weekly", "monthly"
    }

    /// <summary>
    /// Attendance Chart Data
    /// </summary>
    public class AttendanceChart
    {
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Leave { get; set; }
        public int Late { get; set; }
        public decimal Percentage { get; set; }
        public DateTime Date { get; set; }
        public string ChartType { get; set; } // "today", "week", "month"
    }

    /// <summary>
    /// Recent Activity Item
    /// </summary>
    public class RecentActivity
    {
        public string Type { get; set; } // "student_admission", "fee_payment", "attendance", etc.
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public DateTime Timestamp { get; set; }
        public string RelativeTime { get; set; } // "2 hours ago"
        public string Link { get; set; } // Navigation route
        public string EntityId { get; set; } // ID for navigation
    }

    /// <summary>
    /// Pending Task Item
    /// </summary>
    public class PendingTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public string Priority { get; set; } // "high", "medium", "low"
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public string Link { get; set; }
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// User Information for Dashboard Header
    /// </summary>
    public class UserInfo
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string Greeting { get; set; } // "Good morning", "Good afternoon", etc.
    }

    /// <summary>
    /// Today's Schedule Item (for Teachers)
    /// </summary>
    public class TodaySchedule
    {
        public string Time { get; set; }
        public string Subject { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string Room { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
    }

    /// <summary>
    /// Pending Assignment (for Teachers)
    /// </summary>
    public class PendingAssignment
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string ClassName { get; set; }
        public int SubmittedCount { get; set; }
        public int TotalStudents { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
    }

    /// <summary>
    /// Child Summary (for Parents)
    /// </summary>
    public class ChildSummary
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string RollNo { get; set; }
        public decimal AttendancePercentage { get; set; }
        public decimal PendingFees { get; set; }
        public int PendingAssignments { get; set; }
        public string RecentGrade { get; set; }
        public string Avatar { get; set; }
    }

    /// <summary>
    /// Fee Payment History (for Parents)
    /// </summary>
    public class FeePaymentHistory
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string FeeType { get; set; }
        public string ReceiptNo { get; set; }
        public string Status { get; set; }
        public string StudentName { get; set; }
    }

    /// <summary>
    /// Upcoming Event
    /// </summary>
    public class UpcomingEvent
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Type { get; set; } // "exam", "holiday", "meeting", "sports"
        public string Icon { get; set; }
        public string Color { get; set; }
    }

    /// <summary>
    /// Real-time Metric Update (for SignalR)
    /// </summary>
    public class MetricUpdate
    {
        public string MetricType { get; set; } // "students", "revenue", "attendance"
        public object Value { get; set; }
        public decimal Trend { get; set; }
        public string TrendDirection { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

