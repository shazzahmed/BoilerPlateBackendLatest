
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class StudentCategoryRepository : BaseRepository<StudentCategory, int>, IStudentCategoryRepository
    {
        public StudentCategoryRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
