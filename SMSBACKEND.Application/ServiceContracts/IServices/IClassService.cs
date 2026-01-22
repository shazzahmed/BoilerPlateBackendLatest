
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IClassService : IBaseService<ClassModel, Class, int>
    {
        Task<ClassModel> CreateClassSection(ClassModel model);
        Task UpdateClassSection(ClassModel model);
        Task<BaseModel> GetClassTeacher();
        Task<BaseModel> CreateClassTeacher(ClassTeacherRequest model);
        Task<BaseModel> UpdateClassTeacher(ClassTeacherRequest model);
        Task<BaseModel> DeleteClassTeacher(int ClassSectionId);
        Task<BaseModel> GetClassSectionById(int ClassSectionId);
    }
}
