using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IExamTimetableRepository : IBaseRepository<ExamTimetable, int>
    {
        Task<List<ExamTimetableModel>> GetTimetableByExamAsync(int examId);
        Task<ExamTimetableModel> GetTimetableByExamSubjectAsync(int examSubjectId);
        Task<bool> BulkCreateTimetableAsync(List<ExamTimetable> timetables);
        Task<List<ExamTimetableModel>> GetTimetableByDateAsync(DateTime date);
        Task<List<ExamTimetableModel>> GetTimetableByInvigilatorAsync(int invigilatorId);
    }
}


