using Common.DTO.Request;
using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IExamMarksRepository : IBaseRepository<ExamMarks, int>
    {
        Task<List<ExamMarksModel>> GetMarksByExamSubjectAsync(int examSubjectId);
        Task<List<ExamMarksModel>> GetMarksByStudentAsync(int studentId, int examId);
        Task<ExamMarksModel> GetStudentExamMarksAsync(int examSubjectId, int studentId);
        Task<bool> BulkCreateMarksAsync(List<ExamMarks> marks);
        Task<bool> BulkUpdateMarksAsync(List<ExamMarks> marks);
        Task<bool> VerifyMarksAsync(List<int> marksIds, string verifiedBy);
        Task<List<ExamMarksModel>> GetMarksByExamAsync(int examId);
        Task<int> GetTotalStudentsInExamAsync(int examId);
    }
}


