
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

    public class FeeTypeService : BaseService<FeeTypeModel, FeeType, int>, IFeeTypeService
    {
        private readonly IFeeTypeRepository _feetypeRepository;
        
        public FeeTypeService(
            IMapper mapper, 
            IFeeTypeRepository feetypeRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<FeeTypeService> logger
            ) : base(mapper, feetypeRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _feetypeRepository = feetypeRepository;
        }
        // Add your methods here
    }
}

