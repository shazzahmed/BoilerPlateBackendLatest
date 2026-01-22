using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.DTO.Response.Fees;
using Common.Helpers;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Presentation.Filters;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("Api/Fees")]
    [ApiController]
    public class FeesController : ControllerBase
    {
        private readonly IFeeTypeService _feeTypeService;
        private readonly IFeeDiscountService _feeDiscountService;
        private readonly IFeeGroupService _feeGroupService;
        private readonly IFeeGroupFeeTypeService _feeGroupFeeTypeService;
        private readonly IFeeAssignmentService _feeAssignmentService;
        private readonly IFeeTransactionService _feeTransactionService;
        private readonly IFeePaymentService _feePaymentService;
        private readonly ILogger<FeesController> _logger;
        
        public FeesController(
                ILogger<FeesController> logger, 
                IFeeTypeService feeTypeService, 
                IFeeGroupService feeGroupService, 
                IFeeGroupFeeTypeService feeGroupFeeTypeService, 
                IFeeDiscountService feeDiscountService, 
                IFeeAssignmentService feeAssignmentService, 
                IFeeTransactionService feeTransactionService,
                IFeePaymentService feePaymentService)
        {
            _logger = logger;
            _feeTypeService = feeTypeService;
            _feeGroupService = feeGroupService;
            _feeGroupFeeTypeService = feeGroupFeeTypeService;
            _feeDiscountService = feeDiscountService;
            _feeAssignmentService = feeAssignmentService;
            _feeTransactionService = feeTransactionService;
            _feePaymentService = feePaymentService;
        }

        #region FeeTypes

        // GET: Api/Fees/GetAllFeeTypes
        [HttpGet]
        [Authorize]
        [ActionName("GetAllFeeTypes")]
        [Route("GetAllFeeTypes")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllFeeTypes(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonSerializerHelper.Deserialize<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var (result, count, lastId) = await _feeTypeService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/CreateFeeType
        [HttpPost]
        [Authorize]
        [ActionName("CreateFeeType")]
        [Route("CreateFeeType")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateFeeType(FeeTypeModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _feeTypeService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Fee Type {result.Name}" : $"Fee Type {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _feeTypeService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _feeTypeService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Fee Type {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/UpdateFeeType
        [HttpPost]
        [Authorize]
        [ActionName("UpdateFeeType")]
        [Route("UpdateFeeType")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateFeeType(FeeTypeModel model)
        {
            try
            {
                await _feeTypeService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Fee Type {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));

            }
        }

        // Delete: Api/Fees/DeleteFeeType
        [HttpDelete]
        [ActionName("DeleteFeeType")]
        [Route("DeleteFeeType")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteFeeType([FromQuery] int id)
        {
            try
            {
                var result = await _feeTypeService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _feeTypeService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Fee Type {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        #endregion

        #region FeeGroups

        // GET: Api/Fees/GetAllFeeGroups
        [HttpGet]
        [Authorize]
        [ActionName("GetAllFeeGroups")]
        [Route("GetAllFeeGroups")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllFeeGroups(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonSerializerHelper.Deserialize<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var (result, count, lastId) = await _feeGroupService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/CreateFeeGroup
        [HttpPost]
        [Authorize]
        [ActionName("CreateFeeGroup")]
        [Route("CreateFeeGroup")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateFeeGroup(FeeGroupModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _feeGroupService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Fee Group {result.Name}" : $"Fee Group {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _feeGroupService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _feeGroupService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Fee Group {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/UpdateFeeGroup
        [HttpPost]
        [Authorize]
        [ActionName("UpdateFeeGroup")]
        [Route("UpdateFeeGroup")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateFeeGroup(FeeGroupModel model)
        {
            try
            {
                await _feeGroupService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Fee Group {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));

            }
        }

        // Delete: Api/Fees/DeleteFeeGroup
        [HttpDelete]
        [ActionName("DeleteFeeGroup")]
        [Route("DeleteFeeGroup")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteFeeGroup([FromQuery] int id)
        {
            try
            {
                var result = await _feeGroupService.FirstOrDefaultAsync(x => x.Id == id);
                if (result != null)
                {
                    await _feeGroupService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Fee Group {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        #endregion

        #region FeeDiscounts

        // GET: Api/Fees/GetAllFeeDiscounts
        [HttpGet]
        [Authorize]
        [ActionName("GetAllFeeDiscounts")]
        [Route("GetAllFeeDiscounts")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllFeeDiscounts(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonSerializerHelper.Deserialize<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var (result, count, lastId) = await _feeDiscountService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/CreateFeeDiscount
        [HttpPost]
        [Authorize]
        [ActionName("CreateFeeDiscount")]
        [Route("CreateFeeDiscount")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateFeeDiscount(FeeDiscountModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _feeDiscountService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Fee Discount {result.Name}" : $"Fee Discount {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _feeDiscountService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _feeDiscountService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Fee Discount {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/UpdateFeeDiscount
        [HttpPost]
        [Authorize]
        [ActionName("UpdateFeeDiscount")]
        [Route("UpdateFeeDiscount")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateFeeDiscount(FeeDiscountModel model)
        {
            try
            {
                await _feeDiscountService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Fee Discount {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));

            }
        }

        // Delete: Api/Fees/DeleteFeeDiscount
        [HttpDelete]
        [ActionName("DeleteFeeDiscount")]
        [Route("DeleteFeeDiscount")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteFeeDiscount([FromQuery] int id)
        {
            try
            {
                var result = await _feeDiscountService.FirstOrDefaultAsync(x => x.Id == id);
                if (result != null)
                {
                    await _feeDiscountService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Fee Discount {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        #endregion

        #region FeeGroupFeeType

        // GET: Api/Fees/GetAllFeeGroupFeeTypes
        [HttpGet]
        [Authorize]
        [ActionName("GetAllFeeGroupFeeTypes")]
        [Route("GetAllFeeGroupFeeTypes")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllFeeGroupFeeTypes(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var result = await _feeGroupFeeTypeService.GetFeeGroupFeeType(filterObj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/SaveFeeGroupFeeType
        [HttpPost]
        [Authorize]
        [ActionName("SaveFeeGroupFeeType")]
        [Route("SaveFeeGroupFeeType")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> SaveFeeGroupFeeType(FeeGroupFeeTypeRequest model)
        {
            try
            {
                var result = await _feeGroupFeeTypeService.SaveFeeGroupFeeType(model);
                return Ok(BaseModel.Succeed(message: "Succesful", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/UpdateFeeGroupFeeTypes
        [HttpPost]
        [Authorize]
        [ActionName("UpdateFeeGroupFeeTypes")]
        [Route("UpdateFeeGroupFeeTypes")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateFeeGroupFeeTypes(FeeGroupFeeTypeModel model)
        {
            try
            {
                await _feeGroupFeeTypeService.Update(model);
                return Ok(BaseModel.Succeed(message: "Succesful"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));

            }
        }

        #endregion

        #region FeeAssignment

        // GET: Api/Fees/GetFeeAssignmentsByStudentId
        [HttpPost]
        [Authorize]
        [ActionName("GetFeeAssignmentsByStudentId")]
        [Route("GetFeeAssignmentsByStudentId")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetFeeAssignmentsByStudentId(FeeAssignmentRequest request)
        {
            try
            {
                var result = await _feeAssignmentService.GetFeeAssignmentsByStudentId(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // GET: Api/Fees/GetFeeAssignment
        [HttpPost]
        [Authorize]
        [ActionName("GetFeeAssignment")]
        [Route("GetFeeAssignment")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetFeeAssignment(FeeAssignmentRequest request)
        {
            try
            {
                var result = await _feeAssignmentService.GetFeeAssignment(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }
        // GET: Api/Fees/AssignFee
        [HttpPost]
        [Authorize]
        [ActionName("AssignFee")]
        [Route("AssignFee")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> AssignFee(FeeAssignmentRequest request)
        {
            try
            {
                var result = await _feeAssignmentService.AssignFee(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        #endregion

        #region ProvisionalFees (Pre-Admission)

        // POST: Api/Fees/GetFeePreview
        [HttpPost]
        [Authorize]
        [ActionName("GetFeePreview")]
        [Route("GetFeePreview")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetFeePreview(FeePreviewRequest request)
        {
            try
            {
                var result = await _feeAssignmentService.GetFeePreview(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController - GetFeePreview] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"Error retrieving fee preview: {ex.Message}"));
            }
        }

        // POST: Api/Fees/AssignProvisionalFees
        [HttpPost]
        [Authorize]
        [ActionName("AssignProvisionalFees")]
        [Route("AssignProvisionalFees")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> AssignProvisionalFees(ProvisionalFeeAssignmentRequest request)
        {
            try
            {
                var result = await _feeAssignmentService.AssignProvisionalFees(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController - AssignProvisionalFees] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"Error assigning provisional fees: {ex.Message}"));
            }
        }

        // GET: Api/Fees/GetProvisionalFees/{applicationId}
        [HttpGet]
        [Authorize]
        [ActionName("GetProvisionalFees")]
        [Route("GetProvisionalFees/{applicationId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetProvisionalFees(int applicationId)
        {
            try
            {
                var result = await _feeAssignmentService.GetProvisionalFeesByApplication(applicationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController - GetProvisionalFees] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"Error retrieving provisional fees: {ex.Message}"));
            }
        }

        // POST: Api/Fees/MigrateProvisionalFees
        [HttpPost]
        [Authorize]
        [ActionName("MigrateProvisionalFees")]
        [Route("MigrateProvisionalFees")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> MigrateProvisionalFees(MigrateProvisionalFeesRequest request)
        {
            try
            {
                var result = await _feeAssignmentService.MigrateProvisionalFees(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController - MigrateProvisionalFees] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"Error migrating provisional fees: {ex.Message}"));
            }
        }

        #endregion

        #region FeeTransaction

        // POST: Api/Fees/SaveFeeTransaction
        [HttpPost]
        [Authorize]
        [ActionName("SaveFeeTransaction")]
        [Route("SaveFeeTransaction")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> SaveFeeTransaction(FeeTransactionRequest model)
        {
            try
            {
                var result = await _feeTransactionService.SaveFeeTransaction(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/ProcessMultiFeePayment
        [HttpPost]
        [Authorize]
        [ActionName("ProcessMultiFeePayment")]
        [Route("ProcessMultiFeePayment")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ProcessMultiFeePayment([FromBody] MultiFeePaymentRequest model)
        {
            try
            {
                _logger.LogInformation("💰 ProcessMultiFeePayment - Student: {StudentId}, Fee Count: {Count}", 
                    model.StudentId, model.FeePayments?.Count ?? 0);

                var result = await _feePaymentService.PayMultipleFeesAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FeesController] ProcessMultiFeePayment error");
                return BadRequest(BaseModel.Failed(message: $"Error processing multi-fee payment: {ex.Message}"));
            }
        }

        // POST: Api/Fees/RevertFeeTransaction
        [HttpPost]
        [Authorize]
        [ActionName("RevertFeeTransaction")]
        [Route("RevertFeeTransaction")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> RevertFeeTransaction(FeeTransactionRequest model)
        {
            try
            {
                var result = await _feeTransactionService.RevertFeeTransaction(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FeesController] : {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }

        // POST: Api/Fees/RecordAdvancePayment
        /// <summary>
        /// Record advance payment (payment before fees are due)
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("RecordAdvancePayment")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> RecordAdvancePayment([FromBody] AdvancePaymentRequest model)
        {
            try
            {
                _logger.LogInformation("💵 Recording advance payment - Student ID: {StudentId}, Amount: {Amount}", 
                    model.StudentId, model.AdvanceAmount);
                
                var result = await _feeTransactionService.RecordAdvancePaymentAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FeesController] RecordAdvancePayment error");
                return BadRequest(BaseModel.Failed(message: $"Error recording advance payment: {ex.Message}"));
            }
        }

        // GET: Api/Fees/GetStudentAdvanceBalance/{studentId}
        /// <summary>
        /// Get current advance payment balance for a student
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("GetStudentAdvanceBalance/{studentId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetStudentAdvanceBalance(int studentId)
        {
            try
            {
                _logger.LogInformation("📊 Getting advance balance - Student ID: {StudentId}", studentId);
                
                var result = await _feeTransactionService.GetStudentAdvanceBalanceAsync(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FeesController] GetStudentAdvanceBalance error");
                return BadRequest(BaseModel.Failed(message: $"Error getting advance balance: {ex.Message}"));
            }
        }

        #endregion

        #region Enhanced Payment Endpoints (Phase 1)

        // GET: Api/Fees/GetFeeSummary/{feeAssignmentId}
        [HttpGet]
        [Route("GetFeeSummary/{feeAssignmentId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetFeeSummary(int feeAssignmentId)
        {
            try
            {
                var result = await _feePaymentService.GetFeeSummaryAsync(feeAssignmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FeesController] GetFeeSummary error");
                return BadRequest(BaseModel.Failed(message: $"Error loading fee summary: {ex.Message}"));
            }
        }

        // GET: Api/Fees/GetStudentPendingFees/{studentId}
        [HttpGet]
        [Route("GetStudentPendingFees/{studentId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStudentPendingFees(int studentId)
        {
            try
            {
                var result = await _feePaymentService.GetStudentPendingFeesAsync(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FeesController] GetStudentPendingFees error");
                return BadRequest(BaseModel.Failed(message: $"Error loading pending fees: {ex.Message}"));
            }
        }

        // GET: Api/Fees/GetPaymentHistory/{feeAssignmentId}
        [HttpGet]
        [Route("GetPaymentHistory/{feeAssignmentId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPaymentHistory(int feeAssignmentId)
        {
            try
            {
                var result = await _feePaymentService.GetPaymentHistoryAsync(feeAssignmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FeesController] GetPaymentHistory error");
                return BadRequest(BaseModel.Failed(message: $"Error loading payment history: {ex.Message}"));
            }
        }

        // GET: Api/Fees/GetStudentFeeHistory/{studentId}
        [HttpGet]
        [Route("GetStudentFeeHistory/{studentId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetStudentFeeHistory(int studentId)
        {
            try
            {
                var result = await _feePaymentService.GetStudentFeeHistoryAsync(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FeesController] GetStudentFeeHistory error");
                return BadRequest(BaseModel.Failed(message: $"Error loading student fee history: {ex.Message}"));
            }
        }

        // POST: Api/Fees/GetPaymentHistoryReport
        [HttpPost]
        [Route("GetPaymentHistoryReport")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPaymentHistoryReport([FromBody] PaymentHistoryFilterRequest filters)
        {
            try
            {
                var result = await _feePaymentService.GetPaymentHistoryReportAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FeesController] GetPaymentHistoryReport error");
                return BadRequest(BaseModel.Failed(message: $"Error loading payment history report: {ex.Message}"));
            }
        }

        #endregion

    }
}
