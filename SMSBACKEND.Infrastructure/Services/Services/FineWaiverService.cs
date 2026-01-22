using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class FineWaiverService : BaseService<FineWaiverModel, FineWaiver, int>, IFineWaiverService
    {
        private readonly IFineWaiverRepository _finewaiverRepository;
        private readonly ILogger<FineWaiverService> _logger;
        private readonly INotificationService _notificationService;
        
        public FineWaiverService(
            IMapper mapper, 
            IFineWaiverRepository finewaiverRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<FineWaiverService> logger,
            INotificationService notificationService
            ) : base(mapper, finewaiverRepository, unitOfWork, sseService, cacheProvider)
        {
            _finewaiverRepository = finewaiverRepository;
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<BaseModel> RequestFineWaiver(FineWaiverRequest request)
        {
            try
            {
                var waiver = await _finewaiverRepository.RequestWaiverAsync(request);
                var model = mapper.Map<FineWaiverModel>(waiver);
                
                return new BaseModel
                {
                    Success = true,
                    Data = model,
                    Message = "Fine waiver request submitted successfully. Awaiting admin approval.",
                    Total = 1,
                    LastId = waiver.Id,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FineWaiverService] RequestFineWaiver error");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to request fine waiver: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> ApproveFineWaiver(FineWaiverApprovalRequest request)
        {
            try
            {
                var waiver = await _finewaiverRepository.ApproveWaiverAsync(request);
                var model = mapper.Map<FineWaiverModel>(waiver);
                
                string message = request.IsApproved 
                    ? $"Fine waiver approved successfully. ₨{waiver.WaiverAmount} fine waived."
                    : "Fine waiver rejected.";

                // Send email notification
                try
                {
                    await _notificationService.SendFineWaiverStatusEmailAsync(
                        waiver.StudentId,
                        request.IsApproved,
                        waiver.WaiverAmount,
                        waiver.ApprovedBy ?? "Admin",
                        request.ApprovalNote ?? "");
                    
                    _logger.LogInformation("📧 Fine waiver email sent for Student {StudentId}", waiver.StudentId);
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "⚠️ Failed to send fine waiver email, but waiver was processed");
                }

                return new BaseModel
                {
                    Success = true,
                    Data = model,
                    Message = message,
                    Total = 1,
                    LastId = waiver.Id,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FineWaiverService] ApproveFineWaiver error");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to process fine waiver: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetPendingWaivers()
        {
            try
            {
                var waivers = await _finewaiverRepository.GetPendingWaiversAsync();
                var models = mapper.Map<List<FineWaiverModel>>(waivers);
                
                return new BaseModel
                {
                    Success = true,
                    Data = models,
                    Message = "Pending waivers retrieved successfully",
                    Total = models.Count,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FineWaiverService] GetPendingWaivers error");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get pending waivers: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetWaiversByStudent(int studentId)
        {
            try
            {
                var waivers = await _finewaiverRepository.GetWaiversByStudentAsync(studentId);
                var models = mapper.Map<List<FineWaiverModel>>(waivers);
                
                return new BaseModel
                {
                    Success = true,
                    Data = models,
                    Message = "Student waivers retrieved successfully",
                    Total = models.Count,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FineWaiverService] GetWaiversByStudent error");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get student waivers: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel> GetWaiversByStatus(string status)
        {
            try
            {
                var waivers = await _finewaiverRepository.GetWaiversByStatusAsync(status);
                var models = mapper.Map<List<FineWaiverModel>>(waivers);
                
                return new BaseModel
                {
                    Success = true,
                    Data = models,
                    Message = $"{status} waivers retrieved successfully",
                    Total = models.Count,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FineWaiverService] GetWaiversByStatus error");
                return new BaseModel
                {
                    Success = false,
                    Data = null,
                    Message = $"Failed to get waivers by status: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }
    }
}
