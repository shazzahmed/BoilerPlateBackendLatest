
using Common.DTO.Request;
using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IClassRepository : IBaseRepository<Class, int>
    {
        Task CreateClassSection(ClassModel model, List<int> SelectedSectionIds);
        Task UpdateClassSection(ClassModel model, List<int> SelectedSectionIds);
        Task CreateSubjectClass(ClassModel model, List<int> SelectedSubjectIds);
        Task UpdateSubjectClass(ClassModel model, List<int> SelectedSubjectIds);
        Task<(List<ClassTeacherModel> classTeachers, int lastId)> GetClassTeacher();
        Task<int> CreateClassTeacher(ClassTeacherRequest model);
        Task<int> UpdateClassTeacher(ClassTeacherRequest model);
        Task<BaseModel> DeleteClassTeacher(int ClassSectionId);
        Task<BaseModel> GetClassSectionById(int ClassSectionId);
    }
}
