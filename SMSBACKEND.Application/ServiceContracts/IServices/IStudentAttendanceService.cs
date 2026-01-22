
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IStudentAttendanceService : IBaseService<StudentAttendanceModel, StudentAttendance, int>
    {
        Task<BaseModel> MarkAttendance(MarkAttendanceRequest request);
        Task<BaseModel> GetAttendanceStatistics(GetAttendanceStatisticsRequest request);
        Task<BaseModel> GetAllAttendance(); // Returns all attendance for frontend caching
        
        // New methods for Phase 1
        Task<BaseModel> GetAttendanceByClassSection(GetAttendanceByClassSectionRequest request);
        Task<BaseModel> GetAttendanceByStudent(GetAttendanceByStudentRequest request);
        Task<BaseModel> GetAttendanceByDate(GetAttendanceByDateRequest request);
        Task<BaseModel> UpdateAttendance(int id, UpdateAttendanceRequest request);
        Task<BaseModel> DeleteAttendance(int id);
        
        // Phase 2: Reporting methods
        Task<BaseModel> GetMonthlyReportAsync(GetMonthlyReportRequest request);
        Task<BaseModel> GetAttendancePercentageAsync(GetAttendancePercentageRequest request);
        Task<BaseModel> GetDefaultersAsync(GetDefaultersRequest request);
        Task<BaseModel> GetAttendanceSummaryAsync(GetAttendanceSummaryRequest request);
        Task<BaseModel> ExportAttendanceAsync(ExportAttendanceRequest request);
    }
}

