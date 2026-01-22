
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Microsoft.EntityFrameworkCore;
using static Common.Utilities.Enums;
using System.Linq;
using AutoMapper;

namespace Infrastructure.Repository
{
    public class FeeAssignmentRepository : BaseRepository<FeeAssignment, int>, IFeeAssignmentRepository
    {
        private readonly ISessionRepository _sessionRepository;
        public FeeAssignmentRepository(IMapper mapper, ISqlServerDbContext context, ISessionRepository sessionRepository) : base(mapper, context)
        {
            _sessionRepository = sessionRepository;
        }
        public async Task<List<FeeAssignmentModel>> GetFeeAssignmentsByStudentId(FeeAssignmentRequest request)
        {
            try
            {
                var assignedFees = await DbContext.FeeAssignment

                    .AsSplitQuery()
                    .Include(x => x.FeeTransactions)
                    .Include(x => x.FeeGroupFeeType)
                    .Include(x => x.Student)
                        .ThenInclude(x => x.ClassAssignments.Where(i => i.SessionId == 2))
                            .ThenInclude(ca => ca.Class)
                                .ThenInclude(x => x.ClassSections)
                                    .ThenInclude(ca => ca.Section)
                    .Where(fa => fa.StudentId == request.StudentId && fa.FeeGroupFeeType != null)
                    .Select(s => new FeeAssignmentModel
                    {
                        Id = s.Id,
                        StudentId = s.Student.Id,
                        FeeGroupFeeTypeId = s.FeeGroupFeeTypeId,
                        FeeDiscountId = s.FeeDiscountId,
                        Month = s.Month,
                        Year = s.Year,
                        Amount = s.Amount,
                        Status = s.Status,
                        AmountDiscount = s.AmountDiscount,
                        AmountFine = s.AmountFine,
                        FinalAmount = DateTime.Now > s.DueDate ? s.FinalAmount + s.AmountFine : s.FinalAmount,
                        PaidAmount = s.PaidAmount,
                        BalanceAmount = (DateTime.Now > s.DueDate ? s.FinalAmount + s.AmountFine : s.FinalAmount) - s.PaidAmount,
                        Description = s.Description,
                        DueDate = s.DueDate,
                        IsPartialPaymentAllowed = s.IsPartialPaymentAllowed,
                        StudentName = s.Student.StudentInfo.FirstName + " " + s.Student.StudentInfo.LastName,
                        AdmissionNo = s.Student.StudentInfo.AdmissionNo,
                        RollNo = s.Student.StudentInfo.RollNo,
                        ClassName = string.Join(", ", s.Student.ClassAssignments.Select(ca => $"{ca.Class.Name} - {ca.Section.Name}")),
                        FeeTransactions = mapper.Map<List<FeeTransactionModel>>(s.FeeTransactions)
                    })
                    .OrderBy(x=> x.Month)
                    .ToListAsync();
                return assignedFees;
            }
            catch (Exception ex)
            {
                // Log error if needed
                throw;
            }
        }
        public async Task<List<FeeAssignmentModel>> GetFeeAssignment(FeeAssignmentRequest request)
        {
            try
            {
                var activeSession = await _sessionRepository.GetActiveSessionId();
                // ✅ Get student status and amounts in one query
                var studentIdTransactionStatus = await DbContext.FeeAssignment
                .Where(fa => request.FeeGroupFeeTypeIds.Contains(fa.FeeGroupFeeTypeId) && fa.Month == request.Month && fa.Year == request.Year)
                .Select(fa => new
                {
                    fa.StudentId,
                    fa.Amount,
                    IsDisabled = fa.FeeTransactions.Any() // True if transactions exist
                }).ToListAsync();
                
                var studentStatusMap = studentIdTransactionStatus
                    .GroupBy(s => s.StudentId)
                    .ToDictionary(g => g.Key, g => g.Any(x => x.IsDisabled));
                
                var studentAmountMap = studentIdTransactionStatus
                    .GroupBy(s => s.StudentId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                // ✅ Get students based on ClassId and SectionId directly (not from ClassAssignments)
                var students = await DbContext.Students
                   .Include(s => s.ClassAssignments.Where(i => i.SessionId == activeSession)).ThenInclude(ca => ca.Class)
                   .Include(s => s.ClassAssignments.Where(i => i.SessionId == activeSession)).ThenInclude(ca => ca.Section)
                   .Where(s =>
                   s.ClassAssignments.Any(ca => ca.ClassId == request.ClassId && ca.SectionId == request.SectionId && ca.SessionId == activeSession))
                   .Select(s => new FeeAssignmentModel
                   {
                       StudentId = s.Id,
                       MonthlyFees = s.StudentFinancial.MonthlyFees,
                       StudentName = s.StudentInfo.FirstName + ' ' + s.StudentInfo.LastName,
                       AdmissionNo = s.StudentInfo.AdmissionNo,
                       RollNo = s.StudentInfo.RollNo,
                       ClassName = string.Join(", ", s.ClassAssignments.Select(ca => $"{ca.Class.Name} - {ca.Section.Name}")),
                       IsSelected = studentStatusMap.ContainsKey(s.Id),
                       Month = request.Month,
                       Year = request.Year,
                       IsDisabled = studentStatusMap.ContainsKey(s.Id) && studentStatusMap[s.Id]
                   })
                   .ToListAsync();
                
                // ✅ Map amounts from the existing query result
                foreach (var student in students)
                {
                    student.Amount = studentAmountMap.ContainsKey(student.StudentId) 
                        ? studentAmountMap[student.StudentId] 
                        : 0;
                }
                
                return students;
            }
            catch (Exception ex)
            {
                // Log error if needed
                throw;
            }
        }
        public async Task AssignFee(FeeAssignmentRequest request)
        {
            try
            {
                var feeGroupFeeTypes = await DbContext.FeeGroupFeeType.Include(x=> x.FeeType).Include(x => x.FeeGroup)
                    .Where(fgft => request.FeeTypeIds.Contains(fgft.FeeTypeId) && request.FeeGroupIds.Contains(fgft.FeeGroupId))
                    .ToListAsync();

                var assignmentsToAdd = new List<FeeAssignment>();
                var assignmentsToRemove = new List<FeeAssignment>();

                foreach (var studentModel in request.Students)
                {
                    foreach (var feeType in feeGroupFeeTypes)
                    {
                        var fgft = feeGroupFeeTypes.FirstOrDefault(x => x.FeeTypeId == feeType.FeeTypeId);
                        if (fgft == null) continue;

                        var existingAssignment = await DbContext.FeeAssignment
                            .FirstOrDefaultAsync(fa => fa.StudentId == studentModel.StudentId && fa.FeeGroupFeeTypeId == fgft.Id && request.Month == fa.Month && request.Year == fa.Year);

                        if (studentModel.IsSelected)
                        {
                            if (existingAssignment == null)
                            {
                                decimal amount = feeType.FeeType.FeeFrequency != FeeFrequency.Monthly ? feeType.Amount : feeType.FeeGroup.IsSystem ? studentModel.MonthlyFees == 0 ? feeType.Amount : studentModel.MonthlyFees : feeType.Amount;
                                // Calculate discount
                                decimal discountAmount = 0;
                                if (feeType.FeeDiscountId.HasValue)
                                {
                                    var discount = await DbContext.FeeDiscount.FindAsync(feeType.FeeDiscountId.Value);
                                    if (discount != null)
                                    {
                                        discountAmount = discount.DiscountType == DiscountType.Fixed
                                            ? discount.DiscountAmount
                                            : (amount * discount.DiscountPercentage / 100);
                                    }
                                }

                                // Calculate fine
                                decimal fineAmount = feeType.FineType switch
                                {
                                    FinePolicyType.Percentage => (amount * feeType.FinePercentage) / 100,
                                    FinePolicyType.Fixed => feeType.FineAmount,
                                    _ => 0
                                };

                                var finalAmount = amount - discountAmount;

                                assignmentsToAdd.Add(new FeeAssignment
                                {
                                    StudentId = studentModel.StudentId,
                                    FeeGroupFeeTypeId = fgft.Id,
                                    FeeDiscountId = feeType.FeeDiscountId,
                                    Amount = amount,
                                    AmountDiscount = discountAmount,
                                    AmountFine = fineAmount,
                                    FinalAmount = finalAmount,
                                    PaidAmount = 0,
                                    Description = feeType.FeeType.Name + " (" + feeType.FeeType.Code + ")",
                                    DueDate = feeType.DueDate ?? new DateTime(request.Year > 0 ? request.Year : DateTime.UtcNow.Year, request.Month > 0 ? request.Month : DateTime.UtcNow.Month, 12), // DateTime.UtcNow.AddDays(15),
                                    CreatedAt = DateTime.UtcNow,
                                    CreatedBy = "System",
                                    Month = request.Month,
                                    Year = request.Year,
                                    IsPartialPaymentAllowed = true
                                });
                            }
                            // else: already assigned, do nothing
                        }
                        else
                        {
                            if (existingAssignment != null)
                            {
                                assignmentsToRemove.Add(existingAssignment);
                            }
                            // else: not assigned, do nothing
                        }
                    }
                }

                if (assignmentsToAdd.Any())
                    await DbContext.FeeAssignment.AddRangeAsync(assignmentsToAdd);

                if (assignmentsToRemove.Any())
                    DbContext.FeeAssignment.RemoveRange(assignmentsToRemove);

                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error if needed
                throw;
            }
        }
        public async Task<(List<FeeAssignmentModel> fees, List<StatusCount> Counts, int TotalCount)> GetMonthlyFeeStatusAsync()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

            // Fetch payments within this month
            var fees = await DbContext.FeeAssignment
                .Where(x => x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year)
                .Select(p => new FeeAssignmentModel
                {
                    //Id = p.Id,
                    //Description = p.Description,
                    //Amount = p.Amount,
                    PaidAmount = p.PaidAmount,
                    Status = p.IsPaid ? 
                                FeeStatus.Paid : 
                                p.IsPartial ?
                                    FeeStatus.Partial : 
                                        FeeStatus.Pending
                })
                .ToListAsync();

            // Total payments
            int totalCount = fees.Count;

            var statusCounts = Enum.GetValues(typeof(FeeStatus))
                .Cast<FeeStatus>()
                .Select(status => new StatusCount
                {
                    Status = status.ToString(),
                    Count = fees.Count(p => p.Status == status)
                })
                .ToList();

            return (fees, statusCounts, totalCount);
        }

