
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace Application.ServiceContracts
{
    public interface IStudentService : IBaseService<StudentModel, Student, int>
    {
        Task<BaseModel> GetAll(StudentRequest request);
        Task<BaseModel> CreateStudent(StudentCreateRequest model);
        Task<BaseModel> UpdateStudent(StudentCreateRequest model);
        Task<BaseModel> DeleteStudent(int id);
        Task<BaseModel> RestoreStudent(int id);
       
        void BatchImportStudents(object data);
        Task<ParentValidationResponse> ValidateParentAsync(ParentValidationRequest request);
        Task<StudentValidationResponse> ValidateStudentAsync(StudentValidationRequest request);
        Task<string> GetNextAdmissionNumberAsync();
        
        // Bulk Import Methods
        Task<BaseModel> StartBulkImportAsync(Common.DTO.Request.BulkStudentImportRequest request);
        Task<BaseModel> GetImportStatusAsync(string jobId);
        Task<BaseModel> CancelImportJobAsync(string jobId);
        Task<List<Common.DTO.Response.ImportJobStatus>> GetAllImportJobsAsync();
        Task<byte[]> DownloadImportTemplateAsync();
        Task<BaseModel> ParseImportFileAsync(IFormFile file);
        Task StreamImportProgressAsync(string jobId, HttpResponse response);
        
        // ✅ Fee Package Management
        Task<BaseModel> GetStudentInitialFeePackages(int studentId);
        
        // ✅ Student Detail View
        Task<BaseModel> GetStudentById(int id);
        
        // ✅ Auto-fetch parent data
        Task<BaseModel> GetParentDataByCNIC(string cnic);
    }
}
