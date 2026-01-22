
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{

    public class FeeDiscountService : BaseService<FeeDiscountModel, FeeDiscount, int>, IFeeDiscountService
    {
        private readonly IFeeDiscountRepository _feediscountRepository;
        
        public FeeDiscountService(
            IMapper mapper, 
            IFeeDiscountRepository feediscountRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<FeeDiscountService> logger
            ) : base(mapper, feediscountRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _feediscountRepository = feediscountRepository;
        }
        // Add your methods here
    }
}

