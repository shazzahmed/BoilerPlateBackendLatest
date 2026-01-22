
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

    public class StudentCategoryService : BaseService<StudentCategoryModel, StudentCategory, int>, IStudentCategoryService
    {
        private readonly IStudentCategoryRepository _studentcategoryRepository;
        
        public StudentCategoryService(
            IMapper mapper, 
            IStudentCategoryRepository studentcategoryRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<StudentCategoryService> logger
            ) : base(mapper, studentcategoryRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _studentcategoryRepository = studentcategoryRepository;
        }
        // Add your methods here
    }
}

