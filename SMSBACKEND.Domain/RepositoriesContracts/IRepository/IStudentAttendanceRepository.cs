
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IStudentAttendanceRepository : IBaseRepository<StudentAttendance, int>
    {
        Task<List<StudentAttendance>> GetAttendanceByClassSectionAsync(int classId, int sectionId, DateTime attendanceDate);
        Task<List<StudentAttendance>> GetAttendanceStatisticsAsync(int classId, int sectionId, DateTime startDate, DateTime endDate);
        Task DeleteAttendanceByClassSectionAsync(int classId, int sectionId, DateTime attendanceDate);
        Task<List<StudentAttendance>> GetAttendanceByStudentAsync(int studentId, DateTime startDate, DateTime endDate);
        Task<List<StudentAttendance>> GetAttendanceByDateAsync(DateTime attendanceDate);
        Task<bool> AttendanceExistsAsync(int classId, int sectionId, DateTime attendanceDate);
        
        // New methods for Phase 1
        Task<StudentAttendance?> GetAttendanceByIdAsync(int id);
        Task<bool> AttendanceExistsByStudentAndDateAsync(int studentId, DateTime attendanceDate);
        Task UpdateAttendanceAsync(StudentAttendance attendance);
        Task DeleteAttendanceAsync(int id);
        
        // Phase 2: Reporting methods
        Task<int> GetWorkingDaysAsync(int month, int year, int? classId, int? sectionId);
        Task<List<StudentAttendance>> GetAttendanceByDateRangeAsync(DateTime startDate, DateTime endDate, int? classId, int? sectionId, int? sessionId);
        Task<List<StudentAttendance>> GetAttendanceForStudentsByDateRangeAsync(List<int> studentIds, DateTime startDate, DateTime endDate);
    }
}
