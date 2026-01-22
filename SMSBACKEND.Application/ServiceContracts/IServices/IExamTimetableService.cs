using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IExamTimetableService : IBaseService<ExamTimetableModel, ExamTimetable, int>
    {
        Task<BaseModel> GetTimetableByExamAsync(int examId);
        Task<BaseModel> GetTimetableByExamSubjectAsync(int examSubjectId);
        Task<BaseModel> BulkCreateTimetableAsync(BulkExamTimetableCreateRequest request);
        Task<BaseModel> GetTimetableByDateAsync(DateTime date);
        Task<BaseModel> GetTimetableByInvigilatorAsync(int invigilatorId);
    }
}


