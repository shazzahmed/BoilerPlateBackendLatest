using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.DTO.Response.Fees;
using Common.Utilities.StaticClasses;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;

namespace Infrastructure.Services.Services
{
    /// <summary>
    /// Fee Payment Service Implementation
    /// Handles comprehensive payment processing
    /// </summary>
    public class FeePaymentService : IFeePaymentService
    {
        private readonly SqlServerDbContext _context;
        private readonly ILogger<FeePaymentService> _logger;
        private readonly INotificationService _notificationService;
        private readonly ICacheProvider _cacheProvider;

        public FeePaymentService(SqlServerDbContext context, ILogger<FeePaymentService> logger, INotificationService notificationService, ICacheProvider cacheProvider)
        {
            _context = context;
            _logger = logger;
            _notificationService = notificationService;
            _cacheProvider = cacheProvider;
        }

        public async Task<BaseModel<FeeSummaryResponse>> GetFeeSummaryAsync(int feeAssignmentId)
        {
            try
            {
                var assignment = await _context.Set<Domain.Entities.FeeAssignment>()
                    .Include(f => f.Student).ThenInclude(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                    .Include(f => f.Student).ThenInclude(s => s.ClassAssignments).ThenInclude(ca => ca.Section)
                    .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                    .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                    .Include(f => f.AppliedDiscount)
                    .Include(f => f.FeeTransactions)
                    .FirstOrDefaultAsync(f => f.Id == feeAssignmentId);

                if (assignment == null)
                {
                    return new BaseModel<FeeSummaryResponse>
                    {
                        Success = false,
                        Data = null,
                        Message = "Fee assignment not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Calculate overdue days
                var isOverdue = assignment.DueDate.Date < DateTime.Today && !assignment.IsPaid;
                var overdueDays = isOverdue ? (DateTime.Today - assignment.DueDate.Date).Days : 0;

                // Calculate fine if overdue
                var fineAmount = isOverdue ? await CalculateFineAsync(feeAssignmentId) : assignment.AmountFine;

                // Get payment history
                var paymentHistory = assignment.FeeTransactions
                    .OrderByDescending(t => t.PaymentDate)
                    .Select(t => new PaymentHistoryItem
                    {
                        TransactionId = t.Id,
                        PaymentDate = t.PaymentDate,
                        AmountPaid = t.AmountPaid,
                        Discount = t.DiscountApplied,
                        Fine = t.FineApplied,
                        PaymentMethod = t.PaymentMethod ?? "Cash",
                        ReferenceNo = t.ReferenceNo ?? "",
                        ReceiptNo = t.ReferenceNo ?? "",
                        ReceivedBy = t.CreatedBy ?? "System",
                        Note = t.Note ?? ""
                    })
                    .ToList();

                // Calculate totals
                var alreadyPaid = assignment.FeeTransactions.Sum(t => t.AmountPaid);
                var totalPayable = assignment.Amount + fineAmount - assignment.AmountDiscount;
                var balanceDue = totalPayable - alreadyPaid;

                // Determine status
                var status = assignment.IsPaid ? "Paid" :
                            assignment.IsPartial ? "Partial" :
                            isOverdue ? "Overdue" : "Pending";

                var summary = new FeeSummaryResponse
                {
                    FeeAssignmentId = assignment.Id,
                    StudentId = assignment.StudentId ?? 0,
                    StudentName = $"{assignment.Student?.FirstName} {assignment.Student?.LastName}",
                    AdmissionNo = assignment.Student?.AdmissionNo ?? "",
                    ClassName = assignment.Student?.ClassAssignments?.FirstOrDefault()?.Class?.Name ?? "",
                    SectionName = assignment.Student?.ClassAssignments?.FirstOrDefault()?.Section?.Name ?? "",

                    FeeGroupName = assignment.FeeGroupFeeType?.FeeGroup?.Name ?? "",
                    FeeTypeName = assignment.FeeGroupFeeType?.FeeType?.Name ?? "",
                    Month = assignment.Month,
                    Year = assignment.Year,
                    DueDate = assignment.DueDate,
                    IsOverdue = isOverdue,
                    OverdueDays = overdueDays,

                    OriginalAmount = assignment.Amount,
                    DiscountAmount = assignment.AmountDiscount,
                    DiscountName = assignment.AppliedDiscount?.Name ?? "",
                    FineAmount = fineAmount,
                    TotalPayable = totalPayable,
                    AlreadyPaid = alreadyPaid,
                    BalanceDue = balanceDue,

                    Status = status,
                    IsPartialAllowed = assignment.IsPartialPaymentAllowed,
                    IsPaid = assignment.IsPaid,
                    IsPartial = assignment.IsPartial,

                    PaymentHistory = paymentHistory,

                    Description = assignment.Description ?? "",
                    FeeGroupFeeTypeId = assignment.FeeGroupFeeTypeId ?? 0
                };

                return new BaseModel<FeeSummaryResponse>
                {
                    Success = true,
                    Data = summary,
                    Message = "Fee summary loaded successfully",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting fee summary for FeeAssignmentId: {FeeAssignmentId}", feeAssignmentId);
                return new BaseModel<FeeSummaryResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error loading fee summary: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel<StudentPendingFeesResponse>> GetStudentPendingFeesAsync(int studentId)
        {
            try
            {
                var student = await _context.Students
                    .Include(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                    .Include(s => s.ClassAssignments).ThenInclude(ca => ca.Section)
                    .FirstOrDefaultAsync(s => s.Id == studentId);

                if (student == null)
                {
                    return new BaseModel<StudentPendingFeesResponse>
                    {
                        Success = false,
                        Data = null,
                        Message = "Student not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Load all non-deleted fee assignments for the student
                // Note: IsPaid is a computed property and cannot be used in LINQ-to-SQL
                var allAssignments = await _context.Set<Domain.Entities.FeeAssignment>()
                    .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                    .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                    .Include(f => f.AppliedDiscount)
                    .Include(f => f.FeeTransactions)
                    .Where(f => f.StudentId == studentId && !f.IsDeleted)
                    .ToListAsync();

                // Filter for pending fees in memory (IsPaid is computed property)
                var pendingAssignments = allAssignments.Where(f => !f.IsPaid).ToList();

                var pendingFees = new List<FeeSummaryResponse>();
                decimal totalPending = 0;
                decimal totalOverdue = 0;
                int overdueCount = 0;

                foreach (var assignment in pendingAssignments)
                {
                    var summary = await GetFeeSummaryAsync(assignment.Id);
                    if (summary.Success && summary.Data != null)
                    {
                        pendingFees.Add(summary.Data);
                        totalPending += summary.Data.BalanceDue;
                        
                        if (summary.Data.IsOverdue)
                        {
                            totalOverdue += summary.Data.BalanceDue;
                            overdueCount++;
                        }
                    }
                }

                var response = new StudentPendingFeesResponse
                {
                    StudentId = studentId,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    AdmissionNo = student.AdmissionNo ?? "",
                    PendingFees = pendingFees,
                    TotalPendingAmount = totalPending,
                    TotalOverdueAmount = totalOverdue,
                    PendingCount = pendingFees.Count,
                    OverdueCount = overdueCount
                };

                return new BaseModel<StudentPendingFeesResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Pending fees loaded successfully",
                    Total = response.PendingCount,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending fees for StudentId: {StudentId}", studentId);
                return new BaseModel<StudentPendingFeesResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error loading pending fees: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel<List<PaymentHistoryItem>>> GetPaymentHistoryAsync(int feeAssignmentId)
        {
            try
            {
                var history = await _context.Set<Domain.Entities.FeeTransaction>()
                    .Where(t => t.FeeAssignmentId == feeAssignmentId)
                    .OrderByDescending(t => t.PaymentDate)
                    .Select(t => new PaymentHistoryItem
                    {
                        TransactionId = t.Id,
                        PaymentDate = t.PaymentDate,
                        AmountPaid = t.AmountPaid,
                        Discount = t.DiscountApplied,
                        Fine = t.FineApplied,
                        PaymentMethod = t.PaymentMethod ?? "Cash",
                        ReferenceNo = t.ReferenceNo ?? "",
                        ReceiptNo = t.ReferenceNo ?? "",
                        ReceivedBy = t.CreatedBy ?? "System",
                        Note = t.Note ?? ""
                    })
                    .ToListAsync();

                return new BaseModel<List<PaymentHistoryItem>>
                {
                    Success = true,
                    Data = history,
                    Message = "Payment history loaded successfully",
                    Total = history.Count,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment history for FeeAssignmentId: {FeeAssignmentId}", feeAssignmentId);
                return new BaseModel<List<PaymentHistoryItem>>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error loading payment history: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel<List<int>>> PayMultipleFeesAsync(MultiFeePaymentRequest request)
        {
            // ✅ Use execution strategy to handle transactions with retry logic
            var strategy = _context.Database.CreateExecutionStrategy();
            
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                _logger.LogInformation("💰 Processing multi-fee payment for Student {StudentId}, Count: {Count}", 
                    request.StudentId, request.FeePayments?.Count ?? 0);

                if (request.FeePayments == null || request.FeePayments.Count == 0)
                {
                    return new BaseModel<List<int>>
                    {
                        Success = false,
                        Data = new List<int>(),
                        Message = "No fee payments provided",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                var transactionIds = new List<int>();
                var errors = new List<string>();

                // Get student details for receipt
                var student = await _context.Set<Domain.Entities.Student>()
                    .Include(s => s.StudentInfo)
                    .Include(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                    .FirstOrDefaultAsync(s => s.Id == request.StudentId && !s.IsDeleted);

                if (student == null)
                {
                    return new BaseModel<List<int>>
                    {
                        Success = false,
                        Data = new List<int>(),
                        Message = "Student not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Process each fee payment
                foreach (var feePayment in request.FeePayments)
                {
                    try
                    {
                        // Get fee assignment
                        var assignment = await _context.Set<Domain.Entities.FeeAssignment>()
                            .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                            .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                            .Include(f => f.FeeTransactions)
                            .FirstOrDefaultAsync(f => f.Id == feePayment.FeeAssignmentId && !f.IsDeleted);

                        if (assignment == null)
                        {
                            errors.Add($"Fee assignment {feePayment.FeeAssignmentId} not found");
                            continue;
                        }

                        // ✅ Check for duplicate transaction (within last 10 seconds with same amount)
                        var recentDuplicateWindow = DateTime.UtcNow.AddSeconds(-10);
                        var duplicateTransaction = await _context.Set<Domain.Entities.FeeTransaction>()
                            .Where(t => t.FeeAssignmentId == feePayment.FeeAssignmentId &&
                                       t.AmountPaid == feePayment.AmountPaying &&
                                       t.CreatedAtUtc >= recentDuplicateWindow)
                            .OrderByDescending(t => t.CreatedAtUtc)
                            .FirstOrDefaultAsync();

                        if (duplicateTransaction != null)
                        {
                            errors.Add($"Duplicate transaction detected for fee {feePayment.FeeAssignmentId}. A payment with the same amount was just processed.");
                            continue;
                        }

                        // ✅ Calculate final due amount
                        var finalDueAmount = DateTime.Now > assignment.DueDate 
                            ? assignment.Amount + assignment.AmountFine - assignment.PaidAmount 
                            : assignment.Amount - assignment.PaidAmount;

                        // ✅ Validate overpayment
                        if (feePayment.AmountPaying > finalDueAmount)
                        {
                            errors.Add($"Paid amount (₨{feePayment.AmountPaying}) exceeds remaining balance (₨{finalDueAmount}) for fee {feePayment.FeeAssignmentId}");
                            continue;
                        }

                        // ✅ Check if partial payments are disallowed
                        if (!assignment.IsPartialPaymentAllowed && feePayment.AmountPaying < finalDueAmount)
                        {
                            errors.Add($"Partial payments are not allowed for fee {feePayment.FeeAssignmentId}");
                            continue;
                        }

                        // ✅ Generate reference number (FeeAssignmentId/PaymentCount)
                        var paymentCount = await _context.Set<Domain.Entities.FeeTransaction>()
                            .CountAsync(t => t.FeeAssignmentId == feePayment.FeeAssignmentId);
                        var generatedReferenceNo = $"{feePayment.FeeAssignmentId}/{paymentCount + 1}";
                        
                        // Append bank name if provided
                        var finalReferenceNo = string.IsNullOrEmpty(feePayment.BankName) 
                            ? (string.IsNullOrEmpty(feePayment.ReferenceNumber) ? generatedReferenceNo : $"{generatedReferenceNo} - {feePayment.ReferenceNumber}")
                            : $"{generatedReferenceNo} - {feePayment.ReferenceNumber} - {feePayment.BankName}";

                        // ✅ Create transaction with assignment's discount and fine
                        var feeTransaction = new Domain.Entities.FeeTransaction
                        {
                            FeeAssignmentId = feePayment.FeeAssignmentId,
                            FeeGroupFeeTypeId = feePayment.FeeGroupFeeTypeId,
                            StudentId = request.StudentId,
                            Month = assignment.Month,
                            Year = assignment.Year,
                            AmountPaid = feePayment.AmountPaying,
                            DiscountApplied = assignment.AmountDiscount + feePayment.DiscountApplied,  // ✅ Existing + Additional
                            FineApplied = feePayment.FineApplied,  // ✅ User can waive the fine
                            PaymentDate = request.PaymentDate,
                            PaymentMethod = feePayment.PaymentMethod,
                            ReferenceNo = finalReferenceNo,
                            Note = feePayment.Note ?? string.Empty,
                            Status = Common.Utilities.Enums.TransactionStatus.Completed,
                            CreatedAt = DateTime.Now,
                            CreatedBy = request.CollectedBy,
                            IsDeleted = false
                        };

                        await _context.Set<Domain.Entities.FeeTransaction>().AddAsync(feeTransaction);

                        // ✅ Update FeeAssignment's PaidAmount
                        assignment.PaidAmount += feePayment.AmountPaying;

                        // ✅ Apply additional discount to assignment
                        //if (feePayment.DiscountApplied > 0)
                        //{
                        //    assignment.AmountDiscount += feePayment.DiscountApplied;
                        //}

                        // ✅ Update FeeAssignment status
                        assignment.Status = assignment.IsPaid ? Common.Utilities.Enums.FeeStatus.Paid :
                                           assignment.PaidAmount > 0 ? Common.Utilities.Enums.FeeStatus.Partial :
                                           Common.Utilities.Enums.FeeStatus.Pending;

                        await _context.SaveChangesAsync();

                        transactionIds.Add(feeTransaction.Id);

                        _logger.LogInformation("✅ Fee transaction created - ID: {Id}, Amount: {Amount}, Status: {Status}", 
                            feeTransaction.Id, feeTransaction.AmountPaid, assignment.Status);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing fee payment {FeeAssignmentId}", feePayment.FeeAssignmentId);
                        errors.Add($"Error processing fee {feePayment.FeeAssignmentId}: {ex.Message}");
                    }
                }

                // Handle advance payment if provided
                if (request.IncludeAdvancePayment && request.AdvanceAmount > 0)
                {
                    try
                    {
                        var advanceTransaction = new Domain.Entities.FeeTransaction
                        {
                            FeeAssignmentId = 0, // No specific assignment
                            FeeGroupFeeTypeId = 0,
                            StudentId = request.StudentId,
                            AmountPaid = request.AdvanceAmount,
                            DiscountApplied = 0,
                            FineApplied = 0,
                            PaymentDate = request.PaymentDate,
                            PaymentMethod = "Advance",
                            ReferenceNo = $"ADV-{DateTime.Now:yyyyMMddHHmmss}",
                            Note = $"Advance Payment. {request.Note}",
                            CreatedAt = DateTime.Now,
                            CreatedBy = request.CollectedBy,
                            IsDeleted = false
                        };

                        await _context.Set<Domain.Entities.FeeTransaction>().AddAsync(advanceTransaction);
                        await _context.SaveChangesAsync();

                        transactionIds.Add(advanceTransaction.Id);
                        
                            _logger.LogInformation("✅ Advance payment recorded - ID: {Id}, Amount: {Amount}", 
                            advanceTransaction.Id, advanceTransaction.AmountPaid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing advance payment");
                        errors.Add($"Error processing advance payment: {ex.Message}");
                    }
                }

                // If all failed, rollback
                if (transactionIds.Count == 0)
                {
                    await transaction.RollbackAsync();
                    return new BaseModel<List<int>>
                    {
                        Success = false,
                        Data = new List<int>(),
                        Message = $"Failed to process all payments: {string.Join(", ", errors)}",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Commit transaction
                await transaction.CommitAsync();

                _logger.LogInformation("✅ Multi-fee payment completed - Total Transactions: {Count}, Student: {StudentId}", 
                    transactionIds.Count, request.StudentId);
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

                    // Send notifications if requested
                    if (request.SendSMS || request.SendEmail)
                {
                    try
                    {
                        // Calculate total amount paid
                        var totalPaid = request.FeePayments.Sum(fp => fp.AmountPaying) + request.AdvanceAmount;
                        
                        // Send consolidated notification
                        await _notificationService.SendPaymentConfirmationAsync(
                            request.StudentId, 
                            totalPaid, 
                            "Multiple", 
                            request.SendSMS, 
                            request.SendEmail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send payment confirmation notification");
                    }
                }

                var message = errors.Count > 0 
                    ? $"Partially completed: {transactionIds.Count} succeeded, {errors.Count} failed. Errors: {string.Join(", ", errors)}"
                    : $"Successfully processed {transactionIds.Count} payment(s)";

                return new BaseModel<List<int>>
                {
                    Success = true,
                    Data = transactionIds,
                    Message = message,
                    Total = transactionIds.Count,
                    LastId = transactionIds.LastOrDefault(),
                    CorrelationId = Guid.NewGuid().ToString()
                };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "[FeePaymentService] PayMultipleFeesAsync error");
                    
                    return new BaseModel<List<int>>
                    {
                        Success = false,
                        Data = new List<int>(),
                        Message = $"Error processing multi-fee payment: {ex.Message}",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }
            }); // ✅ Close ExecuteAsync lambda
        }

        public async Task<BaseModel<int>> RecordAdvancePaymentAsync(AdvancePaymentRequest request)
        {
            // TODO: Implement in Phase 2
            throw new NotImplementedException("Advance payment will be implemented in Phase 2");
        }

        public async Task<BaseModel<ReceiptResponse>> GenerateReceiptAsync(int transactionId)
        {
            // TODO: Implement in Phase 3
            throw new NotImplementedException("Receipt generation will be implemented in Phase 3");
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidatePaymentAsync(int feeAssignmentId, decimal amountPaying, bool isPartial)
        {
            try
            {
                var assignment = await _context.Set<Domain.Entities.FeeAssignment>()
                    .Include(f => f.FeeTransactions)
                    .FirstOrDefaultAsync(f => f.Id == feeAssignmentId);

                if (assignment == null)
                {
                    return (false, "Fee assignment not found");
                }

                if (assignment.IsPaid)
                {
                    return (false, "Fee is already paid in full");
                }

                if (amountPaying <= 0)
                {
                    return (false, "Payment amount must be greater than zero");
                }

                var alreadyPaid = assignment.FeeTransactions.Sum(t => t.AmountPaid);
                var fine = assignment.IsOverdue ? await CalculateFineAsync(feeAssignmentId) : 0;
                var totalPayable = assignment.Amount + fine - assignment.AmountDiscount;
                var balanceDue = totalPayable - alreadyPaid;

                if (amountPaying > balanceDue)
                {
                    return (false, $"Payment amount (₨{amountPaying}) exceeds balance due (₨{balanceDue})");
                }

                if (isPartial && !assignment.IsPartialPaymentAllowed)
                {
                    return (false, "Partial payment is not allowed for this fee");
                }

                if (amountPaying < balanceDue && !assignment.IsPartialPaymentAllowed)
                {
                    return (false, $"Full payment of ₨{balanceDue} is required (partial not allowed)");
                }

                return (true, "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating payment");
                return (false, "Error validating payment");
            }
        }

        public async Task<decimal> CalculateFineAsync(int feeAssignmentId)
        {
            try
            {
                var assignment = await _context.Set<Domain.Entities.FeeAssignment>()
                    .Include(f => f.FeeGroupFeeType)
                    .FirstOrDefaultAsync(f => f.Id == feeAssignmentId);

                if (assignment == null || !assignment.IsOverdue)
                {
                    return 0;
                }

                // If fine already set, return it
                if (assignment.AmountFine > 0)
                {
                    return assignment.AmountFine;
                }

                // Calculate fine from FeeGroupFeeType configuration
                var feeGroupFeeType = assignment.FeeGroupFeeType;
                if (feeGroupFeeType == null)
                {
                    return 0;
                }

                var overdueDays = (DateTime.Today - assignment.DueDate.Date).Days;

                // Calculate based on fine type (enum)
                switch (feeGroupFeeType.FineType)
                {
                    case Common.Utilities.Enums.FinePolicyType.Fixed:
                        return feeGroupFeeType.FineAmount;
                    
                    case Common.Utilities.Enums.FinePolicyType.Percentage:
                        return assignment.Amount * (feeGroupFeeType.FinePercentage / 100);
                    
                    //case Common.Utilities.Enums.FinePolicyType.Daily:
                    //    return feeGroupFeeType.FineAmount * overdueDays;
                    
                    case Common.Utilities.Enums.FinePolicyType.None:
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating fine for FeeAssignmentId: {FeeAssignmentId}", feeAssignmentId);
                return 0;
            }
        }

        public string GenerateReceiptNumber()
        {
            // Format: RCP-YYYY-NNNNNN
            var year = DateTime.Now.Year;
            var random = new Random();
            var sequence = random.Next(100000, 999999);
            return $"RCP-{year}-{sequence:D6}";
        }

        /// <summary>
        /// Send payment notifications (SMS/Email) after successful payment
        /// </summary>
        public async Task<BaseModel<bool>> SendPaymentNotificationsAsync(int studentId, decimal paymentAmount, string paymentMethod, string receiptNumber, bool sendSMS = true, bool sendEmail = true)
        {
            try
            {
                _logger.LogInformation("🔔 Sending payment notifications for Student {StudentId}, Amount: {Amount}", studentId, paymentAmount);

                var results = new List<bool>();

                // Send payment confirmation
                var confirmationResult = await _notificationService.SendPaymentConfirmationAsync(
                    studentId, paymentAmount, paymentMethod, sendSMS, sendEmail);
                results.Add(confirmationResult.Success);

                // Send payment receipt
                var receiptResult = await _notificationService.SendPaymentReceiptAsync(
                    studentId, receiptNumber, new { Amount = paymentAmount, Method = paymentMethod }, sendSMS, sendEmail);
                results.Add(receiptResult.Success);

                var successCount = results.Count(r => r);
                var totalCount = results.Count;

                return new BaseModel<bool>
                {
                    Success = successCount > 0,
                    Data = successCount == totalCount,
                    Message = $"Payment notifications sent: {successCount}/{totalCount} delivered successfully",
                    Total = successCount,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending payment notifications for Student {StudentId}", studentId);
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send notifications: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        /// <summary>
        /// Send overdue fee reminders for all overdue students
        /// </summary>
        public async Task<BaseModel<bool>> SendOverdueRemindersAsync(bool sendSMS = true, bool sendEmail = true)
        {
            try
            {
                _logger.LogInformation("⚠️ Sending overdue fee reminders");

                // Get all overdue fee assignments
                var overdueAssignments = await _context.Set<Domain.Entities.FeeAssignment>()
                    .Include(f => f.Student)
                    .Where(f => f.DueDate < DateTime.Today && !f.IsPaid && !f.IsDeleted)
                    .ToListAsync();

                var results = new List<bool>();

                foreach (var assignment in overdueAssignments)
                {
                    if (assignment.StudentId.HasValue)
                    {
                        var overdueDays = (DateTime.Today - assignment.DueDate.Date).Days;
                        var balanceDue = assignment.Amount + assignment.AmountFine - assignment.AmountDiscount - 
                                       assignment.FeeTransactions.Sum(t => t.AmountPaid);

                        var reminderResult = await _notificationService.SendOverdueReminderAsync(
                            assignment.StudentId.Value, balanceDue, overdueDays, sendSMS, sendEmail);
                        results.Add(reminderResult.Success);
                    }
                }

                var successCount = results.Count(r => r);
                var totalCount = results.Count;

                return new BaseModel<bool>
                {
                    Success = successCount > 0,
                    Data = successCount == totalCount,
                    Message = $"Overdue reminders sent: {successCount}/{totalCount} delivered successfully",
                    Total = successCount,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending overdue reminders");
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send overdue reminders: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel<StudentFeeHistoryResponse>> GetStudentFeeHistoryAsync(int studentId)
        {
            try
            {
                _logger.LogInformation("📊 Getting complete fee history for Student {StudentId}", studentId);

                // Get student details
                var student = await _context.Set<Domain.Entities.Student>()
                    .Include(s => s.StudentInfo)
                    .Include(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                    .Include(s => s.ClassAssignments).ThenInclude(ca => ca.Section)
                    .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);

                if (student == null)
                {
                    return new BaseModel<StudentFeeHistoryResponse>
                    {
                        Success = false,
                        Data = null,
                        Message = "Student not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Get all fee assignments for this student
                var feeAssignments = await _context.Set<Domain.Entities.FeeAssignment>()
                    .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                    .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                    .Include(f => f.AppliedDiscount)
                    .Include(f => f.FeeTransactions)
                    .Where(f => f.StudentId == studentId && !f.IsDeleted)
                    .OrderByDescending(f => f.Year).ThenByDescending(f => f.Month)
                    .ToListAsync();

                var response = new StudentFeeHistoryResponse
                {
                    StudentId = studentId,
                    StudentName = student.FullName ?? "Unknown",
                    AdmissionNo = student.AdmissionNo ?? "",
                    ClassName = student.ClassAssignments.FirstOrDefault()?.Class?.Name ?? "N/A"
                };

                // Process all fee assignments
                var allFees = new List<StudentFeeItem>();
                
                foreach (var assignment in feeAssignments)
                {
                    var paidAmount = assignment.FeeTransactions.Sum(t => t.AmountPaid);
                    var balanceDue = assignment.Amount + assignment.AmountFine - assignment.AmountDiscount - paidAmount;
                    var isOverdue = assignment.DueDate < DateTime.Today && balanceDue > 0;
                    var daysOverdue = isOverdue ? (DateTime.Today - assignment.DueDate.Date).Days : 0;

                    string status;
                    if (balanceDue <= 0)
                        status = "Paid";
                    else if (paidAmount > 0)
                        status = "Partial";
                    else if (isOverdue)
                        status = "Overdue";
                    else
                        status = "Unpaid";

                    var feeItem = new StudentFeeItem
                    {
                        FeeAssignmentId = assignment.Id,
                        FeeGroupFeeTypeId = assignment.FeeGroupFeeTypeId,
                        FeeTypeName = assignment.FeeGroupFeeType?.FeeType?.Name ?? "Unknown",
                        FeeGroupName = assignment.FeeGroupFeeType?.FeeGroup?.Name ?? "Unknown",
                        Month = assignment.Month,
                        Year = assignment.Year,
                        DueDate = assignment.DueDate,
                        AssignedAmount = assignment.Amount,
                        DiscountAmount = assignment.AmountDiscount,
                        FineAmount = assignment.AmountFine,
                        PaidAmount = paidAmount,
                        BalanceDue = balanceDue,
                        Status = status,
                        IsOverdue = isOverdue,
                        DaysOverdue = daysOverdue,
                        Transactions = assignment.FeeTransactions.Select(t => new FeeTransactionItem
                        {
                            TransactionId = t.Id,
                            FeeAssignmentId = assignment.Id,
                            FeeTypeName = assignment.FeeGroupFeeType?.FeeType?.Name ?? "Unknown",
                            AmountPaid = t.AmountPaid,
                            Discount = t.DiscountApplied,
                            Fine = t.FineApplied,
                            PaymentDate = t.PaymentDate,
                            PaymentMethod = t.PaymentMethod ?? "Cash",
                            ReferenceNo = t.ReferenceNo ?? "",
                            Note = t.Note ?? "",
                            CollectedBy = t.CreatedBy ?? "System",
                            CreatedAt = t.CreatedAt
                        }).OrderByDescending(t => t.PaymentDate).ToList()
                    };

                    allFees.Add(feeItem);
                }

                // Categorize fees
                response.PaidFees = allFees.Where(f => f.Status == "Paid").ToList();
                response.UnpaidFees = allFees.Where(f => f.Status == "Unpaid").ToList();
                response.PartiallyPaidFees = allFees.Where(f => f.Status == "Partial").ToList();
                response.OverdueFees = allFees.Where(f => f.Status == "Overdue").ToList();

                // ✅ Calculate totals directly from allFees (which already has PaidAmount calculated from transactions)
                response.TotalAssignedAmount = allFees.Sum(f => f.AssignedAmount);
                response.TotalPaidAmount = allFees.Sum(f => f.PaidAmount);  // ✅ Already calculated from transactions in loop above
                
                // ✅ Calculate total discount correctly:
                // 1. Sum all FeeAssignment.AmountDiscount (initial/base discount)
                // 2. Add any ADDITIONAL discount from transactions (where transaction discount > assignment discount)
                decimal totalDiscountFromAssignment = allFees.Sum(f => f.DiscountAmount);
                decimal additionalDiscountFromTransactions = 0;
                foreach (var fee in allFees)
                {
                    var assignment = feeAssignments.FirstOrDefault(a => a.Id == fee.FeeAssignmentId);
                    if (assignment != null && assignment.FeeTransactions.Any())
                    {
                        // Get total transaction discount
                        var transactionTotalDiscount = assignment.FeeTransactions.Sum(t => t.DiscountApplied);
                        // Get assignment discount
                        var assignmentDiscount = fee.DiscountAmount;
                        // Add only the ADDITIONAL discount (if any)
                        var additionalDiscount = Math.Max(0, transactionTotalDiscount - assignmentDiscount);
                        additionalDiscountFromTransactions += additionalDiscount;
                    }
                }
                response.TotalDiscountAmount = totalDiscountFromAssignment + additionalDiscountFromTransactions;
                
                // ✅ Calculate fine with conditional logic
                var today = DateTime.Today;
                decimal totalFine = 0;
                foreach (var fee in allFees)
                {
                    var assignment = feeAssignments.FirstOrDefault(a => a.Id == fee.FeeAssignmentId);
                    if (assignment == null) continue;
                    
                    var transactionDiscounts = assignment.FeeTransactions.Sum(t => t.DiscountApplied);
                    var totalDiscountForThisFee = fee.DiscountAmount + transactionDiscounts;
                    var isPaidInFull = fee.PaidAmount >= (fee.AssignedAmount - totalDiscountForThisFee);
                    var isOverdue = assignment.DueDate < today;
                    
                    if (isPaidInFull)
                    {
                        // For paid fees: Check if fine was collected
                        var hasCollectedFine = assignment.FeeTransactions.Any(t => t.FineApplied > 0);
                        if (hasCollectedFine)
                        {
                            totalFine += fee.FineAmount;
                        }
                    }
                    else if (isOverdue)
                    {
                        // For unpaid/partial fees: Add fine only if overdue
                        totalFine += fee.FineAmount;
                    }
                }
                response.TotalFineAmount = totalFine;
                
                // ✅ Calculate total pending (never negative)
                response.TotalPendingAmount = Math.Max(0, response.TotalAssignedAmount - response.TotalPaidAmount + response.TotalFineAmount - response.TotalDiscountAmount);

                // Get recent transactions (last 10)
                response.RecentTransactions = allFees
                    .SelectMany(f => f.Transactions)
                    .OrderByDescending(t => t.PaymentDate)
                    .Take(10)
                    .ToList();

                return new BaseModel<StudentFeeHistoryResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Fee history retrieved successfully",
                    Total = allFees.Count,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting fee history for Student {StudentId}", studentId);
                return new BaseModel<StudentFeeHistoryResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get fee history: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel<PaymentHistoryReportResponse>> GetPaymentHistoryReportAsync(PaymentHistoryFilterRequest filters)
        {
            try
            {
                _logger.LogInformation("📊 Getting payment history report with filters");

                // Base query for fee transactions
                var query = _context.Set<Domain.Entities.FeeTransaction>()
                    .Include(t => t.FeeAssignment).ThenInclude(fa => fa.Student).ThenInclude(s => s.StudentInfo)
                    .Include(t => t.FeeAssignment).ThenInclude(fa => fa.Student).ThenInclude(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                    .Include(t => t.FeeAssignment).ThenInclude(fa => fa.Student).ThenInclude(s => s.ClassAssignments).ThenInclude(ca => ca.Section)
                    .Include(t => t.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                    .Include(t => t.FeeAssignment).ThenInclude(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                    .Where(t => !t.IsDeleted)
                    .AsQueryable();

                // Apply filters
                if (filters.StudentId.HasValue)
                {
                    query = query.Where(t => t.FeeAssignment.StudentId == filters.StudentId.Value);
                }

                if (filters.FromDate.HasValue)
                {
                    query = query.Where(t => t.PaymentDate >= filters.FromDate.Value);
                }

                if (filters.ToDate.HasValue)
                {
                    query = query.Where(t => t.PaymentDate <= filters.ToDate.Value);
                }

                if (!string.IsNullOrWhiteSpace(filters.PaymentMethod))
                {
                    query = query.Where(t => t.PaymentMethod == filters.PaymentMethod);
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply sorting
                query = filters.SortDirection?.ToUpper() == "ASC"
                    ? query.OrderBy(t => EF.Property<object>(t, filters.SortBy ?? "PaymentDate"))
                    : query.OrderByDescending(t => EF.Property<object>(t, filters.SortBy ?? "PaymentDate"));

                // Apply pagination
                var transactions = await query
                    .Skip((filters.PageNumber - 1) * filters.PageSize)
                    .Take(filters.PageSize)
                    .ToListAsync();

                // Map to response
                var payments = transactions.Select(t => new PaymentReportItem
                {
                    TransactionId = t.Id,
                    StudentId = t.FeeAssignment?.Student?.Id ?? 0,
                    StudentName = t.FeeAssignment?.Student?.FullName ?? "Unknown",
                    AdmissionNo = t.FeeAssignment?.Student?.AdmissionNo ?? "",
                    ClassName = t.FeeAssignment?.Student?.ClassAssignments?.FirstOrDefault()?.Class?.Name ?? "N/A",
                    FeeTypeName = t.FeeAssignment?.FeeGroupFeeType?.FeeType?.Name ?? "Unknown",
                    FeeGroupName = t.FeeAssignment?.FeeGroupFeeType?.FeeGroup?.Name ?? "Unknown",
                    Month = t.FeeAssignment?.Month ?? 0,
                    Year = t.FeeAssignment?.Year ?? 0,
                    AmountPaid = t.AmountPaid,
                    Discount = t.DiscountApplied,
                    Fine = t.FineApplied,
                    NetAmount = t.AmountPaid + t.FineApplied - t.DiscountApplied,
                    PaymentDate = t.PaymentDate,
                    PaymentMethod = t.PaymentMethod ?? "Cash",
                    ReferenceNo = t.ReferenceNo ?? "",
                    CollectedBy = t.CreatedBy ?? "System",
                    CreatedAt = t.CreatedAt
                }).ToList();

                var response = new PaymentHistoryReportResponse
                {
                    Payments = payments,
                    TotalRecords = totalCount,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize,
                    TotalAmountCollected = payments.Sum(p => p.AmountPaid),
                    TotalDiscountGiven = payments.Sum(p => p.Discount),
                    TotalFineCollected = payments.Sum(p => p.Fine),
                    PaymentsByMethod = payments.GroupBy(p => p.PaymentMethod)
                        .ToDictionary(g => g.Key, g => g.Sum(p => p.AmountPaid)),
                    PaymentsByClass = payments.GroupBy(p => p.ClassName)
                        .ToDictionary(g => g.Key, g => g.Sum(p => p.AmountPaid)),
                    PaymentsByFeeType = payments.GroupBy(p => p.FeeTypeName)
                        .ToDictionary(g => g.Key, g => g.Sum(p => p.AmountPaid))
                };

                return new BaseModel<PaymentHistoryReportResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Payment history retrieved successfully",
                    Total = totalCount,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting payment history report");
                return new BaseModel<PaymentHistoryReportResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get payment history: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }
    }
}

