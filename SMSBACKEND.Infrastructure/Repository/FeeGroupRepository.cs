
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class FeeGroupRepository : BaseRepository<FeeGroup, int>, IFeeGroupRepository
    {
        public FeeGroupRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
