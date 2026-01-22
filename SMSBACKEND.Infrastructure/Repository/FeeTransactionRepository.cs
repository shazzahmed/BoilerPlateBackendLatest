
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using static Common.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using Common.DTO.Request;
using Common.DTO.Response;
using AutoMapper;

namespace Infrastructure.Repository
{
    public class FeeTransactionRepository : BaseRepository<FeeTransaction, int>, IFeeTransactionRepository
    {
        public FeeTransactionRepository(IMapper mapper, ISqlServerDbContext context) : base(mapper, context)
        {
        }
        public async Task<FeeAssignmentModel> SaveFeeTransaction(FeeTransactionRequest request)
        {
            var assignment = await DbContext.FeeAssignment
                .FirstOrDefaultAsync(fa => fa.Id == request.FeeAssignmentId);

            if (assignment == null)
                throw new Exception("Invalid Fee Assignment.");

            // ✅ Check for duplicate transaction (within last 10 seconds with same amount)
            var recentDuplicateWindow = DateTime.UtcNow.AddSeconds(-10);
            var duplicateTransaction = await DbContext.FeeTransaction
                .Where(t => t.FeeAssignmentId == request.FeeAssignmentId &&
                           t.AmountPaid == request.AmountPaid &&
                           t.CreatedAtUtc >= recentDuplicateWindow)
                .OrderByDescending(t => t.CreatedAtUtc)
                .FirstOrDefaultAsync();

            if (duplicateTransaction != null)
            {
                throw new Exception("Duplicate transaction detected. A payment with the same amount was just processed. Please wait a moment before retrying.");
            }

            var finalDueAmount = DateTime.Now > assignment.DueDate ? assignment.Amount + assignment.AmountFine : assignment.Amount - assignment.PaidAmount;

            // Validate overpayment
            if (request.AmountPaid > finalDueAmount)
                throw new Exception($"Paid amount exceeds remaining balance. Remaining: {finalDueAmount}");

            // Check if partial payments are disallowed
            if (!assignment.IsPartialPaymentAllowed && request.AmountPaid < finalDueAmount)
                throw new Exception("Partial payments are not allowed for this fee.");

            var paymentCount = await DbContext.FeeTransaction
                .CountAsync(t => t.FeeAssignmentId == request.FeeAssignmentId);
            var referenceNo = $"{request.FeeAssignmentId}/{paymentCount + 1}";
            // Create transaction
            var transaction = new FeeTransaction
            {
                StudentId = request.StudentId,
                FeeGroupFeeTypeId = request.FeeGroupFeeTypeId,
                FeeAssignmentId = request.FeeAssignmentId,
                Month = request.Month,
                Year = request.Year,
                AmountPaid = request.AmountPaid,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod,
                DiscountApplied = assignment.AmountDiscount,
                FineApplied = assignment.AmountFine,
                ReferenceNo = referenceNo,
                Note = request.Note,
                Status = TransactionStatus.Completed
            };

            DbContext.FeeTransaction.Add(transaction);

            // Update FeeAssignment's PaidAmount
            assignment.PaidAmount += request.AmountPaid;

            // Update FeeAssignment status
            assignment.Status = assignment.IsPaid ? FeeStatus.Paid :
                                 assignment.PaidAmount > 0 ? FeeStatus.Partial :
                                 FeeStatus.Pending;

            await DbContext.SaveChangesAsync();
            var assignmentModel = mapper.Map<FeeAssignmentModel>(await DbContext.FeeAssignment
                .FirstOrDefaultAsync(fa => fa.Id == request.FeeAssignmentId));
            return assignmentModel;
        }
        public async Task<FeeAssignmentModel> RevertFeeTransaction(FeeTransactionRequest request)
        {
            var assignment = await DbContext.FeeAssignment
                .FirstOrDefaultAsync(fa => fa.Id == request.FeeAssignmentId);

            if (assignment == null)
                throw new Exception("Invalid Fee Assignment.");

            var transaction = await DbContext.FeeTransaction
                .FirstOrDefaultAsync(fa => fa.Id == request.FeeTransactionId);
            
            if (transaction == null)
                return null; // Transaction not found

            // Store amount before deletion
            var amountToRevert = transaction.AmountPaid;
            
            await DeleteAsync(transaction);

            // Update FeeAssignment's PaidAmount
            assignment.PaidAmount -= amountToRevert;

            // Update FeeAssignment status
            assignment.Status = assignment.IsPaid ? FeeStatus.Paid :
                                 assignment.PaidAmount > 0 ? FeeStatus.Partial :
                                 FeeStatus.Pending;

            await DbContext.SaveChangesAsync();
            var assignmentModel = mapper.Map<FeeAssignmentModel>(await DbContext.FeeAssignment
                .FirstOrDefaultAsync(fa => fa.Id == request.FeeAssignmentId));
            return assignmentModel;
        }

