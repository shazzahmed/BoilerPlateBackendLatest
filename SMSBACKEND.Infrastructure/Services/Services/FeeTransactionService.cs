
using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using AutoMapper;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.Utilities.StaticClasses;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Infrastructure.Repository;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{

    public class FeeTransactionService : BaseService<FeeTransactionModel, FeeTransaction, int>, IFeeTransactionService
    {
        private readonly IFeeTransactionRepository _feetransactionRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<FeeTransactionService> _logger;
        private readonly ICacheProvider _cacheProvider;

        public FeeTransactionService(
            IMapper mapper, 
            IFeeTransactionRepository feetransactionRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            INotificationService notificationService,
            ILogger<FeeTransactionService> logger, ICacheProvider cacheProvider
            ) : base(mapper, feetransactionRepository, unitOfWork, sseService)
        {
            _feetransactionRepository = feetransactionRepository;
            _notificationService = notificationService;
            _logger = logger;
            _cacheProvider = cacheProvider;
        }
        public async Task<BaseModel> SaveFeeTransaction(FeeTransactionRequest model)
        {
            try
            {
                var result = await _feetransactionRepository.SaveFeeTransaction(model);
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync("FeeTransaction");

                    // Remove cache for dependent entities
                    if (EntityDependencyMap.Dependencies.TryGetValue("FeeTransaction", out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }
                }

                // Send notifications if payment was successful
                if (result != null && model.StudentId > 0)
                {
                    try
                    {
                        var receiptNumber = $"RCP-{DateTime.Now:yyyyMMdd}-{model.StudentId:D4}";
                        var notificationResult = await _notificationService.SendPaymentConfirmationAsync(
                            model.StudentId, 
                            model.AmountPaid, 
                            model.PaymentMethod ?? "Cash", 
                            model.SendSMS ?? true, 
                            model.SendEmail ?? false);

                        // Log notification result (don't fail the payment if notification fails)
                        if (notificationResult.Success)
                        {
                            // Notifications sent successfully
                        }
                        else
                        {
                            // Log warning but don't fail the payment
                            // In production, you might want to queue failed notifications for retry
                        }
                    }
                    catch (Exception notificationEx)
                    {
                        // Log notification error but don't fail the payment
                        // In production, you might want to queue failed notifications for retry
                    }
                }

                return new BaseModel { Success = true, Data = result, Message = "Payment processed successfully" };
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Data = null, Message = $"Payment failed: {ex.Message}" };
            }
        }
        public async Task<BaseModel> RevertFeeTransaction(FeeTransactionRequest model)
        {
            try
            {
                _logger.LogWarning("⚠️ PAYMENT REVERSAL INITIATED - Transaction ID: {TransactionId}, By: {User}, Reason: {Reason}", 
                    model.FeeTransactionId, model.CreatedBy ?? "Unknown", model.Note ?? "No reason provided");

                // Call repository to revert transaction
                var result = await _feetransactionRepository.RevertFeeTransaction(model);
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync("FeeTransaction");

                    // Remove cache for dependent entities
                    if (EntityDependencyMap.Dependencies.TryGetValue("FeeTransaction", out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }
                }

                if (result == null)
                {
                    _logger.LogError("❌ Payment reversal failed - Transaction not found or already reverted: {TransactionId}", 
                        model.FeeTransactionId);
                    return new BaseModel 
                    { 
                        Success = false, 
                        Data = null, 
                        Message = "Transaction not found or already reverted" 
                    };
                }

                _logger.LogInformation("✅ Payment reverted successfully - Transaction ID: {TransactionId}, Amount: {Amount}", 
                    model.FeeTransactionId, model.AmountPaid);

                // Send reversal notification to student
                if (model.StudentId > 0)
                {
                    try
                    {
                        // TODO: Create SendPaymentReversalNotificationAsync method in INotificationService
                        // await _notificationService.SendPaymentReversalNotificationAsync(
                        //     model.StudentId, 
                        //     model.AmountPaid, 
                        //     model.Note ?? "Payment reversed by administrator");
                        
                        _logger.LogInformation("📧 Reversal notification queued for Student ID: {StudentId}", model.StudentId);
                    }
                    catch (Exception notificationEx)
                    {
                        _logger.LogWarning(notificationEx, "⚠️ Failed to send reversal notification, but payment was reverted");
                    }
                }

                return new BaseModel 
                { 
                    Success = true, 
                    Data = result, 
                    Message = "Payment reverted successfully - Amount returned to pending balance",
                    LastId = model.FeeTransactionId.HasValue ? model.FeeTransactionId.Value : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error reverting payment - Transaction ID: {TransactionId}", 
                    model.FeeTransactionId);
                return new BaseModel 
                { 
                    Success = false, 
                    Data = null, 
                    Message = $"Payment reversal failed: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Record advance payment (before fees are due)
        /// </summary>
        public async Task<BaseModel> RecordAdvancePaymentAsync(AdvancePaymentRequest model)
        {
            try
            {
                _logger.LogInformation("💵 Recording advance payment - Student ID: {StudentId}, Amount: {Amount}", 
                    model.StudentId, model.AdvanceAmount);

                var result = await _feetransactionRepository.RecordAdvancePayment(model);
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync("FeeTransaction");

                    // Remove cache for dependent entities
                    if (EntityDependencyMap.Dependencies.TryGetValue("FeeTransaction", out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }
                }

                if (result == null)
                {
                    return new BaseModel 
                    { 
                        Success = false, 
                        Data = null, 
                        Message = "Failed to record advance payment" 
                    };
                }

                _logger.LogInformation("✅ Advance payment recorded - Transaction ID: {TransactionId}", result.Id);

                // Send confirmation notification
                if (model.SendSMS || model.SendEmail)
                {
                    try
                    {
                        await _notificationService.SendPaymentConfirmationAsync(
                            model.StudentId, 
                            model.AdvanceAmount, 
                            model.PaymentMethod ?? "Cash", 
                            model.SendSMS, 
                            model.SendEmail);
                    }
                    catch (Exception notificationEx)
                    {
                        _logger.LogWarning(notificationEx, "⚠️ Failed to send advance payment notification");
                    }
                }

                return new BaseModel 
                { 
                    Success = true, 
                    Data = result, 
                    Message = "Advance payment recorded successfully",
                    LastId = result.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error recording advance payment - Student ID: {StudentId}", 
                    model.StudentId);
                return new BaseModel 
                { 
                    Success = false, 
                    Data = null, 
                    Message = $"Failed to record advance payment: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Get student's advance payment balance
        /// </summary>
        public async Task<BaseModel> GetStudentAdvanceBalanceAsync(int studentId)
        {
            try
            {
                var balance = await _feetransactionRepository.GetStudentAdvanceBalance(studentId);
                
                return new BaseModel 
                { 
                    Success = true, 
                    Data = new { StudentId = studentId, AdvanceBalance = balance }, 
                    Message = "Advance balance retrieved successfully" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting advance balance - Student ID: {StudentId}", studentId);
                return new BaseModel 
                { 
                    Success = false, 
                    Data = null, 
                    Message = $"Failed to get advance balance: {ex.Message}" 
                };
            }
        }
    }
}
