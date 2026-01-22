
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class DisableReasonRepository : BaseRepository<DisableReason, int>, IDisableReasonRepository
    {
        public DisableReasonRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
