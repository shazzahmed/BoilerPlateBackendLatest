using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class GradeService : BaseService<GradeModel, Grade, int>, IGradeService
    {
        private readonly IGradeRepository _gradeRepository;

        public GradeService(
            IMapper mapper,
            IGradeRepository gradeRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
        ,
            ILogger<GradeService> logger
            ) : base(mapper, gradeRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _gradeRepository = gradeRepository;
        }

        public async Task<BaseModel> GetGradesByScaleAsync(int gradeScaleId)
        {
            var grades = await _gradeRepository.GetGradesByScaleAsync(gradeScaleId);
            return new BaseModel { Success = true, Data = grades, Total = grades.Count };
        }

        public async Task<BaseModel> GetGradeByPercentageAsync(int gradeScaleId, decimal percentage)
        {
            var grade = await _gradeRepository.GetGradeByPercentageAsync(gradeScaleId, percentage);
            return new BaseModel { Success = true, Data = grade };
        }
    }
}