        public async Task<FeeTransaction> RecordAdvancePayment(AdvancePaymentRequest request)
        {
            // Create advance payment transaction (no specific fee assignment yet)
            var transaction = new FeeTransaction
            {
                StudentId = request.StudentId,
                FeeGroupFeeTypeId = 0, // Not assigned to specific fee yet
                FeeAssignmentId = 0, // Not assigned yet
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                AmountPaid = request.AdvanceAmount,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod,
                ReferenceNo = $"ADV-{request.StudentId}-{DateTime.Now:yyyyMMddHHmmss}",
                Note = "ADVANCE PAYMENT - " + (request.Note ?? ""),
                DiscountApplied = 0,
                FineApplied = 0,
                Status = TransactionStatus.Completed,
                IsAdvancePayment = true,
                RemainingBalance = request.AdvanceAmount, // Full amount available
                CreatedAt = DateTime.Now,
                CreatedBy = request.CreatedBy ?? "System"
            };

            DbContext.FeeTransaction.Add(transaction);
            await DbContext.SaveChangesAsync();
            
            return transaction;
        }

        public async Task<decimal> GetStudentAdvanceBalance(int studentId)
        {
            var advanceBalance = await DbContext.FeeTransaction
                .Where(t => t.StudentId == studentId && 
                           t.IsAdvancePayment && 
                           t.RemainingBalance > 0 && 
                           !t.IsDeleted)
                .SumAsync(t => t.RemainingBalance);

            return advanceBalance;
        }

        public async Task ApplyAdvancePaymentsToFee(int studentId, int feeAssignmentId)
        {
            // Get pending advances (FIFO - oldest first)
            var advancePayments = await DbContext.FeeTransaction
                .Where(t => t.StudentId == studentId && 
                           t.IsAdvancePayment && 
                           t.RemainingBalance > 0 && 
                           !t.IsDeleted)
                .OrderBy(t => t.PaymentDate)
                .ToListAsync();

            var feeAssignment = await DbContext.FeeAssignment
                .FirstOrDefaultAsync(fa => fa.Id == feeAssignmentId);

            if (feeAssignment == null || advancePayments.Count == 0)
                return;

            decimal balanceDue = feeAssignment.FinalAmount - feeAssignment.PaidAmount;

            foreach (var advance in advancePayments)
            {
                if (balanceDue <= 0)
                    break;

                decimal amountToApply = Math.Min(advance.RemainingBalance, balanceDue);

                // Create regular transaction linking advance to fee
                var paymentCount = await DbContext.FeeTransaction
                    .CountAsync(t => t.FeeAssignmentId == feeAssignmentId);

                var appliedTransaction = new FeeTransaction
                {
                    StudentId = studentId,
                    FeeGroupFeeTypeId = feeAssignment.FeeGroupFeeTypeId ?? 0,
                    FeeAssignmentId = feeAssignmentId,
                    Month = feeAssignment.Month,
                    Year = feeAssignment.Year,
                    AmountPaid = amountToApply,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Advance Payment",
                    ReferenceNo = $"{feeAssignmentId}/{paymentCount + 1} (from ADV-{advance.Id})",
                    Note = $"Applied from advance payment #{advance.Id}",
                    DiscountApplied = 0,
                    FineApplied = 0,
                    Status = TransactionStatus.Completed,
                    IsAdvancePayment = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "System - Auto-Applied"
                };

                DbContext.FeeTransaction.Add(appliedTransaction);

                // Reduce advance balance
                advance.RemainingBalance -= amountToApply;
                if (advance.RemainingBalance <= 0)
                {
                    advance.IsDeleted = true; // Mark as fully used
                }

                // Update fee assignment
                feeAssignment.PaidAmount += amountToApply;
                balanceDue -= amountToApply;
            }

            // Update FeeAssignment status
            feeAssignment.Status = feeAssignment.IsPaid ? FeeStatus.Paid :
                                    feeAssignment.PaidAmount > 0 ? FeeStatus.Partial :
                                    FeeStatus.Pending;

            await DbContext.SaveChangesAsync();
        }

    }
}
