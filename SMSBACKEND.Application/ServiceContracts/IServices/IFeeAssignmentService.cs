
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;
using System.Linq.Expressions;

namespace Application.ServiceContracts
{
    public interface IFeeAssignmentService : IBaseService<FeeAssignmentModel, FeeAssignment, int>
    {
        Task<BaseModel> GetFeeAssignmentsByStudentId(FeeAssignmentRequest request);
        Task<BaseModel> GetFeeAssignment(FeeAssignmentRequest request);
        Task<BaseModel> AssignFee(FeeAssignmentRequest request);
        Task<ValuePercentageModel> GetFeesCount(Expression<Func<FeeAssignment, bool>> where = null, Expression<Func<FeeAssignment, bool>> whereSecond = null);
        Task<(List<ValuePercentageModel> feetypelist, decimal totalPaid)> GetMonthlyFeeStatusAsync();
        
        // Provisional Fee Methods (Pre-Admission)
        Task<BaseModel> GetFeePreview(FeePreviewRequest request);
        Task<BaseModel> AssignProvisionalFees(ProvisionalFeeAssignmentRequest request);
        Task<BaseModel> GetProvisionalFeesByApplication(int applicationId);
        Task<BaseModel> MigrateProvisionalFees(MigrateProvisionalFeesRequest request);
    }
}
