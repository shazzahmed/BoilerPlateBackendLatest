using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IExamResultRepository : IBaseRepository<ExamResult, int>
    {
        Task<List<ExamResultModel>> GetResultsByExamAsync(int examId);
        Task<ExamResultModel> GetStudentExamResultAsync(int examId, int studentId);
        Task<bool> GenerateExamResultsAsync(int examId, int? studentId = null);
        Task<bool> PublishResultsAsync(int examId, List<int> studentIds);
        Task<bool> CalculateRanksAsync(int examId);
        Task<List<ExamResultModel>> GetTopRankersAsync(int examId, int topCount = 10);
    }
}


