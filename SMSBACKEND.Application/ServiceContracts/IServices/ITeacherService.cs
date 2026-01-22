
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface ITeacherService : IBaseService<StaffModel, Staff, int>
    {
        Task<BaseModel> GetTeacherSubjects(BaseRequest model);
        Task<BaseModel> AssignTeacherSubjects(TeacherSubjectsRequest model);
        Task<BaseModel> GetClassTeachers(BaseRequest model);
        Task<BaseModel> AssignClassTeacher(ClassTeacherRequest model);
    }
}
