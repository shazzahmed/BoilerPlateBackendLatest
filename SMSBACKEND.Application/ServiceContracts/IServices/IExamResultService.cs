using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IExamResultService : IBaseService<ExamResultModel, ExamResult, int>
    {
        Task<BaseModel> GetResultsByExamAsync(int examId);
        Task<BaseModel> GetStudentExamResultAsync(int examId, int studentId);
        Task<BaseModel> GenerateExamResultsAsync(GenerateExamResultRequest request);
        Task<BaseModel> PublishResultsAsync(PublishExamResultRequest request);
        Task<BaseModel> GetTopRankersAsync(int examId, int topCount = 10);
        Task<BaseModel> GenerateReportCardAsync(StudentReportCardRequest request);
        Task<BaseModel> GenerateBulkReportCardsAsync(BulkReportCardRequest request);
    }
}


