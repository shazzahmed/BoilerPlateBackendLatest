using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Response;
using Microsoft.EntityFrameworkCore;
using static Common.Utilities.Enums;
using Infrastructure.Services.Communication;
using Application.ServiceContracts;

namespace Infrastructure.Repository
{
    public class ExamRepository : BaseRepository<Exam, int>, IExamRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;

        public ExamRepository(
            IMapper mapper,
            ISqlServerDbContext context,
            ICacheProvider cacheProvider,
            SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }

        public async Task<List<ExamModel>> GetExamsByClassAsync(int classId, int? sectionId = null)
        {
            var query = DbContext.Set<Exam>()
                .Include(e => e.Class)
                .Include(e => e.Section)
                .Where(e => e.ClassId == classId && !e.IsDeleted);

            if (sectionId.HasValue)
            {
                query = query.Where(e => e.SectionId == sectionId.Value);
            }

            var exams = await query
                .OrderByDescending(e => e.StartDate)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamModel>>(exams);
        }

        public async Task<List<ExamModel>> GetExamsByAcademicYearAsync(string academicYear)
        {
            var exams = await DbContext.Set<Exam>()
                .Include(e => e.Class)
                .Include(e => e.Section)
                .Where(e => e.AcademicYear == academicYear && !e.IsDeleted)
                .OrderByDescending(e => e.StartDate)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamModel>>(exams);
        }

        public async Task<ExamModel> GetExamWithDetailsAsync(int examId)
        {
            var exam = await DbContext.Set<Exam>()
                .Include(e => e.Class)
                .Include(e => e.Section)
                .Include(e => e.ExamSubjects)
                    .ThenInclude(es => es.Subject)
                .Include(e => e.ExamResults)
                .Where(e => e.Id == examId && !e.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<ExamModel>(exam);
        }

        public async Task<bool> PublishExamAsync(int examId, bool isPublished)
        {
            var exam = await DbContext.Set<Exam>()
                .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted);

            if (exam == null) return false;

            exam.IsPublished = isPublished;
            exam.PublishedDate = isPublished ? DateTime.UtcNow : null;
            exam.Status = isPublished ? ExamStatus.Published : ExamStatus.Completed;

            DbContext.Set<Exam>().Update(exam);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<ExamModel>> GetExamsByStatusAsync(ExamStatus status)
        {
            var exams = await DbContext.Set<Exam>()
                .Include(e => e.Class)
                .Include(e => e.Section)
                .Where(e => e.Status == status && !e.IsDeleted)
                .OrderByDescending(e => e.StartDate)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamModel>>(exams);
        }

        /// <summary>
        /// Override Get() to include navigation properties by default
        /// This ensures Class and Section are always loaded for ClassName and SectionName computed properties
        /// </summary>
        public override async Task<List<Exam>> GetAsync(
            System.Linq.Expressions.Expression<Func<Exam, bool>> where = null,
            Func<IQueryable<Exam>, IOrderedQueryable<Exam>> orderBy = null,
            Func<IQueryable<Exam>, IQueryable<Exam>> includeProperties = null)
        {
            var query = DbContext.Set<Exam>()
                .Include(e => e.Class)      // Include Class navigation for ClassName
                .Include(e => e.Section)    // Include Section navigation for SectionName
                .AsQueryable();

            if (where != null)
            {
                query = query.Where(where);
            }

            // Allow additional includes if provided
            if (includeProperties != null)
            {
                query = includeProperties(query);
            }

            if (orderBy != null)
            {
                return await orderBy(query).AsNoTracking().ToListAsync();
            }

            return await query.AsNoTracking().ToListAsync();
        }
    }
}


