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
    public class GradeScaleService : BaseService<GradeScaleModel, GradeScale, int>, IGradeScaleService
    {
        private readonly IGradeScaleRepository _gradeScaleRepository;
        private readonly IGradeRepository _gradeRepository;

        public GradeScaleService(
            IMapper mapper,
            IGradeScaleRepository gradeScaleRepository,
            IGradeRepository gradeRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
        ,
            ILogger<GradeScaleService> logger
            ) : base(mapper, gradeScaleRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _gradeScaleRepository = gradeScaleRepository;
            _gradeRepository = gradeRepository;
        }

        public async Task<BaseModel> GetDefaultGradeScaleAsync()
        {
            var gradeScale = await _gradeScaleRepository.GetDefaultGradeScaleAsync();
            if (gradeScale == null)
            {
                return new BaseModel { Success = false, Message = "No default grade scale found" };
            }
            return new BaseModel { Success = true, Data = gradeScale };
        }

        public async Task<BaseModel> GetGradeScaleWithGradesAsync(int gradeScaleId)
        {
            var gradeScale = await _gradeScaleRepository.GetGradeScaleWithGradesAsync(gradeScaleId);
            if (gradeScale == null)
            {
                return new BaseModel { Success = false, Message = "Grade scale not found" };
            }
            return new BaseModel { Success = true, Data = gradeScale };
        }

        public async Task<BaseModel> SetDefaultGradeScaleAsync(int gradeScaleId)
        {
            var result = await _gradeScaleRepository.SetDefaultGradeScaleAsync(gradeScaleId);
            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to set default grade scale" };
            }
            return new BaseModel { Success = true, Message = "Default grade scale set successfully" };
        }

        public async Task<BaseModel> CreateGradeScaleWithGradesAsync(GradeScaleCreateRequest request)
        {
            // Create grade scale
            var gradeScaleModel = new GradeScaleModel
            {
                GradeScaleName = request.GradeScaleName,
                Description = request.Description,
                IsActive = request.IsActive,
                IsDefault = request.IsDefault
            };

            var createdScale = await Add(gradeScaleModel);

            // Create grades
            if (request.Grades.Any())
            {
                var grades = mapper.Map<List<Grade>>(request.Grades);
                await _gradeRepository.BulkCreateGradesAsync(createdScale.Id, grades);
            }

            return new BaseModel { Success = true, Data = createdScale, Message = "Grade scale created successfully" };
        }
    }
}



