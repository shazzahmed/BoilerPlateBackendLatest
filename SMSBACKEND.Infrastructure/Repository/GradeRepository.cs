using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services.Communication;
using Application.ServiceContracts;

namespace Infrastructure.Repository
{
    public class GradeRepository : BaseRepository<Grade, int>, IGradeRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;

        public GradeRepository(
            IMapper mapper,
            ISqlServerDbContext context,
            ICacheProvider cacheProvider,
            SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }

        public async Task<List<GradeModel>> GetGradesByScaleAsync(int gradeScaleId)
        {
            var grades = await DbContext.Set<Grade>()
                .Where(g => g.GradeScaleId == gradeScaleId && !g.IsDeleted)
                .OrderBy(g => g.DisplayOrder)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<GradeModel>>(grades);
        }

        public async Task<GradeModel> GetGradeByPercentageAsync(int gradeScaleId, decimal percentage)
        {
            var grade = await DbContext.Set<Grade>()
                .Where(g => g.GradeScaleId == gradeScaleId &&
                           percentage >= g.MinPercentage &&
                           percentage <= g.MaxPercentage &&
                           !g.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<GradeModel>(grade);
        }

        public async Task<bool> BulkCreateGradesAsync(int gradeScaleId, List<Grade> grades)
        {
            if (!grades.Any()) return false;

            // Set GradeScaleId for all grades
            foreach (var grade in grades)
            {
                grade.GradeScaleId = gradeScaleId;
                grade.CreatedAt = DateTime.Now;
                grade.CreatedAtUtc = DateTime.UtcNow;
            }

            await DbContext.Set<Grade>().AddRangeAsync(grades);
            await DbContext.SaveChangesAsync();

            return true;
        }
    }
}


