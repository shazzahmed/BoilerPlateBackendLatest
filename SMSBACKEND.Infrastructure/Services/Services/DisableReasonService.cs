
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

    public class DisableReasonService : BaseService<DisableReasonModel, DisableReason, int>, IDisableReasonService
    {
        private readonly IDisableReasonRepository _disablereasonRepository;
        
        public DisableReasonService(
            IMapper mapper, 
            IDisableReasonRepository disablereasonRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<DisableReasonService> logger
            ) : base(mapper, disablereasonRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _disablereasonRepository = disablereasonRepository;
        }
        // Add your methods here
    }
}

