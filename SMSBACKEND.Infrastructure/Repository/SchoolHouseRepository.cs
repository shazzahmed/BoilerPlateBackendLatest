
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class SchoolHouseRepository : BaseRepository<SchoolHouse, int>, ISchoolHouseRepository
    {
        public SchoolHouseRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
