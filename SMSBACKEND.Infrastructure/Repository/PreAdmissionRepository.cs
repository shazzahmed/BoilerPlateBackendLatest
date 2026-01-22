using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class PreAdmissionRepository : BaseRepository<PreAdmission, int>, IPreAdmissionRepository
    {
        public PreAdmissionRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}