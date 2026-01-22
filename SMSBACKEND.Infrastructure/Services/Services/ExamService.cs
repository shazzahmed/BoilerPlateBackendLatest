using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Application.ServiceContracts;
using Common.DTO.Response;
using Common.DTO.Request;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using static Common.Utilities.Enums;

namespace Infrastructure.Services.Services
{
    public class ExamService : BaseService<ExamModel, Exam, int>, IExamService
    {
        private readonly IExamRepository _examRepository;

        public ExamService(
            IMapper mapper,
            IExamRepository examRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<ExamService> logger
        ) : base(mapper, examRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _examRepository = examRepository;
        }

        public async Task<BaseModel> GetExamsByClassAsync(int classId, int? sectionId = null)
        {
            var exams = await _examRepository.GetExamsByClassAsync(classId, sectionId);
            return new BaseModel { Success = true, Data = exams, Total = exams.Count };
        }

        public async Task<BaseModel> GetExamsByAcademicYearAsync(string academicYear)
        {
            var exams = await _examRepository.GetExamsByAcademicYearAsync(academicYear);
            return new BaseModel { Success = true, Data = exams, Total = exams.Count };
        }

        public async Task<BaseModel> GetExamWithDetailsAsync(int examId)
        {
            var exam = await _examRepository.GetExamWithDetailsAsync(examId);
            if (exam == null)
            {
                return new BaseModel { Success = false, Message = "Exam not found" };
            }
            return new BaseModel { Success = true, Data = exam };
        }

        public async Task<BaseModel> PublishExamAsync(ExamPublishRequest request)
        {
            var result = await _examRepository.PublishExamAsync(request.ExamId, request.IsPublished);
            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to publish exam" };
            }
            return new BaseModel { Success = true, Message = request.IsPublished ? "Exam published successfully" : "Exam unpublished successfully" };
        }

        public async Task<BaseModel> GetExamsByStatusAsync(ExamStatus status)
        {
            var exams = await _examRepository.GetExamsByStatusAsync(status);
            return new BaseModel { Success = true, Data = exams, Total = exams.Count };
        }
    }
}


