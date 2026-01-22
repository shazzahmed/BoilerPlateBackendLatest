using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Infrastructure.Services;
using Infrastructure.Services.Services;
using SMSBACKEND.Infrastructure.Services.Services;

namespace Infrastructure.ServicesDependencyResolutions
{
    public static class ServiceModule
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Multi-Tenancy Services
            services.AddScoped<ITenantService, TenantService>();

            // Dashboard Service
            services.AddScoped<IDashboardService, DashboardService>();
            
            // SignalR for Real-time Updates
            services.AddSignalR(options =>
            {
                // ✅ SIGNALR: Enable detailed errors for debugging (disable in production)
                options.EnableDetailedErrors = true;
                
                // ✅ SIGNALR: Set keepalive and timeout intervals
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            });

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<INotificationTemplateService, NotificationTemplateService>();
            services.AddTransient<INotificationTypeService, NotificationTypeService>();
            services.AddSingleton<ICacheProvider, DistributedHybridCacheProvider>();

            // automatic code generation area


            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<IClassService, ClassService>();
            services.AddTransient<ISectionService, SectionService>();
            services.AddTransient<ISubjectService, SubjectService>();
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IStaffService, StaffService>();
            services.AddTransient<IStudentCategoryService, StudentCategoryService>();
            services.AddTransient<ISchoolHouseService, SchoolHouseService>();
            services.AddTransient<ITeacherService, TeacherService>();
            services.AddTransient<ITimetableService, TimetableService>();
            services.AddTransient<IDisableReasonService, DisableReasonService>();
            services.AddTransient<IFeeTypeService, FeeTypeService>();
            services.AddTransient<IFeeGroupService, FeeGroupService>();
            services.AddTransient<IFeeGroupFeeTypeService, FeeGroupFeeTypeService>();
            services.AddTransient<IFeeDiscountService, FeeDiscountService>();
            services.AddTransient<IFeeTransactionService, FeeTransactionService>();
            services.AddTransient<IFeeAssignmentService, FeeAssignmentService>();
            services.AddScoped<IFeePaymentService, FeePaymentService>();
            services.AddScoped<IPaymentAnalyticsService, PaymentAnalyticsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IRolePermissionService, RolePermissionService>();
            services.AddTransient<IExpenseHeadService, ExpenseHeadService>();
            services.AddTransient<IExpenseService, ExpenseService>();
            services.AddTransient<IIncomeHeadService, IncomeHeadService>();
            services.AddTransient<IIncomeService, IncomeService>();
            services.AddTransient<IEnquiryService, EnquiryService>();
            services.AddTransient<IPreAdmissionService, PreAdmissionService>();
            services.AddTransient<IEntryTestService, EntryTestService>();
            services.AddTransient<IAdmissionService, AdmissionService>();
            services.AddTransient<IExcelParserService, ExcelParserService>();
            services.AddTransient<IStudentAttendanceService, StudentAttendanceService>();
            services.AddTransient<IFineWaiverService, FineWaiverService>();
            
            // Exam Module Services
            services.AddTransient<IExamService, ExamService>();
            services.AddTransient<IExamSubjectService, ExamSubjectService>();
            services.AddTransient<IExamMarksService, ExamMarksService>();
            services.AddTransient<IGradeScaleService, GradeScaleService>();
            services.AddTransient<IGradeService, GradeService>();
            services.AddTransient<IExamResultService, ExamResultService>();
            services.AddTransient<IExamTimetableService, ExamTimetableService>();
            services.AddTransient<IMarksheetTemplateService, MarksheetTemplateService>();
            
            services.AddTransient<ISessionService, SessionService>();
            // automatic code generation area end
        }
    }
}
