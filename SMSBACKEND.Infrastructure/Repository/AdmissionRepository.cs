
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class AdmissionRepository : BaseRepository<Admission, int>, IAdmissionRepository
    {
        public AdmissionRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
