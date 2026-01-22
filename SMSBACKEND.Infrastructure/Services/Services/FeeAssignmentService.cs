
using Application.ServiceContracts;
using AutoMapper;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.Utilities.StaticClasses;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Infrastructure.Repository;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Infrastructure.Services.Services
{

    public class FeeAssignmentService : BaseService<FeeAssignmentModel, FeeAssignment, int>, IFeeAssignmentService
    {
        private readonly IFeeAssignmentRepository _feeassignmentRepository;
        private readonly ICacheProvider _cacheProvider;
        public FeeAssignmentService(
            IMapper mapper, 
            IFeeAssignmentRepository feeassignmentRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ILogger<FeeAssignmentService> logger,
            ICacheProvider cacheProvider = null
            ) : base(mapper, feeassignmentRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _feeassignmentRepository = feeassignmentRepository;
            _cacheProvider = cacheProvider;
        }
        public async Task<BaseModel> GetFeeAssignmentsByStudentId(FeeAssignmentRequest request)
        {
            var studentAttendance = await _feeassignmentRepository.GetFeeAssignmentsByStudentId(request);
            return new BaseModel { Success = true, Data = studentAttendance };
        }
        public async Task<BaseModel> GetFeeAssignment(FeeAssignmentRequest request)
        {
            var studentAttendance = await _feeassignmentRepository.GetFeeAssignment(request);
            return new BaseModel { Success = true, Data = studentAttendance };
        }
        public async Task<BaseModel> AssignFee(FeeAssignmentRequest request)
        {
            await _feeassignmentRepository.AssignFee(request);
            if (_cacheProvider != null)
            {
                await _cacheProvider.RemoveByTagAsync("Student");

                // Remove cache for dependent entities
                if (EntityDependencyMap.Dependencies.TryGetValue("Student", out var dependents))
                {
                    foreach (var dependent in dependents)
                        await _cacheProvider.RemoveByTagAsync(dependent);
                }
            }
            return new BaseModel { Success = true, Message = "Fee Assignment Completed" };
        }
        public async Task<ValuePercentageModel> GetFeesCount(Expression<Func<FeeAssignment, bool>> where = null, Expression<Func<FeeAssignment, bool>> whereSecond = null)
        {
            var paidFee = await GetCount(where);
            var allFee = await GetCount(whereSecond);
            var percentage = allFee == 0 ? 0 : ((float)paidFee / allFee) * 100;

            return new ValuePercentageModel
            {
                Text = $"{paidFee}/{allFee}",
                Percentage = ((float)Math.Round(percentage, 2)).ToString()
            };
        }
        public async Task<(List<ValuePercentageModel> feetypelist, decimal totalPaid)> GetMonthlyFeeStatusAsync()
        {
            var feeTypeCountList = new List<ValuePercentageModel>();
            var (fee, counts, totalCounts) = await _feeassignmentRepository.GetMonthlyFeeStatusAsync();
            foreach (var count in counts)
            {
                feeTypeCountList.Add(new ValuePercentageModel
                {
                    Text = $"{count.Count}/{totalCounts}",
                    Value = $"{count.Count}",
                    Percentage = ((float)Math.Round((double)count.Count / totalCounts * 100, 2)).ToString()
                });
            }
            var totalPaid = fee.Sum(x=> x.PaidAmount);
            return (feeTypeCountList, totalPaid);

        }

        // ==================== PROVISIONAL FEE METHODS (PRE-ADMISSION) ====================

        public async Task<BaseModel> GetFeePreview(FeePreviewRequest request)
        {
            try
            {
                var preview = await _feeassignmentRepository.GetFeePreview(request);
                return new BaseModel 
                { 
                    Success = true, 
                    Data = preview,
                    Message = "Fee preview retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"Error retrieving fee preview: {ex.Message}" 
                };
            }
        }

        public async Task<BaseModel> AssignProvisionalFees(ProvisionalFeeAssignmentRequest request)
        {
            try
            {
                await _feeassignmentRepository.AssignProvisionalFees(request);
                return new BaseModel 
                { 
                    Success = true, 
                    Message = "Provisional fees assigned successfully" 
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"Error assigning provisional fees: {ex.Message}" 
                };
            }
        }

        public async Task<BaseModel> GetProvisionalFeesByApplication(int applicationId)
        {
            try
            {
                var provisionalFees = await _feeassignmentRepository.GetProvisionalFeesByApplication(applicationId);
                return new BaseModel 
                { 
                    Success = true, 
                    Data = provisionalFees,
                    Message = "Provisional fees retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"Error retrieving provisional fees: {ex.Message}" 
                };
            }
        }

        public async Task<BaseModel> MigrateProvisionalFees(MigrateProvisionalFeesRequest request)
        {
            try
            {
                await _feeassignmentRepository.MigrateProvisionalFees(request);
                return new BaseModel 
                { 
                    Success = true, 
                    Message = "Provisional fees migrated to student successfully" 
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"Error migrating provisional fees: {ex.Message}" 
                };
            }
        }
    }
}

