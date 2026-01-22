
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

    public class PermissionService : BaseService<PermissionModel, Permission, int>, IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        
        public PermissionService(
            IMapper mapper, 
            IPermissionRepository permissionRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<PermissionService> logger
            ) : base(mapper, permissionRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _permissionRepository = permissionRepository;
        }
        // Add your methods here
    }
}

