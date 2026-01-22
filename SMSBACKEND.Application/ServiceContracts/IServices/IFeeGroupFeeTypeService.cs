
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IFeeGroupFeeTypeService : IBaseService<FeeGroupFeeTypeModel, FeeGroupFeeType, int>
    {
        Task<BaseModel> GetFeeGroupFeeType(BaseRequest model);
        Task<BaseModel> SaveFeeGroupFeeType(FeeGroupFeeTypeRequest model);
    }
}
