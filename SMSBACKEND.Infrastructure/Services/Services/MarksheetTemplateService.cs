using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class MarksheetTemplateService : BaseService<MarksheetTemplateModel, MarksheetTemplate, int>, IMarksheetTemplateService
    {
        private readonly IMarksheetTemplateRepository _marksheetTemplateRepository;

        public MarksheetTemplateService(
            IMapper mapper,
            IMarksheetTemplateRepository marksheetTemplateRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
        ,
            ILogger<MarksheetTemplateService> logger
            ) : base(mapper, marksheetTemplateRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _marksheetTemplateRepository = marksheetTemplateRepository;
        }

        public async Task<BaseModel> GetDefaultTemplateAsync()
        {
            var template = await _marksheetTemplateRepository.GetDefaultTemplateAsync();
            if (template == null)
            {
                return new BaseModel { Success = false, Message = "No default template found" };
            }
            return new BaseModel { Success = true, Data = template };
        }

        public async Task<BaseModel> SetDefaultTemplateAsync(int templateId)
        {
            var result = await _marksheetTemplateRepository.SetDefaultTemplateAsync(templateId);
            if (!result)
            {
                return new BaseModel { Success = false, Message = "Failed to set default template" };
            }
            return new BaseModel { Success = true, Message = "Default template set successfully" };
        }

        public async Task<BaseModel> GetActiveTemplatesAsync()
        {
            var templates = await _marksheetTemplateRepository.GetActiveTemplatesAsync();
            return new BaseModel { Success = true, Data = templates, Total = templates.Count };
        }
    }
}



