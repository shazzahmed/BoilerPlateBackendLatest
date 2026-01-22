
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class StudentAttendanceRepository : BaseRepository<StudentAttendance, int>, IStudentAttendanceRepository
    {
        private readonly ISqlServerDbContext _context;

        public StudentAttendanceRepository(ISqlServerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<StudentAttendance>> GetAttendanceByClassSectionAsync(int classId, int sectionId, DateTime attendanceDate)
        {
            return await _context.StudentAttendance
                .Where(a => a.ClassId == classId &&
                           a.SectionId == sectionId &&
                           a.AttendanceDate.Date == attendanceDate.Date &&
                           a.IsActive &&
                           !a.IsDeleted)
                .Include(a => a.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(a => a.Class)
                .Include(a => a.Section)
                .ToListAsync();
        }

        public async Task<List<StudentAttendance>> GetAttendanceStatisticsAsync(int classId, int sectionId, DateTime startDate, DateTime endDate)
        {
            return await _context.StudentAttendance
                .Where(a => a.ClassId == classId &&
                           a.SectionId == sectionId &&
                           a.AttendanceDate.Date >= startDate.Date &&
                           a.AttendanceDate.Date <= endDate.Date &&
                           a.IsActive &&
                           !a.IsDeleted)
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Include(a => a.Section)
                .ToListAsync();
        }

        public async Task DeleteAttendanceByClassSectionAsync(int classId, int sectionId, DateTime attendanceDate)
        {
            var attendanceRecords = await _context.StudentAttendance
                .Where(a => a.ClassId == classId &&
                           a.SectionId == sectionId &&
                           a.AttendanceDate.Date == attendanceDate.Date)
                .ToListAsync();

            foreach (var record in attendanceRecords)
            {
                record.IsDeleted = true;
                record.UpdatedAt = DateTime.UtcNow;
                record.UpdatedBy = "System"; // TODO: Get from current user context
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<StudentAttendance>> GetAttendanceByStudentAsync(int studentId, DateTime startDate, DateTime endDate)
        {
            return await _context.StudentAttendance
                .Where(a => a.StudentId == studentId &&
                           a.AttendanceDate.Date >= startDate.Date &&
                           a.AttendanceDate.Date <= endDate.Date &&
                           a.IsActive &&
                           !a.IsDeleted)
                .Include(a => a.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(a => a.Class)
                .Include(a => a.Section)
                .OrderBy(a => a.AttendanceDate)
                .ToListAsync();
        }

        public async Task<List<StudentAttendance>> GetAttendanceByDateAsync(DateTime attendanceDate)
        {
            return await _context.StudentAttendance
                .Where(a => a.AttendanceDate.Date == attendanceDate.Date &&
                           a.IsActive &&
                           !a.IsDeleted)
                .Include(a => a.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(a => a.Class)
                .Include(a => a.Section)
                .ToListAsync();
        }

        public async Task<bool> AttendanceExistsAsync(int classId, int sectionId, DateTime attendanceDate)
        {
            return await _context.StudentAttendance
                .AnyAsync(a => a.ClassId == classId &&
                              a.SectionId == sectionId &&
                              a.AttendanceDate.Date == attendanceDate.Date &&
                              a.IsActive &&
                              !a.IsDeleted);
        }

        // ========================================
        // NEW METHODS FOR PHASE 1
        // ========================================

        public async Task<StudentAttendance?> GetAttendanceByIdAsync(int id)
        {
            return await _context.StudentAttendance
                .Where(a => a.Id == id && !a.IsDeleted)
                .Include(a => a.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(a => a.Class)
                .Include(a => a.Section)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AttendanceExistsByStudentAndDateAsync(int studentId, DateTime attendanceDate)
        {
            return await _context.StudentAttendance
                .AnyAsync(a => a.StudentId == studentId &&
                              a.AttendanceDate.Date == attendanceDate.Date &&
                              a.IsActive &&
                              !a.IsDeleted);
        }

        public async Task UpdateAttendanceAsync(StudentAttendance attendance)
        {
            attendance.UpdatedAt = DateTime.UtcNow;
            // UpdatedBy should be set by the service from user context
            _context.StudentAttendance.Update(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAttendanceAsync(int id)
        {
            var attendance = await _context.StudentAttendance.FindAsync(id);
            if (attendance != null)
            {
                // Soft delete
                attendance.IsDeleted = true;
                attendance.IsActive = false;
                attendance.UpdatedAt = DateTime.UtcNow;
                // UpdatedBy should be set by the service from user context
                _context.StudentAttendance.Update(attendance);
                await _context.SaveChangesAsync();
            }
        }


        // ========================================
        // PHASE 2: REPORTING METHODS
        // ========================================

        public async Task<int> GetWorkingDaysAsync(int month, int year, int? classId, int? sectionId)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Calculate working days (exclude weekends)
            var workingDays = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            // TODO: Exclude holidays from tenant calendar when Holiday entity is available
            // var holidays = await _context.Holidays
            //     .Where(h => h.Date >= startDate && h.Date <= endDate && h.TenantId == _tenantId)
            //     .CountAsync();
            // workingDays -= holidays;

            return workingDays;
        }

        public async Task<List<StudentAttendance>> GetAttendanceByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            int? classId,
            int? sectionId,
            int? sessionId)
        {
        var query = _context.StudentAttendance
            .Include(a => a.Student)
                .ThenInclude(s => s.StudentInfo)
            .Include(a => a.Student)
                .ThenInclude(s => s.StudentParent)
            .Include(a => a.Class)
            .Include(a => a.Section)
            .AsNoTracking()
            .Where(a =>
                a.AttendanceDate.Date >= startDate.Date &&
                a.AttendanceDate.Date <= endDate.Date &&
                !a.IsDeleted);

            if (classId.HasValue)
                query = query.Where(a => a.ClassId == classId.Value);

            if (sectionId.HasValue)
                query = query.Where(a => a.SectionId == sectionId.Value);

            if (sessionId.HasValue)
                query = query.Where(a => a.SessionId == sessionId.Value);

            return await query.OrderBy(a => a.AttendanceDate).ToListAsync();
        }

        public async Task<List<StudentAttendance>> GetAttendanceForStudentsByDateRangeAsync(
            List<int> studentIds,
            DateTime startDate,
            DateTime endDate)
        {
        return await _context.StudentAttendance
            .Include(a => a.Student)
                .ThenInclude(s => s.StudentInfo)
            .Include(a => a.Class)
            .Include(a => a.Section)
            .AsNoTracking()
            .Where(a =>
                studentIds.Contains(a.StudentId) &&
                a.AttendanceDate.Date >= startDate.Date &&
                a.AttendanceDate.Date <= endDate.Date &&
                !a.IsDeleted)
            .OrderBy(a => a.StudentId)
            .ThenBy(a => a.AttendanceDate)
            .ToListAsync();
        }
    }
}

