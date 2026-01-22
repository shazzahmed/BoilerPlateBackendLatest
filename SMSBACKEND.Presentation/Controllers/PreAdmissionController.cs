using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.Helpers;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Filters;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Admissions/PreAdmissions")]
    [ApiController]
    public class PreAdmissionController : ControllerBase
    {
        private readonly IPreAdmissionService _preAdmissionService;
        private readonly IFeePaymentService _feePaymentService;
        private readonly ILogger<PreAdmissionController> _logger;

        public PreAdmissionController(
            IPreAdmissionService preAdmissionService,
            IFeePaymentService feePaymentService,
            ILogger<PreAdmissionController> logger)
        {
            _preAdmissionService = preAdmissionService;
            _feePaymentService = feePaymentService;
            _logger = logger;
        }

        // GET: api/Admissions/PreAdmissions/GetAllPreAdmissions
        [HttpGet]
        [Authorize]
        [ActionName("GetAllPreAdmissions")]
        [Route("GetAllPreAdmissions")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllPreAdmissions()
        {
            try
            {
                var (result, count, lastId) = await _preAdmissionService.Get(null, orderBy: x => x.OrderByDescending(x => x.Id),includeProperties: i => i.Include(c => c.Class).Include(e => e.Enquiry), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionController] GetAllPreAdmissions: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: api/Admissions/PreAdmissions/CreatePreAdmission
        [HttpPost]
        [Authorize]
        [ActionName("CreatePreAdmission")]
        [Route("CreatePreAdmission")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreatePreAdmission([FromBody] PreAdmissionModel model)
        {
            try
            {
                var result = await _preAdmissionService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Successfully Added PreAdmission {result.FullName}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionController] CreatePreAdmission: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // PUT: api/Admissions/PreAdmissions/UpdatePreAdmission
        [HttpPost]
        [Authorize]
        [ActionName("UpdatePreAdmission")]
        [Route("UpdatePreAdmission")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdatePreAdmission([FromBody] PreAdmissionModel model)
        {
            try
            {
                await _preAdmissionService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Successfully Updated PreAdmission {model.FullName}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionController] UpdatePreAdmission: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // DELETE: api/Admissions/PreAdmissions/DeletePreAdmission
        [HttpDelete]
        [Authorize]
        [ActionName("DeletePreAdmission")]
        [Route("DeletePreAdmission")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeletePreAdmission([FromQuery] int id)
        {
            try
            {
                var result = await _preAdmissionService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _preAdmissionService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Successfully Deleted PreAdmission {result.FullName}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionController] DeletePreAdmission: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
            }
        }

        // GET: api/Admissions/PreAdmissions/GetProvisionalFees/{applicationId}
        [HttpGet]
        [Authorize]
        [ActionName("GetProvisionalFees")]
        [Route("GetProvisionalFees/{applicationId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetProvisionalFees(int applicationId)
        {
            try
            {
                var fees = await _preAdmissionService.GetProvisionalFees(applicationId);
                return Ok(fees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionController] GetProvisionalFees: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error retrieving provisional fees. " + ex.Message));
            }
        }

        // POST: api/Admissions/PreAdmissions/ProcessProvisionalFeePayment
        [HttpPost]
        [Authorize]
        [ActionName("ProcessProvisionalFeePayment")]
        [Route("ProcessProvisionalFeePayment")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ProcessProvisionalFeePayment([FromBody] ProvisionalFeePaymentRequest request)
        {
            try
            {
                var result = await _preAdmissionService.ProcessProvisionalFeePayment(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PreAdmissionController] ProcessProvisionalFeePayment: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing the payment. " + ex.Message));
            }
        }
    }
}
