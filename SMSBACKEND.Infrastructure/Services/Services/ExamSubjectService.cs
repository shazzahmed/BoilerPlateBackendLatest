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
    public class ExamSubjectService : BaseService<ExamSubjectModel, ExamSubject, int>, IExamSubjectService
    {
        private readonly IExamSubjectRepository _examSubjectRepository;

        public ExamSubjectService(
            IMapper mapper,
            IExamSubjectRepository examSubjectRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
        ,
            ILogger<ExamSubjectService> logger
            ) : base(mapper, examSubjectRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _examSubjectRepository = examSubjectRepository;
        }

        public async Task<BaseModel> GetSubjectsByExamAsync(int examId)
        {
            var subjects = await _examSubjectRepository.GetSubjectsByExamAsync(examId);
            return new BaseModel { Success = true, Data = subjects, Total = subjects.Count };
        }

        public async Task<BaseModel> GetExamSubjectWithDetailsAsync(int examSubjectId)
        {
            var subject = await _examSubjectRepository.GetExamSubjectWithDetailsAsync(examSubjectId);
            if (subject == null)
            {
                return new BaseModel { Success = false, Message = "Exam subject not found" };
            }
            return new BaseModel { Success = true, Data = subject };
        }

        public async Task<BaseModel> BulkCreateExamSubjectsAsync(BulkExamSubjectCreateRequest request)
        {
            var subjects = mapper.Map<List<ExamSubject>>(request.Subjects);
            var result = await _examSubjectRepository.BulkCreateExamSubjectsAsync(request.ExamId, subjects);
            
            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to create exam subjects" };
            }
            
            return new BaseModel { Success = true, Message = "Exam subjects created successfully" };
        }

        public async Task<BaseModel> DeleteExamSubjectsAsync(int examId)
        {
            var result = await _examSubjectRepository.DeleteExamSubjectsAsync(examId);
            
            if (!result)
            {
                return new BaseModel { Success = false, Message = "No exam subjects found to delete" };
            }
            
            return new BaseModel { Success = true, Message = "Exam subjects deleted successfully" };
        }
    }
}



