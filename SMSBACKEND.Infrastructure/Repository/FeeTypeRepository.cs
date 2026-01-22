
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class FeeTypeRepository : BaseRepository<FeeType, int>, IFeeTypeRepository
    {
        public FeeTypeRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
