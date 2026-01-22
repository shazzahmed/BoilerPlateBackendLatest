using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class SectionService : BaseService<SectionModel, Section, int>, ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        
        public SectionService(
            IMapper mapper, 
            ISectionRepository sectionRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<SectionService> logger
            ) : base(mapper, sectionRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _sectionRepository = sectionRepository;
        }
        // Add your methods here
    }
}
