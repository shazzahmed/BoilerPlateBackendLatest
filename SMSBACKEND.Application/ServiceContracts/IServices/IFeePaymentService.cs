using Common.DTO.Request;
using Common.DTO.Response;
using Common.DTO.Response.Fees;

namespace Application.ServiceContracts.IServices
{
    /// <summary>
    /// Fee Payment Service Interface
    /// Comprehensive payment processing for all fee scenarios
    /// </summary>
    public interface IFeePaymentService
    {
        /// <summary>
        /// Get comprehensive fee summary for payment modal
        /// </summary>
        Task<BaseModel<FeeSummaryResponse>> GetFeeSummaryAsync(int feeAssignmentId);

        /// <summary>
        /// Get all pending fees for a student
        /// </summary>
        Task<BaseModel<StudentPendingFeesResponse>> GetStudentPendingFeesAsync(int studentId);

        /// <summary>
        /// Get payment history for a fee assignment
        /// </summary>
        Task<BaseModel<List<PaymentHistoryItem>>> GetPaymentHistoryAsync(int feeAssignmentId);

        /// <summary>
        /// Pay multiple fees in single transaction
        /// </summary>
        Task<BaseModel<List<int>>> PayMultipleFeesAsync(MultiFeePaymentRequest request);

        /// <summary>
        /// Record advance payment (before fees are due)
        /// </summary>
        Task<BaseModel<int>> RecordAdvancePaymentAsync(AdvancePaymentRequest request);

        /// <summary>
        /// Generate receipt for a transaction
        /// </summary>
        Task<BaseModel<ReceiptResponse>> GenerateReceiptAsync(int transactionId);

        /// <summary>
        /// Validate payment request
        /// </summary>
        Task<(bool IsValid, string ErrorMessage)> ValidatePaymentAsync(int feeAssignmentId, decimal amountPaying, bool isPartial);

        /// <summary>
        /// Calculate fine for overdue fees
        /// </summary>
        Task<decimal> CalculateFineAsync(int feeAssignmentId);

        /// <summary>
        /// Generate unique receipt number
        /// </summary>
        string GenerateReceiptNumber();

        /// <summary>
        /// Get complete fee history for a student (paid + unpaid)
        /// </summary>
        Task<BaseModel<StudentFeeHistoryResponse>> GetStudentFeeHistoryAsync(int studentId);

        /// <summary>
        /// Get system-wide payment history with filters
        /// </summary>
        Task<BaseModel<PaymentHistoryReportResponse>> GetPaymentHistoryReportAsync(PaymentHistoryFilterRequest filters);
    }
}

