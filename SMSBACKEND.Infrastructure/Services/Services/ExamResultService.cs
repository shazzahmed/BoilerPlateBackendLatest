using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Application.ServiceContracts;
using Common.DTO.Response;
using Common.DTO.Request;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class ExamResultService : BaseService<ExamResultModel, ExamResult, int>, IExamResultService
    {
        private readonly IExamResultRepository _examResultRepository;

        public ExamResultService(
            IMapper mapper,
            IExamResultRepository examResultRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<ExamResultService> logger
            ) : base(mapper, examResultRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _examResultRepository = examResultRepository;
        }

        public async Task<BaseModel> GetResultsByExamAsync(int examId)
        {
            var results = await _examResultRepository.GetResultsByExamAsync(examId);
            return new BaseModel { Success = true, Data = results, Total = results.Count };
        }

        public async Task<BaseModel> GetStudentExamResultAsync(int examId, int studentId)
        {
            var result = await _examResultRepository.GetStudentExamResultAsync(examId, studentId);
            return new BaseModel { Success = true, Data = result };
        }

        public async Task<BaseModel> GenerateExamResultsAsync(GenerateExamResultRequest request)
        {
            var result = await _examResultRepository.GenerateExamResultsAsync(request.ExamId, request.StudentId);
            
            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to generate exam results" };
            }
            
            return new BaseModel { Success = true, Message = "Exam results generated successfully" };
        }

        public async Task<BaseModel> PublishResultsAsync(PublishExamResultRequest request)
        {
            var result = await _examResultRepository.PublishResultsAsync(request.ExamId, request.StudentIds);
            
            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to publish results" };
            }
            
            return new BaseModel { Success = true, Message = "Results published successfully" };
        }

        public async Task<BaseModel> GetTopRankersAsync(int examId, int topCount = 10)
        {
            var rankers = await _examResultRepository.GetTopRankersAsync(examId, topCount);
            return new BaseModel { Success = true, Data = rankers, Total = rankers.Count };
        }

        public async Task<BaseModel> GenerateReportCardAsync(StudentReportCardRequest request)
        {
            // TODO: Implement PDF generation for report card
            return new BaseModel { Success = false, Message = "Report card generation not yet implemented" };
        }

        public async Task<BaseModel> GenerateBulkReportCardsAsync(BulkReportCardRequest request)
        {
            // TODO: Implement bulk PDF generation for report cards
            return new BaseModel { Success = false, Message = "Bulk report card generation not yet implemented" };
        }
    }
}



