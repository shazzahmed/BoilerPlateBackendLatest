
using Common.DTO.Request;
using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IFeeAssignmentRepository : IBaseRepository<FeeAssignment, int>
    {
        Task<List<FeeAssignmentModel>> GetFeeAssignmentsByStudentId(FeeAssignmentRequest request);
        Task<List<FeeAssignmentModel>> GetFeeAssignment(FeeAssignmentRequest request);
        Task AssignFee(FeeAssignmentRequest request);
        Task<(List<FeeAssignmentModel> fees, List<StatusCount> Counts, int TotalCount)> GetMonthlyFeeStatusAsync();
        
        // Provisional Fee Methods (Pre-Admission)
        Task<FeePreviewResponse> GetFeePreview(FeePreviewRequest request);
        Task AssignProvisionalFees(ProvisionalFeeAssignmentRequest request);
        Task<List<FeeAssignmentModel>> GetProvisionalFeesByApplication(int applicationId);
        Task MigrateProvisionalFees(MigrateProvisionalFeesRequest request);
    }
}
