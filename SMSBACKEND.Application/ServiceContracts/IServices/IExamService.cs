using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;
using static Common.Utilities.Enums;

namespace Application.ServiceContracts
{
    public interface IExamService : IBaseService<ExamModel, Exam, int>
    {
        Task<BaseModel> GetExamsByClassAsync(int classId, int? sectionId = null);
        Task<BaseModel> GetExamsByAcademicYearAsync(string academicYear);
        Task<BaseModel> GetExamWithDetailsAsync(int examId);
        Task<BaseModel> PublishExamAsync(ExamPublishRequest request);
        Task<BaseModel> GetExamsByStatusAsync(ExamStatus status);
    }
}


