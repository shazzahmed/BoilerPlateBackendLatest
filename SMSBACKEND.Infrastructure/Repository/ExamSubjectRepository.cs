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
    public class ExamSubjectRepository : BaseRepository<ExamSubject, int>, IExamSubjectRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;

        public ExamSubjectRepository(
            IMapper mapper,
            ISqlServerDbContext context,
            ICacheProvider cacheProvider,
            SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }

        public async Task<List<ExamSubjectModel>> GetSubjectsByExamAsync(int examId)
        {
            var examSubjects = await DbContext.Set<ExamSubject>()
                .Include(es => es.Subject)
                .Include(es => es.Exam)
                .Where(es => es.ExamId == examId && !es.IsDeleted)
                .OrderBy(es => es.Subject.Name)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamSubjectModel>>(examSubjects);
        }

        public async Task<ExamSubjectModel> GetExamSubjectWithDetailsAsync(int examSubjectId)
        {
            var examSubject = await DbContext.Set<ExamSubject>()
                .Include(es => es.Subject)
                .Include(es => es.Exam)
                    .ThenInclude(e => e.Class)
                .Include(es => es.Exam)
                    .ThenInclude(e => e.Section)
                .Include(es => es.ExamMarks)
                .Where(es => es.Id == examSubjectId && !es.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<ExamSubjectModel>(examSubject);
        }

        public async Task<bool> BulkCreateExamSubjectsAsync(int examId, List<ExamSubject> subjects)
        {
            if (!subjects.Any()) return false;

            // Set ExamId for all subjects
            foreach (var subject in subjects)
            {
                subject.ExamId = examId;
                subject.CreatedAt = DateTime.Now;
                subject.CreatedAtUtc = DateTime.UtcNow;
            }

            await DbContext.Set<ExamSubject>().AddRangeAsync(subjects);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteExamSubjectsAsync(int examId)
        {
            var examSubjects = await DbContext.Set<ExamSubject>()
                .Where(es => es.ExamId == examId && !es.IsDeleted)
                .ToListAsync();

            if (!examSubjects.Any()) return false;

            DbContext.Set<ExamSubject>().RemoveRange(examSubjects);
            await DbContext.SaveChangesAsync();

            return true;
        }
    }
}


