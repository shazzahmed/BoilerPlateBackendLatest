
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class EnquiryRepository : BaseRepository<Enquiry, int>, IEnquiryRepository
    {
        public EnquiryRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
