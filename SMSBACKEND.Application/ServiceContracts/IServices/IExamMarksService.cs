using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IExamMarksService : IBaseService<ExamMarksModel, ExamMarks, int>
    {
        Task<BaseModel> GetMarksByExamSubjectAsync(int examSubjectId);
        Task<BaseModel> GetMarksByStudentAsync(int studentId, int examId);
        Task<BaseModel> GetStudentExamMarksAsync(int examSubjectId, int studentId);
        Task<BaseModel> BulkCreateMarksAsync(BulkExamMarksCreateRequest request);
        Task<BaseModel> BulkUpdateMarksAsync(List<ExamMarksUpdateRequest> requests);
        Task<BaseModel> VerifyMarksAsync(VerifyMarksRequest request);
        Task<BaseModel> GetMarksByExamAsync(int examId);
        Task<BaseModel> ImportMarksFromExcelAsync(ExamMarksImportRequest request);
    }
}


