
using Common.DTO.Request;
using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IFeeGroupFeeTypeRepository : IBaseRepository<FeeGroupFeeType, int>
    {
        Task<(List<FeeGroupWithTypesModel> feeGroupFeeTypeModels, int lastId)> GetFeeGroupFeeType(FilterParams filters);
        Task<int> SaveFeeGroupFeeType(FeeGroupFeeTypeRequest model);
    }
}
