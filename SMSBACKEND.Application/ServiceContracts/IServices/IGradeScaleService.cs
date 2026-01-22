using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IGradeScaleService : IBaseService<GradeScaleModel, GradeScale, int>
    {
        Task<BaseModel> GetDefaultGradeScaleAsync();
        Task<BaseModel> GetGradeScaleWithGradesAsync(int gradeScaleId);
        Task<BaseModel> SetDefaultGradeScaleAsync(int gradeScaleId);
        Task<BaseModel> CreateGradeScaleWithGradesAsync(GradeScaleCreateRequest request);
    }
}


