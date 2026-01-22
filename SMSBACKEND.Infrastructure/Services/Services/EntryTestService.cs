using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{

    public class EntryTestService : BaseService<EntryTestModel, EntryTest, int>, IEntryTestService
    {
        private readonly IEntryTestRepository _entryTestRepository;
        private readonly IPreAdmissionRepository _preAdmissionRepository;
        
        public EntryTestService(
            IMapper mapper, 
            IEntryTestRepository entryTestRepository, 
            IPreAdmissionRepository preAdmissionRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<EntryTestService> logger
            ) : base(mapper, entryTestRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _entryTestRepository = entryTestRepository;
            _preAdmissionRepository = preAdmissionRepository;
        }

        /// <summary>
        /// Override Add to update Application status and auto-calculate marks
        /// </summary>
        public override async Task<EntryTestModel> Add(EntryTestModel businessEntity)
        {
            try
            {
                _logger.LogInformation($"[EntryTestService] Creating EntryTest for ApplicationId: {businessEntity.ApplicationId}");

                // 1. Auto-calculate percentage if marks are provided
                if (businessEntity.TotalMarks.HasValue && businessEntity.ObtainedMarks.HasValue && businessEntity.TotalMarks.Value > 0)
                {
                    businessEntity.Percentage = Math.Round((decimal)businessEntity.ObtainedMarks.Value / businessEntity.TotalMarks.Value * 100, 2);
                    
                    // Auto-update status based on percentage (assuming 50% is passing)
                    if (businessEntity.Status == "Completed")
                    {
                        businessEntity.Status = businessEntity.Percentage >= 50 ? "Passed" : "Failed";
                    }
                    
                    _logger.LogInformation($"[EntryTestService] Calculated percentage: {businessEntity.Percentage}%, Status: {businessEntity.Status}");
                }

                // 2. Call base Add method to create EntryTest
                var result = await base.Add(businessEntity);
                _logger.LogInformation($"[EntryTestService] EntryTest created with Id: {result.Id}");

                // 3. Update Application status to "TestScheduled" or "TestCompleted"
                if (businessEntity.ApplicationId > 0)
                {
                    var application = await _preAdmissionRepository.GetAsync(businessEntity.ApplicationId);
                    if (application != null)
                    {
                        // Update status based on test status
                        if (businessEntity.Status == "Passed")
                        {
                            application.Status = "TestPassed";
                        }
                        else if (businessEntity.Status == "Failed")
                        {
                            application.Status = "TestFailed";
                        }
                        else if (businessEntity.Status == "Completed")
                        {
                            application.Status = "TestCompleted";
                        }
                        else
                        {
                            application.Status = "TestScheduled";
                        }
                        
                        application.UpdatedAt = DateTime.UtcNow;
                        application.UpdatedBy = businessEntity.CreatedBy;
                        
                        await _preAdmissionRepository.UpdateAsync(application);
                        await unitOfWork.SaveChangesAsync();
                        
                        _logger.LogInformation($"[EntryTestService] Updated Application {application.Id} status to '{application.Status}'");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EntryTestService] Error creating entry test: {ex.Message}");
                throw new Exception($"Error creating entry test: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Override Update to handle marks calculation and status sync
        /// </summary>
        public override async Task Update(EntryTestModel businessEntity)
        {
            try
            {
                // Auto-calculate percentage if marks are provided
                if (businessEntity.TotalMarks.HasValue && businessEntity.ObtainedMarks.HasValue && businessEntity.TotalMarks.Value > 0)
                {
                    businessEntity.Percentage = Math.Round((decimal)businessEntity.ObtainedMarks.Value / businessEntity.TotalMarks.Value * 100, 2);
                    
                    // Auto-update status based on percentage if status is Completed
                    if (businessEntity.Status == "Completed")
                    {
                        businessEntity.Status = businessEntity.Percentage >= 50 ? "Passed" : "Failed";
                        businessEntity.CompletedDate = DateTime.UtcNow;
                    }
                }

                // Call base Update method
                await base.Update(businessEntity);

                // Sync Application status
                if (businessEntity.ApplicationId > 0)
                {
                    var application = await _preAdmissionRepository.GetAsync(businessEntity.ApplicationId);
                    if (application != null)
                    {
                        if (businessEntity.Status == "Passed")
                        {
                            application.Status = "TestPassed";
                        }
                        else if (businessEntity.Status == "Failed")
                        {
                            application.Status = "TestFailed";
                        }
                        else if (businessEntity.Status == "Completed")
                        {
                            application.Status = "TestCompleted";
                        }
                        else if (businessEntity.Status == "Scheduled")
                        {
                            application.Status = "TestScheduled";
                        }
                        
                        application.UpdatedAt = DateTime.UtcNow;
                        application.UpdatedBy = businessEntity.UpdatedBy;
                        
                        await _preAdmissionRepository.UpdateAsync(application);
                        await unitOfWork.SaveChangesAsync();
                        
                        _logger.LogInformation($"[EntryTestService] Synced Application status to '{application.Status}'");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EntryTestService] Error updating entry test: {ex.Message}");
                throw new Exception($"Error updating entry test: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Override Delete to revert Application status
        /// </summary>
        public override async Task Delete(int id)
        {
            try
            {
                _logger.LogInformation($"[EntryTestService] Deleting EntryTest with Id: {id}");

                // 1. Get the EntryTest to find the ApplicationId
                var entryTest = await _entryTestRepository.GetAsync(id);
                if (entryTest == null)
                {
                    throw new Exception($"EntryTest with Id {id} not found");
                }

                int applicationId = entryTest.ApplicationId;
                _logger.LogInformation($"[EntryTestService] Found EntryTest with ApplicationId: {applicationId}");

                // 2. Call base Delete method
                await base.Delete(id);
                _logger.LogInformation($"[EntryTestService] EntryTest {id} deleted");

                // 3. Revert Application status back to "Approved"
                if (applicationId > 0)
                {
                    var application = await _preAdmissionRepository.GetAsync(applicationId);
                    if (application != null)
                    {
                        // Revert to "Approved" status (ready for test again)
                        application.Status = "Approved";
                        application.UpdatedAt = DateTime.UtcNow;
                        application.UpdatedBy = entryTest.UpdatedBy ?? "System";
                        
                        await _preAdmissionRepository.UpdateAsync(application);
                        await unitOfWork.SaveChangesAsync();
                        
                        _logger.LogInformation($"[EntryTestService] Reverted Application {application.Id} status to 'Approved'");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EntryTestService] Error deleting entry test: {ex.Message}");
                throw new Exception($"Error deleting entry test: {ex.Message}", ex);
            }
        }
    }
}
