
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IFeeTransactionService : IBaseService<FeeTransactionModel, FeeTransaction, int>
    {
        Task<BaseModel> SaveFeeTransaction(FeeTransactionRequest model);
        Task<BaseModel> RevertFeeTransaction(FeeTransactionRequest model);
        Task<BaseModel> RecordAdvancePaymentAsync(AdvancePaymentRequest model);
        Task<BaseModel> GetStudentAdvanceBalanceAsync(int studentId);
    }
}
