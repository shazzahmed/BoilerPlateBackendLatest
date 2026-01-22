
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

    public class FeeGroupService : BaseService<FeeGroupModel, FeeGroup, int>, IFeeGroupService
    {
        private readonly IFeeGroupRepository _feegroupRepository;
        
        public FeeGroupService(
            IMapper mapper, 
            IFeeGroupRepository feegroupRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<FeeGroupService> logger
            ) : base(mapper, feegroupRepository, unitOfWork,sseService, cacheProvider, logger)
        {
            _feegroupRepository = feegroupRepository;
        }
        // Add your methods here
    }
}

