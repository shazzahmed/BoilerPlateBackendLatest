using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class EntryTestRepository : BaseRepository<EntryTest, int>, IEntryTestRepository
    {
        public EntryTestRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}