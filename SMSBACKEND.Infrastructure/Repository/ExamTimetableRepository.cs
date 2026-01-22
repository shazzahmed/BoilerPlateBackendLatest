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
    public class ExamTimetableRepository : BaseRepository<ExamTimetable, int>, IExamTimetableRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;

        public ExamTimetableRepository(
            IMapper mapper,
            ISqlServerDbContext context,
            ICacheProvider cacheProvider,
            SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }

        public async Task<List<ExamTimetableModel>> GetTimetableByExamAsync(int examId)
        {
            var timetables = await DbContext.Set<ExamTimetable>()
                .Include(t => t.Exam)
                .Include(t => t.ExamSubject)
                    .ThenInclude(es => es.Subject)
                .Include(t => t.Invigilator)
                .Where(t => t.ExamId == examId && !t.IsDeleted)
                .OrderBy(t => t.ExamDate)
                .ThenBy(t => t.StartTime)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamTimetableModel>>(timetables);
        }

        public async Task<ExamTimetableModel> GetTimetableByExamSubjectAsync(int examSubjectId)
        {
            var timetable = await DbContext.Set<ExamTimetable>()
                .Include(t => t.Exam)
                .Include(t => t.ExamSubject)
                    .ThenInclude(es => es.Subject)
                .Include(t => t.Invigilator)
                .Where(t => t.ExamSubjectId == examSubjectId && !t.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<ExamTimetableModel>(timetable);
        }

        public async Task<bool> BulkCreateTimetableAsync(List<ExamTimetable> timetables)
        {
            if (!timetables.Any()) return false;

            foreach (var timetable in timetables)
            {
                timetable.CreatedAt = DateTime.Now;
                timetable.CreatedAtUtc = DateTime.UtcNow;
            }

            await DbContext.Set<ExamTimetable>().AddRangeAsync(timetables);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<ExamTimetableModel>> GetTimetableByDateAsync(DateTime date)
        {
            var timetables = await DbContext.Set<ExamTimetable>()
                .Include(t => t.Exam)
                .Include(t => t.ExamSubject)
                    .ThenInclude(es => es.Subject)
                .Include(t => t.Invigilator)
                .Where(t => t.ExamDate.Date == date.Date && !t.IsDeleted)
                .OrderBy(t => t.StartTime)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamTimetableModel>>(timetables);
        }

        public async Task<List<ExamTimetableModel>> GetTimetableByInvigilatorAsync(int invigilatorId)
        {
            var timetables = await DbContext.Set<ExamTimetable>()
                .Include(t => t.Exam)
                .Include(t => t.ExamSubject)
                    .ThenInclude(es => es.Subject)
                .Include(t => t.Invigilator)
                .Where(t => t.InvigilatorId == invigilatorId && !t.IsDeleted)
                .OrderBy(t => t.ExamDate)
                .ThenBy(t => t.StartTime)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamTimetableModel>>(timetables);
        }
    }
}


