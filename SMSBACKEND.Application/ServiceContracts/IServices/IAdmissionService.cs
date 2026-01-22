
using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface IAdmissionService : IBaseService<AdmissionModel, Admission, int>
    {
        Task<BaseModel> CreateAdmissionAsync(AdmissionModel model);
        Task<bool> CanCreateStudent(int admissionId);
        Task<BaseModel> MarkStudentCreated(int admissionId, int studentId, string AdmissionNo);
        Task<string> GetNextAdmissionNumberAsync();
        Task<BaseModel> GetAdmissionWithStudent(int admissionId);
        Task<BaseModel> GetAdmissionWithWorkflowData(int admissionId);
    }
}
