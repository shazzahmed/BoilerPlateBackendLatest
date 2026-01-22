
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

    public class ExpenseService : BaseService<ExpenseModel, Expense, int>, IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        
        public ExpenseService(
            IMapper mapper, 
            IExpenseRepository expenseRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService
            ) : base(mapper, expenseRepository, unitOfWork, sseService)
        {
            _expenseRepository = expenseRepository;
        }
        // Add your methods here
    }
}

