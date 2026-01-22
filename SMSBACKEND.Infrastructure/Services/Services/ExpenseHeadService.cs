
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

    public class ExpenseHeadService : BaseService<ExpenseHeadModel, ExpenseHead, int>, IExpenseHeadService
    {
        private readonly IExpenseHeadRepository _expenseheadRepository;
        
        public ExpenseHeadService(
            IMapper mapper, 
            IExpenseHeadRepository expenseheadRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<ExpenseHeadService> logger
            ) : base(mapper, expenseheadRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _expenseheadRepository = expenseheadRepository;
        }
        // Add your methods here
    }
}

