using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Database
{
    public interface ISqlServerDbContext : IDisposable
    {
        DatabaseFacade Database { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        //int SaveChanges(CancellationToken cancellationToken = default(CancellationToken));


        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
       
        DbSet<ApplicationUser> User { get; set; }
        DbSet<Status> Statuses { get; set; }
        DbSet<StatusType> StatusTypes { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<Notification> Notifications { get; set; }
        DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        DbSet<NotificationType> NotificationTypes { get; set; }
        // automatic code generation area

        DbSet<Student> Students { get; set; }
        DbSet<StudentInfo> StudentInfo { get; set; }
        DbSet<StudentAddress> StudentAddress { get; set; }
        DbSet<StudentFinancial> StudentFinancial { get; set; }
        DbSet<StudentParent> StudentParent { get; set; }
        DbSet<StudentMiscellaneous> StudentMiscellaneous { get; set; }
        DbSet<Class> Classes { get; set; }
        DbSet<ClassSection> ClassSections { get; set; }
        DbSet<Section> Sections { get; set; }
        DbSet<StudentClassAssignment> StudentClasses { get; set; }
        DbSet<Module> Modules { get; set; }
        DbSet<Session> Session { get; set; }
        DbSet<Subject> Subject { get; set; }
        DbSet<SubjectClass> SubjectClass { get; set; }
        DbSet<Department> Department { get; set; }
        DbSet<Staff> Staff { get; set; }
        DbSet<StudentCategory> StudentCategory { get; set; }
        DbSet<ClassTeacher> ClassTeacher { get; set; }
        DbSet<SchoolHouse> SchoolHouse { get; set; }
        DbSet<TeacherSubjects> TeacherSubjects { get; set; }
        DbSet<Timetable> Timetable { get; set; }
        DbSet<StudentAttendance> StudentAttendance { get; set; }
        DbSet<DisableReason> DisableReason { get; set; }
        DbSet<FeeType> FeeType { get; set; }
        DbSet<FeeGroup> FeeGroup { get; set; }
        DbSet<FeeGroupFeeType> FeeGroupFeeType { get; set; }
        DbSet<FeeDiscount> FeeDiscount { get; set; }
        DbSet<FeeTransaction> FeeTransaction { get; set; }
        DbSet<FeeAssignment> FeeAssignment { get; set; }
        DbSet<Permission> Permission { get; set; }
        DbSet<RolePermission> RolePermission { get; set; }
        DbSet<ExpenseHead> ExpenseHead { get; set; }
        DbSet<Expense> Expense { get; set; }
        DbSet<IncomeHead> IncomeHead { get; set; }
        DbSet<Income> Income { get; set; }
        DbSet<Enquiry> Enquiry { get; set; }
        DbSet<PreAdmission> PreAdmission { get; set; }
        DbSet<EntryTest> EntryTest { get; set; }
        DbSet<Admission> Admission { get; set; }
        DbSet<FineWaiver> FineWaiver { get; set; }
        DbSet<Exam> Exams { get; set; }
        DbSet<ExamMarks> ExamMarks { get; set; }
        DbSet<ExamResult> ExamResults { get; set; }
        DbSet<ExamSubject> ExamSubjects { get; set; }
        DbSet<ExamTimetable> ExamTimetables { get; set; }
        DbSet<GradeScale> GradeScales { get; set; }
        DbSet<Grade> Grades { get; set; }
        DbSet<MarksheetTemplate> MarksheetTemplates { get; set; }
        DbSet<Tenant> Tenants { get; set; }

        // automatic code generation area end
    }
}

