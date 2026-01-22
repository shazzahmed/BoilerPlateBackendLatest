
using AutoMapper;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using Domain.Entities;

namespace Infrastructure.Services.Services
{

    public class TeacherService : BaseService<StaffModel, Staff, int>, ITeacherService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ITimetableService _timetableService;
        
        public TeacherService(
            IMapper mapper,
            IStaffRepository staffRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ITimetableService timetableService
            ) : base(mapper, staffRepository, unitOfWork, sseService)
        {
            _staffRepository = staffRepository;
            _timetableService = timetableService;
        }
        public async Task<BaseModel> GetTeacherSubjects(BaseRequest model)
        {
            var filterParams = new FilterParams
            {
                PaginationParam = model.PaginationParam
            };
            var (result, lastId) = await _staffRepository.GetTeacherSubjects(filterParams);
            return new BaseModel { Success = true, Data = result, Total = result.Count, LastId = lastId };
        }
        public async Task<BaseModel> AssignTeacherSubjects(TeacherSubjectsRequest model)
        {
            var result = await _staffRepository.AssignTeacherSubjects(model);
            return new BaseModel { Success = true, Data = result, Message = "" };
        }

        public async Task<BaseModel> GetClassTeachers(BaseRequest model)
        {
            var filterParams = new FilterParams
            {
                PaginationParam = model.PaginationParam
            };
            var (result, lastId) = await _staffRepository.GetClassTeachers(filterParams);
            return new BaseModel { Success = true, Data = result, Total = result.Count, LastId = lastId };
        }

        public async Task<BaseModel> AssignClassTeacher(ClassTeacherRequest model)
        {
            var result = await _staffRepository.AssignClassTeacher(model);
            return new BaseModel { Success = true, Data = result, Message = "Class teacher assigned successfully" };
        }
    }
}

