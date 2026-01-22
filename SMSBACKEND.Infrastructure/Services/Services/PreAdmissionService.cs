using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Services
{
    public class PreAdmissionService : BaseService<PreAdmissionModel, PreAdmission, int>, IPreAdmissionService
    {
        private readonly IPreAdmissionRepository _preAdmissionRepository;
        private readonly IFeeAssignmentService _feeAssignmentService;
        private readonly IEnquiryRepository _enquiryRepository;
        private readonly IFeeAssignmentRepository _feeAssignmentRepository;
        private readonly IFeeTransactionRepository _feeTransactionRepository;
        
        public PreAdmissionService(
            IMapper mapper, 
            IPreAdmissionRepository preAdmissionRepository,
            IFeeAssignmentService feeAssignmentService,
            IEnquiryRepository enquiryRepository,
            IFeeAssignmentRepository feeAssignmentRepository,
            IFeeTransactionRepository feeTransactionRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<PreAdmissionService> logger
            ) : base(mapper, preAdmissionRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _preAdmissionRepository = preAdmissionRepository;
            _feeAssignmentService = feeAssignmentService;
            _enquiryRepository = enquiryRepository;
            _feeAssignmentRepository = feeAssignmentRepository;
            _feeTransactionRepository = feeTransactionRepository;
        }

        /// <summary>
        /// Override Add to handle fee configuration snapshot, enquiry status update, and fee assignment creation
        /// </summary>
        public override async Task<PreAdmissionModel> Add(PreAdmissionModel businessEntity)
        {
            try
            {
                _logger.LogInformation($"[PreAdmissionService] Creating PreAdmission for EnquiryId: {businessEntity.EnquiryId}");

                // 1. Generate fee configuration snapshot if FeeGroupFeeTypeId is provided
                FeePreviewResponse? feePreviewData = null;
                if (businessEntity.FeeGroupFeeTypeId.HasValue)
                {
                    var feePreview = await _feeAssignmentService.GetFeePreview(new FeePreviewRequest
                    {
                        FeeGroupFeeTypeId = businessEntity.FeeGroupFeeTypeId,
                        ClassId = businessEntity.ApplyingClass
                    });

                    if (feePreview.Success && feePreview.Data != null)
                    {
                        feePreviewData = feePreview.Data as FeePreviewResponse;
                        businessEntity.FeeConfigurationSnapshot = JsonSerializer.Serialize(feePreview.Data);
                        _logger.LogInformation($"[PreAdmissionService] Fee configuration snapshot created for FeeGroupFeeTypeId: {businessEntity.FeeGroupFeeTypeId}");
                    }
                }

                // 2. Call base Add method to create PreAdmission
                var result = await base.Add(businessEntity);
                _logger.LogInformation($"[PreAdmissionService] PreAdmission created with Id: {result.Id}");

                // 3. Update Enquiry status to "Converted"
                if (businessEntity.EnquiryId > 0)
                {
                    var enquiry = await _enquiryRepository.GetAsync(businessEntity.EnquiryId);
                    if (enquiry != null)
                    {
                        enquiry.Status = "Converted";
                        enquiry.UpdatedAt = DateTime.UtcNow;
                        enquiry.UpdatedBy = businessEntity.CreatedBy;
                        
                        await _enquiryRepository.UpdateAsync(enquiry);
                        await unitOfWork.SaveChangesAsync();
                        
                        _logger.LogInformation($"[PreAdmissionService] Updated Enquiry {enquiry.Id} status to 'Converted'");
                    }
                }

                // 4. Create FeeAssignment entries if fee configuration exists
                if (feePreviewData != null && feePreviewData.FeeTypes != null && feePreviewData.FeeTypes.Any() && result.Id > 0)
                {
                    _logger.LogInformation($"[PreAdmissionService] Creating FeeAssignment entries for PreAdmission {result.Id}");
                    
                    var currentDate = DateTime.UtcNow;
                    var assignments = new List<FeeAssignment>();

                    // Create assignments based on fee frequency
                    foreach (var feeType in feePreviewData.FeeTypes)
                    {
                        if (feeType.FeeFrequency == "Monthly")
                        {
                            // Create 12 monthly assignments
                            for (int month = 1; month <= 12; month++)
                            {
                                var assignment = new FeeAssignment
                                {
                                    FeeGroupFeeTypeId = businessEntity.FeeGroupFeeTypeId!.Value,
                                    StudentId = null, // No student yet (provisional)
                                    ApplicationId = result.Id, // Link to PreAdmission
                                    IsProvisional = true, // Mark as provisional
                                    Month = month,
                                    Year = DateTime.UtcNow.Year,
                                    Amount = feeType.Amount,
                                    DueDate = feeType.DueDate ?? new DateTime(DateTime.UtcNow.Year, month, 10),
                                    FeeDiscountId = null,
                                    AmountDiscount = feeType.DiscountAmount,
                                    AmountFine = 0,
                                    FinalAmount = feeType.Amount - feeType.DiscountAmount,
                                    Status = Common.Utilities.Enums.FeeStatus.Pending,
                                    PaidAmount = 0,
                                    IsPartialPaymentAllowed = false,
                                    Description = $"{feeType.FeeTypeName} - {result.ApplicationNumber}",
                                    TenantId = businessEntity.TenantId,
                                    CreatedAt = currentDate,
                                    CreatedAtUtc = currentDate,
                                    CreatedBy = businessEntity.CreatedBy ?? "System",
                                    UpdatedAt = currentDate,
                                    UpdatedBy = businessEntity.CreatedBy ?? "System",
                                    IsDeleted = false
                                };
                                assignments.Add(assignment);
                            }
                        }
                        else if (feeType.FeeFrequency == "OneTime" || feeType.FeeFrequency == "Annually")
                        {
                            // Create single assignment
                            var assignment = new FeeAssignment
                            {
                                FeeGroupFeeTypeId = businessEntity.FeeGroupFeeTypeId!.Value,
                                StudentId = null,
                                ApplicationId = result.Id,
                                IsProvisional = true,
                                Month = 0, // 0 for non-monthly fees
                                Year = DateTime.UtcNow.Year,
                                Amount = feeType.Amount,
                                DueDate = feeType.DueDate ?? DateTime.UtcNow.AddDays(30),
                                FeeDiscountId = null,
                                AmountDiscount = feeType.DiscountAmount,
                                AmountFine = 0,
                                FinalAmount = feeType.Amount - feeType.DiscountAmount,
                                Status = Common.Utilities.Enums.FeeStatus.Pending,
                                PaidAmount = 0,
                                IsPartialPaymentAllowed = false,
                                Description = $"{feeType.FeeTypeName} - {result.ApplicationNumber}",
                                TenantId = businessEntity.TenantId,
                                CreatedAt = currentDate,
                                CreatedAtUtc = currentDate,
                                CreatedBy = businessEntity.CreatedBy ?? "System",
                                UpdatedAt = currentDate,
                                UpdatedBy = businessEntity.CreatedBy ?? "System",
                                IsDeleted = false
                            };
                            assignments.Add(assignment);
                        }
                    }

                    if (assignments.Any())
                    {
                        await _feeAssignmentRepository.AddAsync(assignments);
                        await unitOfWork.SaveChangesAsync();
                        
                        _logger.LogInformation($"[PreAdmissionService] Created {assignments.Count} FeeAssignment entries");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionService] Error creating pre-admission: {ex.Message}");
                throw new Exception($"Error creating pre-admission: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Override Update to handle fee configuration snapshot
        /// </summary>
        public override async Task Update(PreAdmissionModel businessEntity)
        {
            try
            {
                // Generate fee configuration snapshot if FeeGroupFeeTypeId is provided
                if (businessEntity.FeeGroupFeeTypeId.HasValue)
                {
                    var feePreview = await _feeAssignmentService.GetFeePreview(new FeePreviewRequest
                    {
                        FeeGroupFeeTypeId = businessEntity.FeeGroupFeeTypeId,
                        ClassId = businessEntity.ApplyingClass
                    });

                    if (feePreview.Success && feePreview.Data != null)
                    {
                        businessEntity.FeeConfigurationSnapshot = JsonSerializer.Serialize(feePreview.Data);
                    }
                }

                // Call base update method
                await base.Update(businessEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating pre-admission: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Override Delete to revert Enquiry status and delete related FeeAssignments
        /// </summary>
        public override async Task Delete(int id)
        {
            try
            {
                _logger.LogInformation($"[PreAdmissionService] Deleting PreAdmission with Id: {id}");

                // 1. Get the PreAdmission to find the EnquiryId
                var preAdmission = await _preAdmissionRepository.GetAsync(id);
                if (preAdmission == null)
                {
                    throw new Exception($"PreAdmission with Id {id} not found");
                }

                int enquiryId = preAdmission.EnquiryId;
                _logger.LogInformation($"[PreAdmissionService] Found PreAdmission with EnquiryId: {enquiryId}");

                // 2. Delete related FeeAssignments (provisional fees)
                var feeAssignments = await _feeAssignmentRepository.GetAsync(
                    where: fa => fa.ApplicationId == id && !fa.IsDeleted);
                
                if (feeAssignments != null && feeAssignments.Any())
                {
                    _logger.LogInformation($"[PreAdmissionService] Found {feeAssignments.Count} FeeAssignments to delete");
                    
                    foreach (var assignment in feeAssignments)
                    {
                        assignment.IsDeleted = true;
                        assignment.DeletedAt = DateTime.UtcNow;
                        assignment.DeletedBy = preAdmission.UpdatedBy ?? "System";
                        await _feeAssignmentRepository.UpdateAsync(assignment);
                    }
                    
                    await unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"[PreAdmissionService] Deleted {feeAssignments.Count} FeeAssignments");
                }

                // 3. Call base Delete method
                await base.Delete(id);
                _logger.LogInformation($"[PreAdmissionService] PreAdmission {id} deleted");

                // 4. Revert Enquiry status back to previous status (e.g., "Interested" or "New")
                if (enquiryId > 0)
                {
                    var enquiry = await _enquiryRepository.GetAsync(enquiryId);
                    if (enquiry != null && enquiry.Status == "Converted")
                    {
                        // Revert to "Interested" status (or you can track original status)
                        enquiry.Status = "Interested";
                        enquiry.UpdatedAt = DateTime.UtcNow;
                        enquiry.UpdatedBy = preAdmission.UpdatedBy ?? "System";
                        
                        await _enquiryRepository.UpdateAsync(enquiry);
                        await unitOfWork.SaveChangesAsync();
                        
                        _logger.LogInformation($"[PreAdmissionService] Reverted Enquiry {enquiry.Id} status to 'Interested'");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionService] Error deleting pre-admission: {ex.Message}");
                throw new Exception($"Error deleting pre-admission: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get provisional fees for an application
        /// </summary>
        public async Task<BaseModel> GetProvisionalFees(int applicationId)
        {
            try
            {
                _logger.LogInformation($"[PreAdmissionService] Getting provisional fees for Application: {applicationId}");

                var feeAssignments = await _feeAssignmentRepository.GetAsync(
                    where: fa => fa.ApplicationId == applicationId && fa.IsProvisional && !fa.IsDeleted,
                    includeProperties: query => query
                        .Include(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                        .Include(fa => fa.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                        .Include(fa => fa.FeeTransactions)
                );

                var feeModels = mapper.Map<List<FeeAssignmentModel>>(feeAssignments);
                
                _logger.LogInformation($"[PreAdmissionService] Found {feeModels.Count} provisional fees");
                
                return BaseModel.Succeed(
                    data: feeModels,
                    total: feeModels.Count,
                    message: "Provisional fees retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionService] Error getting provisional fees: {ex.Message}");
                return BaseModel.Failed($"Error retrieving provisional fees: {ex.Message}");
            }
        }

        /// <summary>
        /// Process provisional fee payment
        /// </summary>
        public async Task<BaseModel> ProcessProvisionalFeePayment(ProvisionalFeePaymentRequest request)
        {
            try
            {
                _logger.LogInformation($"[PreAdmissionService] Processing provisional fee payment for Application: {request.ApplicationId}");

                var transactionIds = new List<int>();
                var errors = new List<string>();

                // Get PreAdmission details
                var preAdmission = await _preAdmissionRepository.GetAsync(request.ApplicationId);
                if (preAdmission == null)
                {
                    return BaseModel.Failed("PreAdmission/Application not found");
                }

                // Process each fee payment
                foreach (var feePayment in request.FeePayments)
                {
                    try
                    {
                        // Get fee assignment
                        var assignment = await _feeAssignmentRepository.GetAsync(feePayment.FeeAssignmentId);
                        
                        if (assignment == null || !assignment.IsProvisional)
                        {
                            errors.Add($"Fee assignment {feePayment.FeeAssignmentId} not found or not provisional");
                            continue;
                        }

                        // Create transaction
                        var feeTransaction = new FeeTransaction
                        {
                            FeeAssignmentId = feePayment.FeeAssignmentId,
                            FeeGroupFeeTypeId = feePayment.FeeGroupFeeTypeId,
                            StudentId = null, // No student yet (provisional)
                            ApplicationId = request.ApplicationId, // Link to PreAdmission
                            Month = assignment.Month, // From FeeAssignment
                            Year = assignment.Year, // From FeeAssignment
                            AmountPaid = feePayment.AmountPaying,
                            DiscountApplied = feePayment.DiscountApplied,
                            FineApplied = feePayment.FineApplied,
                            PaymentDate = request.PaymentDate,
                            PaymentMethod = feePayment.PaymentMethod,
                            ReferenceNo = string.IsNullOrEmpty(feePayment.BankName) 
                                ? feePayment.ReferenceNumber ?? string.Empty 
                                : $"{feePayment.ReferenceNumber} - {feePayment.BankName}",
                            Note = feePayment.Note ?? string.Empty,
                            TenantId = preAdmission.TenantId,
                            CreatedAt = DateTime.UtcNow,
                            CreatedAtUtc = DateTime.UtcNow,
                            CreatedBy = request.CollectedBy,
                            IsDeleted = false
                        };

                        var createdTransaction = await _feeTransactionRepository.AddAsync(feeTransaction);
                        await unitOfWork.SaveChangesAsync();

                        transactionIds.Add(createdTransaction.Id);

                        // Update FeeAssignment status and paid amount
                        assignment.PaidAmount += feePayment.AmountPaying;
                        assignment.AmountFine += feePayment.FineApplied;
                        
                        if (assignment.PaidAmount >= assignment.FinalAmount + assignment.AmountFine)
                        {
                            assignment.Status = Common.Utilities.Enums.FeeStatus.Paid;
                        }
                        else if (assignment.PaidAmount > 0)
                        {
                            assignment.Status = Common.Utilities.Enums.FeeStatus.Partial;
                        }
                        
                        await _feeAssignmentRepository.UpdateAsync(assignment);
                        await unitOfWork.SaveChangesAsync();

                        _logger.LogInformation($"✅ Fee transaction created - ID: {createdTransaction.Id}, Amount: {feeTransaction.AmountPaid}");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing fee {feePayment.FeeAssignmentId}: {ex.Message}");
                        _logger.LogError($"❌ Error processing fee payment: {ex.Message}");
                    }
                }

                // Prepare response
                var message = errors.Any() 
                    ? $"Processed {transactionIds.Count} payments with {errors.Count} errors" 
                    : $"Successfully processed {transactionIds.Count} payments";

                if (errors.Any())
                {
                    _logger.LogWarning($"[PreAdmissionService] Payment completed with errors: {string.Join("; ", errors)}");
                }

                return BaseModel.Succeed(
                    data: transactionIds,
                    message: message,
                    total: transactionIds.Count
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionService] Error processing provisional fee payment: {ex.Message}");
                return BaseModel.Failed($"Error processing payment: {ex.Message}");
            }
        }
    }
}
