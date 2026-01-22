
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class ExpenseRepository : BaseRepository<Expense, int>, IExpenseRepository
    {
        public ExpenseRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
