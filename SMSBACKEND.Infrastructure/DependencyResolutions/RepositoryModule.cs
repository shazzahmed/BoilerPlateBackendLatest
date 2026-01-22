using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Common.Options;
using Infrastructure.Repository;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Hangfire;

namespace Infrastructure.DependencyResolutions
{
    public static class RepositoryModule
    {
        public static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SqlServerDbContext>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    msSqlServerOptions => msSqlServerOptions.MigrationsAssembly("Infrastructure")
                    .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)
                    .CommandTimeout(30)) // 30 seconds command timeout
                .EnableSensitiveDataLogging() // Enable sensitive data logging
                .EnableDetailedErrors(), // Optional: Enable detailed errors for more debugging information
                ServiceLifetime.Scoped);
            services.AddHangfire(config => config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"))
            .UseRecommendedSerializerSettings());
            services.AddHangfireServer();
            services.BuildServiceProvider().GetService<UserManager<ApplicationUser>>();

            services.AddScoped<ISqlServerDbContext, SqlServerDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IUnitOfWork>(unitOfWork => new UnitOfWork(unitOfWork.GetService<ISqlServerDbContext>()));

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddTransient<INotificationTypeRepository, NotificationTypeRepository>();

            // automatic code generation area


            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IStudentRepository, StudentRepository>();
            services.AddTransient<IClassRepository, ClassRepository>();
            services.AddTransient<ISectionRepository, SectionRepository>();
            services.AddTransient<ISubjectRepository, SubjectRepository>();
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IStaffRepository, StaffRepository>();
            services.AddTransient<IStudentCategoryRepository, StudentCategoryRepository>();
            services.AddTransient<ISchoolHouseRepository, SchoolHouseRepository>();
            services.AddTransient<ITimetableRepository, TimetableRepository>();
            services.AddTransient<IDisableReasonRepository, DisableReasonRepository>();
            services.AddTransient<IFeeTypeRepository, FeeTypeRepository>();
            services.AddTransient<IFeeGroupRepository, FeeGroupRepository>();
            services.AddTransient<IFeeGroupFeeTypeRepository, FeeGroupFeeTypeRepository>();
            services.AddTransient<IFeeDiscountRepository, FeeDiscountRepository>();
            services.AddTransient<IFeeTransactionRepository, FeeTransactionRepository>();
            services.AddTransient<IFeeAssignmentRepository, FeeAssignmentRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IRolePermissionRepository, RolePermissionRepository>();
            services.AddTransient<IExpenseHeadRepository, ExpenseHeadRepository>();
            services.AddTransient<IExpenseRepository, ExpenseRepository>();
            services.AddTransient<IIncomeHeadRepository, IncomeHeadRepository>();
            services.AddTransient<IIncomeRepository, IncomeRepository>();
            services.AddTransient<IEnquiryRepository, EnquiryRepository>();
            services.AddTransient<IPreAdmissionRepository, PreAdmissionRepository>();
            services.AddTransient<IEntryTestRepository, EntryTestRepository>();
            services.AddTransient<IAdmissionRepository, AdmissionRepository>();
            services.AddTransient<IStudentAttendanceRepository, StudentAttendanceRepository>();
            services.AddTransient<IFineWaiverRepository, FineWaiverRepository>();
            
            // Exam Module Repositories
            services.AddTransient<IExamRepository, ExamRepository>();
            services.AddTransient<IExamSubjectRepository, ExamSubjectRepository>();
            services.AddTransient<IExamMarksRepository, ExamMarksRepository>();
            services.AddTransient<IGradeScaleRepository, GradeScaleRepository>();
            services.AddTransient<IGradeRepository, GradeRepository>();
            services.AddTransient<IExamResultRepository, ExamResultRepository>();
            services.AddTransient<IExamTimetableRepository, ExamTimetableRepository>();
            services.AddTransient<IMarksheetTemplateRepository, MarksheetTemplateRepository>();
            
            services.AddTransient<ISessionRepository, SessionRepository>();
            // automatic code generation area end

        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            DbMigrator.Migrate(app);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            DataSeeder.SeedingData(app);
            DataSeeder.Seed(app);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
