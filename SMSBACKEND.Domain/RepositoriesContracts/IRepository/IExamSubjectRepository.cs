using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IExamSubjectRepository : IBaseRepository<ExamSubject, int>
    {
        Task<List<ExamSubjectModel>> GetSubjectsByExamAsync(int examId);
        Task<ExamSubjectModel> GetExamSubjectWithDetailsAsync(int examSubjectId);
        Task<bool> BulkCreateExamSubjectsAsync(int examId, List<ExamSubject> subjects);
        Task<bool> DeleteExamSubjectsAsync(int examId);
    }
}


