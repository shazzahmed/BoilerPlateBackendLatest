
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class IncomeHeadRepository : BaseRepository<IncomeHead, int>, IIncomeHeadRepository
    {
        public IncomeHeadRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
