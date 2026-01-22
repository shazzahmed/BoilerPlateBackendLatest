using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IGradeRepository : IBaseRepository<Grade, int>
    {
        Task<List<GradeModel>> GetGradesByScaleAsync(int gradeScaleId);
        Task<GradeModel> GetGradeByPercentageAsync(int gradeScaleId, decimal percentage);
        Task<bool> BulkCreateGradesAsync(int gradeScaleId, List<Grade> grades);
    }
}