        // ==================== PROVISIONAL FEE METHODS (PRE-ADMISSION) ====================

        public async Task<FeePreviewResponse> GetFeePreview(FeePreviewRequest request)
        {
            try
            {
                if (!request.FeeGroupFeeTypeId.HasValue)
                {
                    return new FeePreviewResponse();
                }

                // Get the fee group and its associated fee types
                var feeGroupFeeTypes = await DbContext.FeeGroupFeeType
                    .Include(fgft => fgft.FeeType)
                    .Include(fgft => fgft.FeeGroup)
                    .Include(fgft => fgft.FeeDiscount)
                    .Where(fgft => fgft.FeeGroupId == request.FeeGroupFeeTypeId.Value)
                    .ToListAsync();

                if (!feeGroupFeeTypes.Any())
                {
                    return new FeePreviewResponse();
                }

                var feeGroup = feeGroupFeeTypes.First().FeeGroup;
                var feeTypePreviews = new List<FeeTypePreview>();
                decimal totalAnnual = 0, totalMonthly = 0, totalOneTime = 0;

                foreach (var fgft in feeGroupFeeTypes)
                {
                    var feeType = fgft.FeeType;
                    decimal discountAmount = 0;

                    if (fgft.FeeDiscountId.HasValue && fgft.FeeDiscount != null)
                    {
                        discountAmount = fgft.FeeDiscount.DiscountType == DiscountType.Fixed
                            ? fgft.FeeDiscount.DiscountAmount
                            : (fgft.Amount * fgft.FeeDiscount.DiscountPercentage / 100);
                    }

                    var preview = new FeeTypePreview
                    {
                        FeeTypeId = feeType.Id,
                        FeeTypeName = feeType.Name,
                        FeeTypeCode = feeType.Code,
                        FeeFrequency = feeType.FeeFrequency.ToString(),
                        Amount = fgft.Amount,
                        DueDate = fgft.DueDate,
                        FineType = fgft.FineType.ToString(),
                        FineAmount = fgft.FineAmount,
                        FinePercentage = fgft.FinePercentage,
                        DiscountName = fgft.FeeDiscount?.Name ?? "",
                        DiscountAmount = discountAmount
                    };

                    feeTypePreviews.Add(preview);

                    // Sum totals by frequency
                    switch (feeType.FeeFrequency)
                    {
                        case FeeFrequency.OneTime:
                            totalOneTime += fgft.Amount - discountAmount;
                            break;
                        case FeeFrequency.Monthly:
                            totalMonthly += fgft.Amount - discountAmount;
                            break;
                        case FeeFrequency.Annually:
                        case FeeFrequency.Quaterly:
                        case FeeFrequency.HalfYearly:
                            totalAnnual += fgft.Amount - discountAmount;
                            break;
                    }
                }

                return new FeePreviewResponse
                {
                    FeeGroupFeeTypeId = request.FeeGroupFeeTypeId,
                    FeeGroupName = feeGroup.Name,
                    FeeTypes = feeTypePreviews,
                    TotalAnnualFees = totalAnnual,
                    TotalMonthlyFees = totalMonthly,
                    TotalOneTimeFees = totalOneTime
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task AssignProvisionalFees(ProvisionalFeeAssignmentRequest request)
        {
            try
            {
                // Get fee group fee types
                var feeGroupFeeTypes = await DbContext.FeeGroupFeeType
                    .Include(x => x.FeeType)
                    .Include(x => x.FeeGroup)
                    .Include(x => x.FeeDiscount)
                    .Where(fgft => request.FeeGroupFeeTypeIds.Contains(fgft.Id))
                    .ToListAsync();

                var assignmentsToAdd = new List<FeeAssignment>();

                foreach (var fgft in feeGroupFeeTypes)
                {
                    // Only allow OneTime fees to be provisional
                    if (fgft.FeeType.FeeFrequency != FeeFrequency.OneTime)
                    {
                        continue;
                    }

                    // Check if already assigned
                    var existingAssignment = await DbContext.FeeAssignment
                        .FirstOrDefaultAsync(fa => fa.ApplicationId == request.ApplicationId && 
                                                   fa.FeeGroupFeeTypeId == fgft.Id && 
                                                   fa.Month == request.Month && 
                                                   fa.Year == request.Year);

                    if (existingAssignment != null)
                    {
                        continue; // Already assigned
                    }

                    // Calculate discount
                    decimal discountAmount = 0;
                    if (fgft.FeeDiscountId.HasValue && fgft.FeeDiscount != null)
                    {
                        discountAmount = fgft.FeeDiscount.DiscountType == DiscountType.Fixed
                            ? fgft.FeeDiscount.DiscountAmount
                            : (fgft.Amount * fgft.FeeDiscount.DiscountPercentage / 100);
                    }

                    // Calculate fine
                    decimal fineAmount = fgft.FineType switch
                    {
                        FinePolicyType.Percentage => (fgft.Amount * fgft.FinePercentage) / 100,
                        FinePolicyType.Fixed => fgft.FineAmount,
                        _ => 0
                    };

                    var finalAmount = fgft.Amount - discountAmount;

                    assignmentsToAdd.Add(new FeeAssignment
                    {
                        ApplicationId = request.ApplicationId,
                        StudentId = null, // No student yet
                        IsProvisional = true,
                        FeeGroupFeeTypeId = fgft.Id,
                        FeeDiscountId = fgft.FeeDiscountId,
                        Amount = fgft.Amount,
                        AmountDiscount = discountAmount,
                        AmountFine = fineAmount,
                        FinalAmount = finalAmount,
                        PaidAmount = 0,
                        Description = fgft.FeeType.Name + " (Provisional)",
                        DueDate = fgft.DueDate ?? DateTime.UtcNow.AddDays(30),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System",
                        Month = request.Month,
                        Year = request.Year,
                        IsPartialPaymentAllowed = true
                    });
                }

                if (assignmentsToAdd.Any())
                {
                    await DbContext.FeeAssignment.AddRangeAsync(assignmentsToAdd);
                    await DbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<FeeAssignmentModel>> GetProvisionalFeesByApplication(int applicationId)
        {
            try
            {
                var provisionalFees = await DbContext.FeeAssignment
                    .Include(x => x.FeeGroupFeeType)
                        .ThenInclude(x => x.FeeType)
                    .Include(x => x.FeeTransactions)
                    .Where(fa => fa.ApplicationId == applicationId && fa.IsProvisional)
                    .Select(s => new FeeAssignmentModel
                    {
                        Id = s.Id,
                        ApplicationId = s.ApplicationId,
                        FeeGroupFeeTypeId = s.FeeGroupFeeTypeId,
                        FeeDiscountId = s.FeeDiscountId,
                        Month = s.Month,
                        Year = s.Year,
                        Amount = s.Amount,
                        Status = s.Status,
                        AmountDiscount = s.AmountDiscount,
                        AmountFine = s.AmountFine,
                        FinalAmount = DateTime.Now > s.DueDate ? s.FinalAmount + s.AmountFine : s.FinalAmount,
                        PaidAmount = s.PaidAmount,
                        BalanceAmount = (DateTime.Now > s.DueDate ? s.FinalAmount + s.AmountFine : s.FinalAmount) - s.PaidAmount,
                        Description = s.Description,
                        DueDate = s.DueDate,
                        IsPartialPaymentAllowed = s.IsPartialPaymentAllowed,
                        FeeTransactions = mapper.Map<List<FeeTransactionModel>>(s.FeeTransactions)
                    })
                    .ToListAsync();

                return provisionalFees;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task MigrateProvisionalFees(MigrateProvisionalFeesRequest request)
        {
            try
            {
                // Get all provisional fees for the application
                var provisionalFees = await DbContext.FeeAssignment
                    .Where(fa => fa.ApplicationId == request.ApplicationId && fa.IsProvisional)
                    .ToListAsync();

                if (!provisionalFees.Any())
                {
                    return; // No provisional fees to migrate
                }

                // Update each provisional fee
                foreach (var fee in provisionalFees)
                {
                    fee.StudentId = request.StudentId;
                    fee.ApplicationId = null;
                    fee.IsProvisional = false;
                    fee.Description = fee.Description?.Replace("(Provisional)", "").Trim() ?? "";
                    fee.UpdatedAt = DateTime.UtcNow;
                    fee.UpdatedBy = "System";
                }

                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
