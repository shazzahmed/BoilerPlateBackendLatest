
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

    public class DepartmentService : BaseService<DepartmentModel, Department, int>, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        
        public DepartmentService(
            IMapper mapper, 
            IDepartmentRepository departmentRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<DepartmentService> logger
            ) : base(mapper, departmentRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _departmentRepository = departmentRepository;
        }
        // Add your methods here
    }
}

