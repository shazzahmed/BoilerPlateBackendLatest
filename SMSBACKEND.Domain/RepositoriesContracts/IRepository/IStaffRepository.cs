
using Common.DTO.Request;
using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IStaffRepository : IBaseRepository<Staff, int>
    {
        Task<StaffModel> CreateStaff(StaffCreateReq model);
        Task<StaffModel> UpdateStaff(StaffCreateReq model);
        Task<(List<StaffModel> Staffs, int Count, int lastId)> GetAllStaff(FilterParams filters);
        Task<(List<TeacherSubjectsModel> teacherSubjects, int lastId)> GetTeacherSubjects(FilterParams filters);
        Task<int> AssignTeacherSubjects(TeacherSubjectsRequest request);
        Task<(List<ClassTeacherModel> classTeachers, int lastId)> GetClassTeachers(FilterParams filters);
        Task<int> AssignClassTeacher(ClassTeacherRequest request);
    }
}
