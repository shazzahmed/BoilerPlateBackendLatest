
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class PermissionRepository : BaseRepository<Permission, int>, IPermissionRepository
    {
        public PermissionRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
