
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

    public class IncomeHeadService : BaseService<IncomeHeadModel, IncomeHead, int>, IIncomeHeadService
    {
        private readonly IIncomeHeadRepository _incomeheadRepository;
        
        public IncomeHeadService(
            IMapper mapper, 
            IIncomeHeadRepository incomeheadRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<IncomeHeadService> logger
            ) : base(mapper, incomeheadRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _incomeheadRepository = incomeheadRepository;
        }
        // Add your methods here
    }
}

