
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Repository;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{

    public class FeeGroupFeeTypeService : BaseService<FeeGroupFeeTypeModel, FeeGroupFeeType, int>, IFeeGroupFeeTypeService
    {
        private readonly IFeeGroupFeeTypeRepository _feegroupfeetypeRepository;
        
        public FeeGroupFeeTypeService(
            IMapper mapper, 
            IFeeGroupFeeTypeRepository feegroupfeetypeRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService
            ) : base(mapper, feegroupfeetypeRepository, unitOfWork, sseService)
        {
            _feegroupfeetypeRepository = feegroupfeetypeRepository;
        }
        public async Task<BaseModel> GetFeeGroupFeeType(BaseRequest model)
        {
            var filterParams = new FilterParams
            {
                PaginationParam = model.PaginationParam
            };
            var result = await _feegroupfeetypeRepository.GetFeeGroupFeeType(filterParams);
            return new BaseModel { Success = true, Data = result.feeGroupFeeTypeModels, Total = result.feeGroupFeeTypeModels.Count, LastId = result.lastId };
        }
        public async Task<BaseModel> SaveFeeGroupFeeType(FeeGroupFeeTypeRequest model)
        {
            var result = await _feegroupfeetypeRepository.SaveFeeGroupFeeType(model);
            return new BaseModel { Success = true, Data = result, Message = "" };
        }
    }
}

