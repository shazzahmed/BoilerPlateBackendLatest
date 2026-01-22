
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

    public class MenuService : BaseService<ModuleModel, Module, int>, IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(
            IMapper mapper,
            IMenuRepository menuRepository,
            IUnitOfWork unitOfWork,
            SseService sseService) : base(mapper, menuRepository, unitOfWork, sseService)
        {
            _menuRepository = menuRepository;
        }
    }
}

