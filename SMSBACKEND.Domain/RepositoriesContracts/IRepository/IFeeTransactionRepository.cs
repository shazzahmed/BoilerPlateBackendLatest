
using Common.DTO.Request;
using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IFeeTransactionRepository : IBaseRepository<FeeTransaction, int>
    {
        Task<FeeAssignmentModel> SaveFeeTransaction(FeeTransactionRequest request);
        Task<FeeAssignmentModel> RevertFeeTransaction(FeeTransactionRequest request);
        Task<FeeTransaction> RecordAdvancePayment(AdvancePaymentRequest request);
        Task<decimal> GetStudentAdvanceBalance(int studentId);
        Task ApplyAdvancePaymentsToFee(int studentId, int feeAssignmentId);
    }
}
