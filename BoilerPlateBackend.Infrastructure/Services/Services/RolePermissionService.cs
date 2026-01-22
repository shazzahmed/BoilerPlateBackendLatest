
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using IdentityModel;
using Common.Utilities.StaticClasses;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{

    public class RolePermissionService : BaseService<RolePermissionModel, RolePermission, int>, IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolepermissionRepository;
        private readonly ICacheProvider _cacheProvider;

        public RolePermissionService(
            IMapper mapper,
            IRolePermissionRepository rolepermissionRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<RolePermissionService> logger
            ) : base(mapper, rolepermissionRepository, unitOfWork, sseService)
        {
            _rolepermissionRepository = rolepermissionRepository;
            _cacheProvider = cacheProvider;
        }

        public async Task SyncRolePermissionsAsync(string roleId, List<PermissionAssignmentRequest> assignments)
        {
           await _rolepermissionRepository.SyncRolePermissionsAsync(roleId, assignments);
            // Remove cache for current entity
            if (_cacheProvider != null)
                await _cacheProvider.RemoveByTagAsync(typeof(RolePermission).Name);

            // Remove cache for dependent entities
            if (EntityDependencyMap.Dependencies.TryGetValue(typeof(RolePermission).Name, out var dependents))
            {
                foreach (var dependent in dependents)
                    await _cacheProvider.RemoveByTagAsync(dependent);
            }
        }
    }
}

