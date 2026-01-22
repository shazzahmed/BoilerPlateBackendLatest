
using Common.DTO.Request;
using Common.DTO.Response;
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.RepositoriesContracts
{
    public interface IStudentRepository : IBaseRepository<Student, int>
    {
        Task<int> CreateStudent(StudentCreateRequest model);
        Task<StudentModel> UpdateStudent(StudentCreateRequest model);
        Task<bool> SoftDeleteStudent(int id, string deletedBy);
        Task<bool> RestoreStudent(int id, string restoredBy);
        Task<(List<StudentModel> Students, int Count, int LastId, string lastAdmNo)> GetAllStudent(FilterParams filters);
        Task<bool> IsAdmissionNoUnique(string admissionNo, int? excludeId = null);
        Task<bool> IsRollNoUniqueInClass(string rollNo, int classId, int sectionId, int? excludeId = null);
        Task<string> GenerateNextAdmissionNumberAsync(int tenantId);
        Task<string> PeekNextAdmissionNumberAsync(int tenantId);
    }
}
