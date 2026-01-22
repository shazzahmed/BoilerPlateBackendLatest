
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class SectionRepository : BaseRepository<Section, int>, ISectionRepository
    {
        public SectionRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
