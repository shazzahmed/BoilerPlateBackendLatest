using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IGradeScaleRepository : IBaseRepository<GradeScale, int>
    {
        Task<GradeScaleModel> GetDefaultGradeScaleAsync();
        Task<GradeScaleModel> GetGradeScaleWithGradesAsync(int gradeScaleId);
        Task<bool> SetDefaultGradeScaleAsync(int gradeScaleId);
    }
}


