
using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface IExpenseService : IBaseService<ExpenseModel, Expense, int>
    {
        // Define your methods here
    }
}
