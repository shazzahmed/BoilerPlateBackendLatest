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
    public class ExamTimetableService : BaseService<ExamTimetableModel, ExamTimetable, int>, IExamTimetableService
    {
        private readonly IExamTimetableRepository _examTimetableRepository;

        public ExamTimetableService(
            IMapper mapper,
            IExamTimetableRepository examTimetableRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
        ,
            ILogger<ExamTimetableService> logger
            ) : base(mapper, examTimetableRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _examTimetableRepository = examTimetableRepository;
        }

        public async Task<BaseModel> GetTimetableByExamAsync(int examId)
        {
            var timetables = await _examTimetableRepository.GetTimetableByExamAsync(examId);
            return new BaseModel { Success = true, Data = timetables, Total = timetables.Count };
        }

        public async Task<BaseModel> GetTimetableByExamSubjectAsync(int examSubjectId)
        {
            var timetable = await _examTimetableRepository.GetTimetableByExamSubjectAsync(examSubjectId);
            return new BaseModel { Success = true, Data = timetable };
        }

        public async Task<BaseModel> BulkCreateTimetableAsync(BulkExamTimetableCreateRequest request)
        {
            var timetables = mapper.Map<List<ExamTimetable>>(request.Timetables);
            var result = await _examTimetableRepository.BulkCreateTimetableAsync(timetables);
            
            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to create timetables" };
            }
            
            return new BaseModel { Success = true, Message = "Timetables created successfully" };
        }

        public async Task<BaseModel> GetTimetableByDateAsync(DateTime date)
        {
            var timetables = await _examTimetableRepository.GetTimetableByDateAsync(date);
            return new BaseModel { Success = true, Data = timetables, Total = timetables.Count };
        }

        public async Task<BaseModel> GetTimetableByInvigilatorAsync(int invigilatorId)
        {
            var timetables = await _examTimetableRepository.GetTimetableByInvigilatorAsync(invigilatorId);
            return new BaseModel { Success = true, Data = timetables, Total = timetables.Count };
        }
    }
}



