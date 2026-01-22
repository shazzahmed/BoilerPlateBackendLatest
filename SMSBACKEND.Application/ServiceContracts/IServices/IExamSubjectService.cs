using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IExamSubjectService : IBaseService<ExamSubjectModel, ExamSubject, int>
    {
        Task<BaseModel> GetSubjectsByExamAsync(int examId);
        Task<BaseModel> GetExamSubjectWithDetailsAsync(int examSubjectId);
        Task<BaseModel> BulkCreateExamSubjectsAsync(BulkExamSubjectCreateRequest request);
        Task<BaseModel> DeleteExamSubjectsAsync(int examId);
    }
}


