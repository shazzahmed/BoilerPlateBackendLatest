
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

    public class SchoolHouseService : BaseService<SchoolHouseModel, SchoolHouse, int>, ISchoolHouseService
    {
        private readonly ISchoolHouseRepository _schoolhouseRepository;
        
        public SchoolHouseService(
            IMapper mapper, 
            ISchoolHouseRepository schoolhouseRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<SchoolHouseService> logger
            ) : base(mapper, schoolhouseRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _schoolhouseRepository = schoolhouseRepository;
        }
        // Add your methods here
    }
}

