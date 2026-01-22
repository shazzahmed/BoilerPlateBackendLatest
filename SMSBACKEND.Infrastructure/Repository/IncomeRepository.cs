
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class IncomeRepository : BaseRepository<Income, int>, IIncomeRepository
    {
        public IncomeRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
