using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Infrastructure.Repository;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static System.Collections.Specialized.BitVector32;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class ClassService : BaseService<ClassModel, Class, int>, IClassService
    {
        private readonly IClassRepository _classRepository;
        
        public ClassService(
            IMapper mapper, 
            IClassRepository classRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<ClassService> logger
            ) : base(mapper, classRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _classRepository = classRepository;
        }
        public async Task<ClassModel> CreateClassSection(ClassModel model)
        {
            var result = await Add(new ClassModel { Name = model.Name });
            model.Id = result.Id;
            await _classRepository.CreateClassSection(model, model.SelectedSectionIds);
            await _classRepository.CreateSubjectClass(model, model.SelectedSubjectIds);
            return result;
        }
        public async Task UpdateClassSection(ClassModel model)
        {
            model.IsDeleted = false;
            await _classRepository.UpdateClassSection(model, model.SelectedSectionIds);
            await _classRepository.UpdateSubjectClass(model, model.SelectedSubjectIds);
            await Update(model);
        }
        
        public async Task<BaseModel> GetClassTeacher()
        {
            var result = await _classRepository.GetClassTeacher();
            return new BaseModel { Success = true, Data = result.classTeachers, Total = result.classTeachers.Count, LastId = result.lastId };
        }
        public async Task<BaseModel> CreateClassTeacher(ClassTeacherRequest model)
        {
            var result = await _classRepository.CreateClassTeacher(model);
            return new BaseModel { Success = true, Data = result, Message = "" };
        } 
        public async Task<BaseModel> UpdateClassTeacher(ClassTeacherRequest model)
        {
            var result = await _classRepository.UpdateClassTeacher(model);
            return new BaseModel { Success = true, Data = result, Message = "" };
        }
        public async Task<BaseModel> DeleteClassTeacher(int ClassSectionId)
        {
            var result = await _classRepository.DeleteClassTeacher(ClassSectionId);
            return result;
        }
        public async Task<BaseModel> GetClassSectionById(int ClassSectionId)
        {
            var result = await _classRepository.GetClassSectionById(ClassSectionId);
            return result;
        }
    }
}
