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
    public class ExamResultRepository : BaseRepository<ExamResult, int>, IExamResultRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;
        private readonly IGradeRepository _gradeRepository;

        public ExamResultRepository(
            IMapper mapper,
            ISqlServerDbContext context,
            ICacheProvider cacheProvider,
            SseService sseService,
            IGradeRepository gradeRepository) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
            _gradeRepository = gradeRepository;
        }

        public async Task<List<ExamResultModel>> GetResultsByExamAsync(int examId)
        {
            var results = await DbContext.Set<ExamResult>()
                .Include(r => r.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(r => r.Exam)
                .Include(r => r.OverallGrade)
                .Where(r => r.ExamId == examId && !r.IsDeleted)
                .OrderBy(r => r.Rank ?? int.MaxValue)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamResultModel>>(results);
        }

        public async Task<ExamResultModel> GetStudentExamResultAsync(int examId, int studentId)
        {
            var result = await DbContext.Set<ExamResult>()
                .Include(r => r.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(r => r.Exam)
                .Include(r => r.OverallGrade)
                .Where(r => r.ExamId == examId && r.StudentId == studentId && !r.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<ExamResultModel>(result);
        }

        public async Task<bool> GenerateExamResultsAsync(int examId, int? studentId = null)
        {
            // Get exam details
            var exam = await DbContext.Set<Exam>()
                .Include(e => e.ExamSubjects)
                .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted);

            if (exam == null) return false;

            // Get all marks for this exam
            var marksQuery = DbContext.Set<ExamMarks>()
                .Include(m => m.ExamSubject)
                .Where(m => m.ExamSubject.ExamId == examId && !m.IsDeleted);

            if (studentId.HasValue)
            {
                marksQuery = marksQuery.Where(m => m.StudentId == studentId.Value);
            }

            var marks = await marksQuery.ToListAsync();

            // Group by student
            var studentMarks = marks.GroupBy(m => m.StudentId);

            // Get default grade scale
            var gradeScale = await DbContext.Set<GradeScale>()
                .Include(gs => gs.Grades)
                .FirstOrDefaultAsync(gs => gs.IsDefault && gs.IsActive && !gs.IsDeleted);

            if (gradeScale == null) return false;

            var results = new List<ExamResult>();

            foreach (var studentGroup in studentMarks)
            {
                var studentId_val = studentGroup.Key;
                var totalObtained = studentGroup.Sum(m => m.TotalMarks);
                var totalMax = studentGroup.Sum(m => m.ExamSubject.MaxMarks + m.ExamSubject.MaxPracticalMarks);
                var percentage = totalMax > 0 ? (totalObtained / totalMax) * 100 : 0;

                // Find grade
                var grade = gradeScale.Grades
                    .FirstOrDefault(g => percentage >= g.MinPercentage && percentage <= g.MaxPercentage);

                // Calculate GPA
                var gpa = grade?.GradePoint ?? 0;

                // Determine result
                var isPass = percentage >= exam.PassingMarks;
                var hasAbsent = studentGroup.Any(m => m.IsAbsent);

                var result = hasAbsent ? "Absent" : (isPass ? "Pass" : "Fail");

                // Check if result already exists
                var existingResult = await DbContext.Set<ExamResult>()
                    .FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId_val && !r.IsDeleted);

                if (existingResult != null)
                {
                    // Update existing
                    existingResult.TotalMarksObtained = totalObtained;
                    existingResult.TotalMaxMarks = totalMax;
                    existingResult.OverallGradeId = grade?.Id;
                    existingResult.GPA = gpa;
                    existingResult.Result = result;
                    existingResult.GeneratedDate = DateTime.UtcNow;
                    existingResult.UpdatedAt = DateTime.UtcNow;

                    DbContext.Set<ExamResult>().Update(existingResult);
                }
                else
                {
                    // Create new
                    var newResult = new ExamResult
                    {
                        ExamId = examId,
                        StudentId = studentId_val,
                        TotalMarksObtained = totalObtained,
                        TotalMaxMarks = totalMax,
                        OverallGradeId = grade?.Id,
                        GPA = gpa,
                        Result = result,
                        GeneratedDate = DateTime.UtcNow,
                        IsPublished = false,
                        CreatedAt = DateTime.Now,
                        CreatedAtUtc = DateTime.UtcNow
                    };

                    results.Add(newResult);
                }
            }

            if (results.Any())
            {
                await DbContext.Set<ExamResult>().AddRangeAsync(results);
            }

            await DbContext.SaveChangesAsync();

            // Calculate ranks
            await CalculateRanksAsync(examId);

            return true;
        }

        public async Task<bool> PublishResultsAsync(int examId, List<int> studentIds)
        {
            var query = DbContext.Set<ExamResult>()
                .Where(r => r.ExamId == examId && !r.IsDeleted);

            if (studentIds.Any())
            {
                query = query.Where(r => studentIds.Contains(r.StudentId));
            }

            var results = await query.ToListAsync();

            if (!results.Any()) return false;

            foreach (var result in results)
            {
                result.IsPublished = true;
                result.PublishedDate = DateTime.UtcNow;
            }

            DbContext.Set<ExamResult>().UpdateRange(results);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CalculateRanksAsync(int examId)
        {
            var exam = await DbContext.Set<Exam>()
                .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted);

            if (exam == null) return false;

            // Class-wide ranking
            var classResults = await DbContext.Set<ExamResult>()
                .Where(r => r.ExamId == examId && r.Result == "Pass" && !r.IsDeleted)
                .OrderByDescending(r => r.TotalMarksObtained)
                .ToListAsync();

            int classRank = 1;
            foreach (var result in classResults)
            {
                result.Rank = classRank++;
            }

            // Section-wide ranking (if section specified)
            if (exam.SectionId.HasValue)
            {
                var sectionResults = classResults;
                int sectionRank = 1;
                foreach (var result in sectionResults)
                {
                    result.SectionRank = sectionRank++;
                }
            }

            DbContext.Set<ExamResult>().UpdateRange(classResults);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<ExamResultModel>> GetTopRankersAsync(int examId, int topCount = 10)
        {
            var results = await DbContext.Set<ExamResult>()
                .Include(r => r.Student)
                    .ThenInclude(s => s.StudentInfo)
                .Include(r => r.OverallGrade)
                .Where(r => r.ExamId == examId && r.Result == "Pass" && !r.IsDeleted)
                .OrderBy(r => r.Rank)
                .Take(topCount)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ExamResultModel>>(results);
        }
    }
}


