
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class MenuRepository : BaseRepository<Module, int>, IMenuRepository
    {
        public MenuRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
