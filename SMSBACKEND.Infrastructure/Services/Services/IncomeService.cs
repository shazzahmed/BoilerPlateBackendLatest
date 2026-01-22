
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

    public class IncomeService : BaseService<IncomeModel, Income, int>, IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        
        public IncomeService(
            IMapper mapper, 
            IIncomeRepository incomeRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService
            ) : base(mapper, incomeRepository, unitOfWork, sseService)
        {
            _incomeRepository = incomeRepository;
        }
        // Add your methods here
    }
}

