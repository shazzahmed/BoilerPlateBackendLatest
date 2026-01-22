using Common.DTO.Request;
using Common.DTO.Response;
using Domain.Entities;
using static Common.Utilities.Enums;

namespace Domain.RepositoriesContracts
{
    public interface IExamRepository : IBaseRepository<Exam, int>
    {
        Task<List<ExamModel>> GetExamsByClassAsync(int classId, int? sectionId = null);
        Task<List<ExamModel>> GetExamsByAcademicYearAsync(string academicYear);
        Task<ExamModel> GetExamWithDetailsAsync(int examId);
        Task<bool> PublishExamAsync(int examId, bool isPublished);
        Task<List<ExamModel>> GetExamsByStatusAsync(ExamStatus status);
    }
}

