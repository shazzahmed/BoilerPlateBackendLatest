using IdentityModel;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Infrastructure.Database
{
    public class SqlServerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
    ApplicationUserRole, IdentityUserLogin<string>,
    IdentityRoleClaim<string>, IdentityUserToken<string>>, ISqlServerDbContext
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            this.configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public SqlServerDbContext() : base()
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(SqlServerDbContext).Assembly);
            base.OnModelCreating(builder);
            SeedData.Seed(builder);

            // ===== GLOBAL TENANT CASCADE PREVENTION =====
            // Configure ALL Tenant relationships to use Restrict to prevent multiple cascade paths
            // This is essential because many entities inherit from BaseEntity which has TenantId FK
            // and these entities often reference each other, creating cascade cycles

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tenantForeignKey = entityType.GetForeignKeys()
                    .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Tenant));
                
                if (tenantForeignKey != null)
                {
                    tenantForeignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
            
            // Configure Fee Module relationships to prevent cascade delete conflicts
            builder.Entity<FeeTransaction>()
                .HasOne(ft => ft.FeeAssignment)
                .WithMany(fa => fa.FeeTransactions)
                .HasForeignKey(ft => ft.FeeAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Student relationship as optional (for provisional fees)
            builder.Entity<FeeTransaction>()
                .HasOne(ft => ft.Student)
                .WithMany()
                .HasForeignKey(ft => ft.StudentId)
                .IsRequired(false) // ✅ Make StudentId optional
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeGroupFeeType>()
                .HasOne(fgft => fgft.FeeGroup)
                .WithMany(fg => fg.FeeGroupFeeTypes)
                .HasForeignKey(fgft => fgft.FeeGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeGroupFeeType>()
                .HasOne(fgft => fgft.FeeType)
                .WithMany(ft => ft.FeeGroupFeeTypes)
                .HasForeignKey(fgft => fgft.FeeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeGroupFeeType>()
                .HasOne(fgft => fgft.FeeDiscount)
                .WithMany(fd => fd.FeeGroupFeeTypes)
                .HasForeignKey(fgft => fgft.FeeDiscountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeGroupFeeType>()
                .HasOne(fgft => fgft.Tenant)
                .WithMany()
                .HasForeignKey(fgft => fgft.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeGroup>()
                .HasOne(fg => fg.Tenant)
                .WithMany()
                .HasForeignKey(fg => fg.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeType>()
                .HasOne(ft => ft.Tenant)
                .WithMany()
                .HasForeignKey(ft => ft.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeDiscount>()
                .HasOne(fd => fd.Tenant)
                .WithMany()
                .HasForeignKey(fd => fd.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Configure Expense relationships to prevent cascade delete conflicts
            builder.Entity<Expense>()
                .HasOne(e => e.ExpenseHead)
                .WithMany(eh => eh.Expenses)
                .HasForeignKey(e => e.ExpenseHeadId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Expense>()
                .HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExpenseHead>()
                .HasOne(eh => eh.Tenant)
                .WithMany()
                .HasForeignKey(eh => eh.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Income relationships to prevent cascade delete conflicts
            builder.Entity<Income>()
                .HasOne(i => i.IncomeHead)
                .WithMany(ih => ih.Incomes)
                .HasForeignKey(i => i.IncomeHeadId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Income>()
                .HasOne(i => i.Tenant)
                .WithMany()
                .HasForeignKey(i => i.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<IncomeHead>()
                .HasOne(ih => ih.Tenant)
                .WithMany()
                .HasForeignKey(ih => ih.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
            // Student-Parent relationship is now handled through StudentInfo
            builder.Entity<StudentInfo>()
            .HasOne(si => si.Parent)
            .WithMany() // A parent can have multiple students
            .HasForeignKey(si => si.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // Optional: Prevent cascading delete

            builder.Entity<Module>()
            .HasOne(m => m.Parent) // Each menu has one parent
            .WithMany(m => m.Children) // Each menu can have many children
            .HasForeignKey(m => m.ParentId) // Foreign key is ParentId
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes to avoid issues

            // Composite Primary Key for StudentClassAssignment
            builder.Entity<StudentClassAssignment>()
                .HasKey(sca => new { sca.StudentId, sca.ClassId, sca.SectionId, sca.SessionId });

            // Define relationships
            builder.Entity<StudentClassAssignment>()
                .HasOne(sca => sca.Student)
                .WithMany(s => s.ClassAssignments)
                .HasForeignKey(sca => sca.StudentId);

            // Configure RefreshToken entity for multi-device support
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.RefreshTokenId); // New primary key
                entity.Property(rt => rt.RefreshTokenId).ValueGeneratedOnAdd(); // Auto-increment
                
                // Create composite index for efficient queries
                entity.HasIndex(rt => new { rt.Id, rt.DeviceId }).IsUnique(); // One token per user per device
                entity.HasIndex(rt => rt.Username); // For username-based queries
                entity.HasIndex(rt => rt.Token); // For token-based queries
            });

            builder.Entity<StudentClassAssignment>()
                .HasOne(sca => sca.Class)
                .WithMany(c => c.StudentAssignments)
                .HasForeignKey(sca => sca.ClassId);

            builder.Entity<StudentClassAssignment>()
                .HasOne(sca => sca.Section)
                .WithMany(sec => sec.StudentAssignments)
                .HasForeignKey(sca => sca.SectionId);

            builder.Entity<StudentClassAssignment>()
                .HasOne(sca => sca.Session)
                .WithMany(sess => sess.StudentAssignments)
                .HasForeignKey(sca => sca.SessionId);

            builder.Entity<Class>()
                .HasMany(c => c.ClassSections)
                .WithOne(cs => cs.Class)
                .HasForeignKey(cs => cs.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Section>()
                .HasMany(s => s.ClassSections)
                .WithOne(cs => cs.Section)
                .HasForeignKey(cs => cs.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClassSection>()
                .HasOne(cs => cs.Tenant)
                .WithMany()
                .HasForeignKey(cs => cs.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Class>()
                .HasOne(c => c.Tenant)
                .WithMany()
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Section>()
                .HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PreAdmission foreign key constraints to prevent cascade delete conflicts
            builder.Entity<Enquiry>()
                 .HasOne(e => e.PreAdmission)
                 .WithOne(pa => pa.Enquiry)
                 .HasForeignKey<PreAdmission>(pa => pa.EnquiryId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enquiry>()
                .HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ApplyingClass)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PreAdmission>()
                .HasOne(pa => pa.Class)
                .WithMany()
                .HasForeignKey(pa => pa.ApplyingClass)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure EntryTest -> PreAdmission (one-to-one for workflow tracking)
            builder.Entity<EntryTest>()
                .HasOne(e => e.Application)
                .WithOne(pa => pa.EntryTest)
                .HasForeignKey<EntryTest>(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Admission -> EntryTest (one-to-one for workflow tracking)
            builder.Entity<Admission>()
                .HasOne(a => a.EntryTest)
                .WithOne(e => e.Admission)
                .HasForeignKey<Admission>(a => a.EntryTestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EntryTest>()
                .HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ApplyingClass)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Student normalized entities relationships
            // Keep cascade for Student -> Child entities, but restrict Tenant relationships
            builder.Entity<Student>()
                .HasOne(s => s.StudentInfo)
                .WithOne(si => si.Student)
                .HasForeignKey<StudentInfo>(si => si.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Student>()
                .HasOne(s => s.StudentAddress)
                .WithOne(sa => sa.Student)
                .HasForeignKey<StudentAddress>(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Student>()
                .HasOne(s => s.StudentFinancial)
                .WithOne(sf => sf.Student)
                .HasForeignKey<StudentFinancial>(sf => sf.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Student>()
                .HasOne(s => s.StudentParent)
                .WithOne(sp => sp.Student)
                .HasForeignKey<StudentParent>(sp => sp.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Student>()
                .HasOne(s => s.StudentMiscellaneous)
                .WithOne(sm => sm.Student)
                .HasForeignKey<StudentMiscellaneous>(sm => sm.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict Tenant relationships for Student and all child entities to prevent cascade conflicts
            builder.Entity<Student>()
                .HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentInfo>()
                .HasOne(si => si.Tenant)
                .WithMany()
                .HasForeignKey(si => si.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentAddress>()
                .HasOne(sa => sa.Tenant)
                .WithMany()
                .HasForeignKey(sa => sa.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentFinancial>()
                .HasOne(sf => sf.Tenant)
                .WithMany()
                .HasForeignKey(sf => sf.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentParent>()
                .HasOne(sp => sp.Tenant)
                .WithMany()
                .HasForeignKey(sp => sp.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentMiscellaneous>()
                .HasOne(sm => sm.Tenant)
                .WithMany()
                .HasForeignKey(sm => sm.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure StudentAttendance relationships to prevent cascade conflicts
            builder.Entity<StudentAttendance>()
                .HasOne(sa => sa.Student)
                .WithMany()
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentAttendance>()
                .HasOne(sa => sa.Class)
                .WithMany()
                .HasForeignKey(sa => sa.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentAttendance>()
                .HasOne(sa => sa.Section)
                .WithMany()
                .HasForeignKey(sa => sa.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentAttendance>()
                .HasOne(sa => sa.Session)
                .WithMany()
                .HasForeignKey(sa => sa.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentAttendance>()
                .HasOne(sa => sa.Tenant)
                .WithMany()
                .HasForeignKey(sa => sa.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Session Tenant relationship
            builder.Entity<Session>()
                .HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Subject Module relationships to prevent cascade conflicts
            builder.Entity<SubjectClass>()
                .HasOne(sc => sc.Subject)
                .WithMany(s => s.SubjectClasses)
                .HasForeignKey(sc => sc.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SubjectClass>()
                .HasOne(sc => sc.Class)
                .WithMany(c => c.SubjectClasses)
                .HasForeignKey(sc => sc.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Subject>()
                .HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure TeacherSubjects relationships to prevent cascade conflicts
            builder.Entity<TeacherSubjects>()
                .HasOne(ts => ts.Teacher)
                .WithMany(s => s.TeacherSubjects)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TeacherSubjects>()
                .HasOne(ts => ts.Subject)
                .WithMany(s => s.TeacherSubjects)
                .HasForeignKey(ts => ts.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ClassTeacher relationships to prevent cascade conflicts
            builder.Entity<ClassTeacher>()
                .HasOne(ct => ct.ClassSection)
                .WithMany()
                .HasForeignKey(ct => ct.ClassSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClassTeacher>()
                .HasOne(ct => ct.Staff)
                .WithMany()
                .HasForeignKey(ct => ct.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClassTeacher>()
                .HasOne(ct => ct.Session)
                .WithMany()
                .HasForeignKey(ct => ct.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Staff Tenant relationship
            builder.Entity<Staff>()
                .HasOne(s => s.Tenant)
                .WithMany()
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Department Tenant relationship
            builder.Entity<Department>()
                .HasOne(d => d.Tenant)
                .WithMany()
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure StudentCategory and SchoolHouse Tenant relationships
            builder.Entity<StudentCategory>()
                .HasOne(sc => sc.Tenant)
                .WithMany()
                .HasForeignKey(sc => sc.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SchoolHouse>()
                .HasOne(sh => sh.Tenant)
                .WithMany()
                .HasForeignKey(sh => sh.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Timetable relationships to prevent cascade conflicts
            builder.Entity<Timetable>()
                .HasOne(t => t.ClassSection)
                .WithMany()
                .HasForeignKey(t => t.ClassSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Timetable>()
                .HasOne(t => t.Subject)
                .WithMany()
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Timetable>()
                .HasOne(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Timetable>()
                .HasOne(t => t.Session)
                .WithMany()
                .HasForeignKey(t => t.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure FeeAssignment relationships to prevent cascade conflicts
            builder.Entity<FeeAssignment>()
                .HasOne(fa => fa.Student)
                .WithMany()
                .HasForeignKey(fa => fa.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeAssignment>()
                .HasOne(fa => fa.Application)
                .WithMany()
                .HasForeignKey(fa => fa.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeAssignment>()
                .HasOne(fa => fa.FeeGroupFeeType)
                .WithMany(fgft => fgft.FeeAssignments)
                .HasForeignKey(fa => fa.FeeGroupFeeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeAssignment>()
                .HasOne(fa => fa.AppliedDiscount)
                .WithMany(fd => fd.FeeAssignments)
                .HasForeignKey(fa => fa.FeeDiscountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Admission Module Tenant relationships
            builder.Entity<Enquiry>()
                .HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PreAdmission>()
                .HasOne(pa => pa.Tenant)
                .WithMany()
                .HasForeignKey(pa => pa.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EntryTest>()
                .HasOne(et => et.Tenant)
                .WithMany()
                .HasForeignKey(et => et.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Admission>()
                .HasOne(a => a.Tenant)
                .WithMany()
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ignore all computed properties in Student entity
            builder.Entity<Student>()
                .Ignore(s => s.AdmissionNo)
                .Ignore(s => s.FirstName)
                .Ignore(s => s.LastName)
                .Ignore(s => s.FullName);

            // ===== EXAM MODULE RELATIONSHIPS =====
            // Configure Exam module foreign keys to prevent cascade delete conflicts
            
            // Exam relationships
            builder.Entity<Exam>()
                .HasOne(e => e.Class)
                .WithMany()
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Exam>()
                .HasOne(e => e.Section)
                .WithMany()
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamSubject relationships
            builder.Entity<ExamSubject>()
                .HasOne(es => es.Exam)
                .WithMany(e => e.ExamSubjects)
                .HasForeignKey(es => es.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamSubject>()
                .HasOne(es => es.Subject)
                .WithMany()
                .HasForeignKey(es => es.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamMarks relationships
            builder.Entity<ExamMarks>()
                .HasOne(em => em.ExamSubject)
                .WithMany(es => es.ExamMarks)
                .HasForeignKey(em => em.ExamSubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamMarks>()
                .HasOne(em => em.Student)
                .WithMany()
                .HasForeignKey(em => em.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamMarks>()
                .HasOne(em => em.Grade)
                .WithMany(g => g.ExamMarks)
                .HasForeignKey(em => em.GradeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamResult relationships
            builder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(e => e.ExamResults)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamResult>()
                .HasOne(er => er.Student)
                .WithMany()
                .HasForeignKey(er => er.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamResult>()
                .HasOne(er => er.OverallGrade)
                .WithMany(g => g.ExamResults)
                .HasForeignKey(er => er.OverallGradeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamTimetable relationships
            builder.Entity<ExamTimetable>()
                .HasOne(et => et.Exam)
                .WithMany(e => e.ExamTimetables)
                .HasForeignKey(et => et.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamTimetable>()
                .HasOne(et => et.ExamSubject)
                .WithOne(es => es.ExamTimetable)
                .HasForeignKey<ExamTimetable>(et => et.ExamSubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamTimetable>()
                .HasOne(et => et.Invigilator)
                .WithMany()
                .HasForeignKey(et => et.InvigilatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grade relationships
            builder.Entity<Grade>()
                .HasOne(g => g.GradeScale)
                .WithMany(gs => gs.Grades)
                .HasForeignKey(g => g.GradeScaleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== END EXAM MODULE RELATIONSHIPS =====

            // ===== MULTI-TENANCY: GLOBAL QUERY FILTERS =====
            // Automatically filter all queries by TenantId
            // This ensures users can only see data from their own tenant/school
            // CRITICAL SECURITY: Never use IgnoreQueryFilters() in production without explicit authorization
            // IMPORTANT: Don't capture tenantId in a variable! Call GetCurrentTenantId() directly in each filter
            //            so it evaluates PER-QUERY, not once at startup
            
            // Apply query filters to all entities inheriting from BaseEntity
            // Each filter calls GetCurrentTenantId() which will be evaluated at query execution time
            builder.Entity<Student>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<StudentInfo>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<StudentAddress>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<StudentFinancial>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<StudentParent>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<StudentMiscellaneous>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Staff>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Class>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<ClassSection>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Section>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Subject>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<SubjectClass>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Session>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Department>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<StudentCategory>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<SchoolHouse>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<DisableReason>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            
            // Fee Management
            builder.Entity<FeeType>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<FeeGroup>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<FeeGroupFeeType>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<FeeDiscount>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<FeeAssignment>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<FeeTransaction>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            
            // Income & Expense
            builder.Entity<Income>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<IncomeHead>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Expense>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<ExpenseHead>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            
            // Attendance
            builder.Entity<StudentAttendance>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            
            // Timetable & Teachers
            builder.Entity<Timetable>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<ClassTeacher>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<TeacherSubjects>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            
            // Admissions
            builder.Entity<Enquiry>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<PreAdmission>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<EntryTest>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Admission>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            
            // Class Assignments
            builder.Entity<StudentClassAssignment>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());

            // Exam
            builder.Entity<Exam>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<ExamMarks>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<ExamResult>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<ExamSubject>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<ExamTimetable>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<GradeScale>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<Grade>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            builder.Entity<MarksheetTemplate>().HasQueryFilter(x => x.TenantId == GetCurrentTenantId());
            
            // Note: ApplicationUser already filtered by TenantId at user level
            // Note: Tenant table itself should NOT have a query filter
            // ===== END MULTI-TENANCY FILTERS =====

            // ===== PERFORMANCE: DATABASE INDEXES =====
            // Add composite indexes for frequently queried field combinations
            
            // Exam module indexes
            builder.Entity<Exam>()
                .HasIndex(e => new { e.ClassId, e.SectionId, e.AcademicYear, e.TenantId })
                .HasDatabaseName("IX_Exam_Class_Section_Year_Tenant");

            builder.Entity<Exam>()
                .HasIndex(e => new { e.Status, e.IsDeleted, e.TenantId })
                .HasDatabaseName("IX_Exam_Status_Deleted_Tenant");

            builder.Entity<Exam>()
                .HasIndex(e => new { e.StartDate, e.EndDate, e.TenantId })
                .HasDatabaseName("IX_Exam_DateRange_Tenant");

            // Section module indexes
            builder.Entity<Section>()
                .HasIndex(s => new { s.Name, s.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Section_Name_Tenant_Unique");

            // Subject module indexes
            builder.Entity<Subject>()
                .HasIndex(s => new { s.Code, s.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Subject_Code_Tenant_Unique");

            builder.Entity<Subject>()
                .HasIndex(s => new { s.Name, s.TenantId })
                .HasDatabaseName("IX_Subject_Name_Tenant");

            // Class module indexes
            builder.Entity<Class>()
                .HasIndex(c => new { c.Name, c.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Class_Name_Tenant_Unique");

            // Student module indexes
            builder.Entity<Student>()
                .HasIndex(s => new { s.TenantId, s.IsDeleted, s.IsActive })
                .HasDatabaseName("IX_Student_Tenant_Deleted_Active");

            // Attendance module indexes
            builder.Entity<StudentAttendance>()
                .HasIndex(sa => new { sa.StudentId, sa.AttendanceDate, sa.TenantId })
                .HasDatabaseName("IX_Attendance_Student_Date_Tenant");

            // Fee module indexes
            // Index for student transactions (filtered to exclude provisional fees where StudentId is NULL)
            builder.Entity<FeeTransaction>()
                .HasIndex(ft => new { ft.StudentId, ft.PaymentDate, ft.TenantId })
                .HasDatabaseName("IX_FeeTransaction_Student_Date_Tenant")
                .HasFilter("[StudentId] IS NOT NULL"); // Allow NULL StudentId for provisional fees
            
            // Index for provisional fee transactions (ApplicationId)
            builder.Entity<FeeTransaction>()
                .HasIndex(ft => new { ft.ApplicationId, ft.PaymentDate, ft.TenantId })
                .HasDatabaseName("IX_FeeTransaction_Application_Date_Tenant")
                .HasFilter("[ApplicationId] IS NOT NULL");

            // ===== END DATABASE INDEXES =====
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected virtual void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;
            var now = DateTime.Now;
            var user = GetCurrentUser();
            var tenantId = GetCurrentTenantId(); // Get current tenant ID from JWT claims

            foreach (var entry in entries)
            {
                if (entry.Entity is Domain.Entities.BaseEntity baseEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // Prevent modification of audit fields and tenant ID
                            entry.Property(nameof(baseEntity.CreatedAt)).IsModified = false;
                            entry.Property(nameof(baseEntity.CreatedBy)).IsModified = false;
                            entry.Property(nameof(baseEntity.TenantId)).IsModified = false; // SECURITY: Prevent tenant switching
                            
                            baseEntity.UpdatedAt = now;
                            baseEntity.UpdatedBy = user;
                            break;

                        case EntityState.Added:
                            // Auto-set tenant ID for new entities
                            // CRITICAL SECURITY: All new entities must belong to current tenant
                            baseEntity.TenantId = tenantId;
                            
                            baseEntity.CreatedAt = now;
                            baseEntity.CreatedAtUtc = utcNow;
                            baseEntity.CreatedBy = user;
                            baseEntity.UpdatedAt = now;
                            baseEntity.UpdatedBy = user;
                            break;
                    }
                }
                
                // Handle ApplicationUser (Identity) separately since it doesn't inherit from BaseEntity
                if (entry.Entity is ApplicationUser appUser)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            // Auto-set tenant ID for new users
                            if (appUser.TenantId == 0)
                            {
                                appUser.TenantId = tenantId;
                            }
                            break;
                        
                        case EntityState.Modified:
                            // SECURITY: Prevent tenant ID modification for existing users
                            entry.Property(nameof(appUser.TenantId)).IsModified = false;
                            break;
                    }
                }
            }
        }
        
        // Multi-Tenancy
        public virtual DbSet<Tenant> Tenants { get; set; }

        public virtual DbSet<ApplicationUser> User { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<StatusType> StatusTypes { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }


        // automatic code generation area

        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentInfo> StudentInfo { get; set; }
        public virtual DbSet<StudentAddress> StudentAddress { get; set; }
        public virtual DbSet<StudentFinancial> StudentFinancial { get; set; }
        public virtual DbSet<StudentParent> StudentParent { get; set; }
        public virtual DbSet<StudentMiscellaneous> StudentMiscellaneous { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<ClassSection> ClassSections { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<StudentClassAssignment> StudentClasses { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<Subject> Subject { get; set; }
        public virtual DbSet<SubjectClass> SubjectClass { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StudentCategory> StudentCategory { get; set; }
        public virtual DbSet<ClassTeacher> ClassTeacher { get; set; }
        public virtual DbSet<SchoolHouse> SchoolHouse { get; set; }
        public virtual DbSet<TeacherSubjects> TeacherSubjects { get; set; }
        public virtual DbSet<Timetable> Timetable { get; set; }
        public virtual DbSet<StudentAttendance> StudentAttendance { get; set; }
        public virtual DbSet<DisableReason> DisableReason { get; set; }
        public virtual DbSet<FeeType> FeeType { get; set; }
        public virtual DbSet<FeeGroup> FeeGroup { get; set; }
        public virtual DbSet<FeeGroupFeeType> FeeGroupFeeType { get; set; }
        public virtual DbSet<FeeDiscount> FeeDiscount { get; set; }
        public virtual DbSet<FeeTransaction> FeeTransaction { get; set; }
        public virtual DbSet<FeeAssignment> FeeAssignment { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<RolePermission> RolePermission { get; set; }
        public virtual DbSet<ExpenseHead> ExpenseHead { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<IncomeHead> IncomeHead { get; set; }
        public virtual DbSet<Income> Income { get; set; }
        public virtual DbSet<Enquiry> Enquiry { get; set; }
        public virtual DbSet<PreAdmission> PreAdmission { get; set; }
        public virtual DbSet<EntryTest> EntryTest { get; set; }
        public virtual DbSet<Admission> Admission { get; set; }
        public virtual DbSet<FineWaiver> FineWaiver { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<ExamMarks> ExamMarks { get; set; }
        public virtual DbSet<ExamResult> ExamResults { get; set; }
        public virtual DbSet<ExamSubject> ExamSubjects { get; set; }
        public virtual DbSet<ExamTimetable> ExamTimetables { get; set; }
        public virtual DbSet<GradeScale> GradeScales { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<MarksheetTemplate> MarksheetTemplates { get; set; }
        // automatic code generation area end
        
        /// <summary>
        /// Gets the current tenant ID from multiple sources (priority order):
        /// 1. HttpContext.Items["TenantId"] - Set by TenantMiddleware from X-Tenant-Id header
        /// 2. JWT claims - Fallback for backward compatibility
        /// 3. Default (1) - For migrations, seeding, background jobs
        /// </summary>
        private int GetCurrentTenantId()
        {
            // Priority 1: Get from HttpContext.Items (set by TenantMiddleware from X-Tenant-Id header)
            if (_httpContextAccessor?.HttpContext?.Items?.ContainsKey("TenantId") == true)
            {
                var tenantIdFromHeader = _httpContextAccessor.HttpContext.Items["TenantId"];
                if (tenantIdFromHeader is int headerTenantId && headerTenantId > 0)
                {
                    return headerTenantId;
                }
            }
            
            // Priority 2: Get from JWT claims (backward compatibility)
            var tenantIdClaim = GetContextClaims("TenantId") ?? GetContextClaims("tenantId");
            if (int.TryParse(tenantIdClaim, out var tenantId) && tenantId > 0)
            {
                return tenantId;
            }
            
            // Priority 3: FALLBACK - Return default tenant ID (1)
            // This happens during migrations, seeding, or background jobs
            // IMPORTANT: Ensure Tenant with ID = 1 exists in your Tenants table
            return 1;
        }

        private string GetCurrentUser() => $"{GetContextClaims(JwtClaimTypes.Name) ?? string.Empty} : {GetContextClaims(ClaimTypes.Role) ?? "<unknown>"}";
        private string? GetContextClaims(string typeName) => _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c?.Type == typeName)?.Value;
    }
}
