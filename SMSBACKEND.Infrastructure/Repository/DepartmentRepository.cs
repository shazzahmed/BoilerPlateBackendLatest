
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class DepartmentRepository : BaseRepository<Department, int>, IDepartmentRepository
    {
        public DepartmentRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
