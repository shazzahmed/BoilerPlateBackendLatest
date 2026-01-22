using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface IGradeService : IBaseService<GradeModel, Grade, int>
    {
        Task<BaseModel> GetGradesByScaleAsync(int gradeScaleId);
        Task<BaseModel> GetGradeByPercentageAsync(int gradeScaleId, decimal percentage);
    }
}


