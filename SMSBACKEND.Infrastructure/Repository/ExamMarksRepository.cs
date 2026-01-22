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
    public class ExamMarksRepository : BaseRepository<ExamMarks, int>, IExamMarksRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;

        public ExamMarksRepository(
            IMapper mapper,
            ISqlServerDbContext context,
            ICacheProvider cacheProvider,
            SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }

        public async Task<List<ExamMarksModel>> GetMarksByExamSubjectAsync(int examSubjectId)
        {
            var marks = await DbContext.Set<ExamMarks>()
                .Include(m => m.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(m => m.ExamSubject)
                    .ThenInclude(es => es.Subject)
                .Include(m => m.Grade)
                .Where(m => m.ExamSubjectId == examSubjectId && !m.IsDeleted)
                .OrderBy(m => m.Student.StudentInfo.FirstName)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamMarksModel>>(marks);
        }

        public async Task<List<ExamMarksModel>> GetMarksByStudentAsync(int studentId, int examId)
        {
            var marks = await DbContext.Set<ExamMarks>()
                .Include(m => m.ExamSubject)
                    .ThenInclude(es => es.Subject)
                .Include(m => m.ExamSubject)
                    .ThenInclude(es => es.Exam)
                .Include(m => m.Grade)
                .Where(m => m.StudentId == studentId && 
                           m.ExamSubject.ExamId == examId && 
                           !m.IsDeleted)
                .OrderBy(m => m.ExamSubject.Subject.Name)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamMarksModel>>(marks);
        }

        public async Task<ExamMarksModel> GetStudentExamMarksAsync(int examSubjectId, int studentId)
        {
            var marks = await DbContext.Set<ExamMarks>()
                .Include(m => m.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(m => m.ExamSubject)
                    .ThenInclude(es => es.Subject)
                .Include(m => m.Grade)
                .Where(m => m.ExamSubjectId == examSubjectId && 
                           m.StudentId == studentId && 
                           !m.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<ExamMarksModel>(marks);
        }

        public async Task<bool> BulkCreateMarksAsync(List<ExamMarks> marks)
        {
            if (!marks.Any()) return false;

            await DbContext.Set<ExamMarks>().AddRangeAsync(marks);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BulkUpdateMarksAsync(List<ExamMarks> marks)
        {
            if (!marks.Any()) return false;

            DbContext.Set<ExamMarks>().UpdateRange(marks);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifyMarksAsync(List<int> marksIds, string verifiedBy)
        {
            var marks = await DbContext.Set<ExamMarks>()
                .Where(m => marksIds.Contains(m.Id) && !m.IsDeleted)
                .ToListAsync();

            if (!marks.Any()) return false;

            foreach (var mark in marks)
            {
                mark.Status = MarksStatus.Verified;
                mark.VerifiedBy = verifiedBy;
                mark.VerifiedDate = DateTime.UtcNow;
            }

            DbContext.Set<ExamMarks>().UpdateRange(marks);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<ExamMarksModel>> GetMarksByExamAsync(int examId)
        {
            var marks = await DbContext.Set<ExamMarks>()
                .Include(m => m.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(m => m.ExamSubject)
                    .ThenInclude(es => es.Subject)
                .Include(m => m.Grade)
                .Where(m => m.ExamSubject.ExamId == examId && !m.IsDeleted)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamMarksModel>>(marks);
        }

        public async Task<int> GetTotalStudentsInExamAsync(int examId)
        {
            return await DbContext.Set<ExamMarks>()
                .Where(m => m.ExamSubject.ExamId == examId && !m.IsDeleted)
                .Select(m => m.StudentId)
                .Distinct()
                .CountAsync();
        }
    }
}


