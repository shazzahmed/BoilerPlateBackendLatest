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
    public class GradeScaleRepository : BaseRepository<GradeScale, int>, IGradeScaleRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;

        public GradeScaleRepository(
            IMapper mapper,
            ISqlServerDbContext context,
            ICacheProvider cacheProvider,
            SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }

        public async Task<GradeScaleModel> GetDefaultGradeScaleAsync()
        {
            var gradeScale = await DbContext.Set<GradeScale>()
                .Include(gs => gs.Grades.OrderBy(g => g.DisplayOrder))
                .Where(gs => gs.IsDefault && gs.IsActive && !gs.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<GradeScaleModel>(gradeScale);
        }

        public async Task<GradeScaleModel> GetGradeScaleWithGradesAsync(int gradeScaleId)
        {
            var gradeScale = await DbContext.Set<GradeScale>()
                .Include(gs => gs.Grades.OrderBy(g => g.DisplayOrder))
                .Where(gs => gs.Id == gradeScaleId && !gs.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<GradeScaleModel>(gradeScale);
        }

        public async Task<bool> SetDefaultGradeScaleAsync(int gradeScaleId)
        {
            // Remove default from all
            var allGradeScales = await DbContext.Set<GradeScale>()
                .Where(gs => !gs.IsDeleted)
                .ToListAsync();

            foreach (var gs in allGradeScales)
            {
                gs.IsDefault = (gs.Id == gradeScaleId);
            }

            DbContext.Set<GradeScale>().UpdateRange(allGradeScales);
            await DbContext.SaveChangesAsync();

            return true;
        }
    }
}


