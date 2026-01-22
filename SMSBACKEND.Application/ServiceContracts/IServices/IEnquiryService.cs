
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IEnquiryService : IBaseService<EnquiryModel, Enquiry, int>
    {
        // Bulk import methods
        Task<BaseModel> ValidateBulkImportAsync(BulkEnquiryImportRequest request);
        Task<BaseModel> ImportEnquiriesAsync(BulkEnquiryImportRequest request);
        Task<BaseModel> GetImportStatusAsync(string jobId);
        Task<BaseModel> GetImportHistoryAsync();
        Task<byte[]> DownloadTemplateAsync();
        
        // Workflow integration methods
        Task<BaseModel> ProcessImportedEnquiriesWorkflowAsync(List<int> enquiryIds, string processedBy);
        Task<BaseModel> GetImportedEnquiriesWorkflowStatsAsync();
    }
}
