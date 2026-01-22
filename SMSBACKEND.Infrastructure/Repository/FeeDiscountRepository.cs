
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class FeeDiscountRepository : BaseRepository<FeeDiscount, int>, IFeeDiscountRepository
    {
        public FeeDiscountRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
