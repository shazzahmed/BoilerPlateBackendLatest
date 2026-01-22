
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class ExpenseHeadRepository : BaseRepository<ExpenseHead, int>, IExpenseHeadRepository
    {
        public ExpenseHeadRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
