using System;
using System.Threading.Tasks;
using Application.ServiceContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Filters;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("Api/[controller]")]
    [ApiController]
    public class FineWaiverController : ControllerBase
    {
        private readonly IFineWaiverService _fineWaiverService;
        private readonly ILogger<FineWaiverController> _logger;

        public FineWaiverController(ILogger<FineWaiverController> logger, IFineWaiverService fineWaiverService)
        {
            _logger = logger;
            _fineWaiverService = fineWaiverService;
        }

        #region Fine Waiver Management

        // POST: Api/FineWaiver/RequestWaiver
        [HttpPost]
        [Authorize]
        [ActionName("RequestWaiver")]
        [Route("RequestWaiver")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> RequestWaiver(FineWaiverRequest model)
        {
            try
            {
                var result = await _fineWaiverService.RequestFineWaiver(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FineWaiverController] RequestWaiver error: {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // POST: Api/FineWaiver/ApproveWaiver
        [HttpPost]
        [Authorize] // Temporarily removed role check for testing
        [ActionName("ApproveWaiver")]
        [Route("ApproveWaiver")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ApproveWaiver(FineWaiverApprovalRequest model)
        {
            try
            {
                var result = await _fineWaiverService.ApproveFineWaiver(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FineWaiverController] ApproveWaiver error: {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // GET: Api/FineWaiver/GetAllFineWaivers
        [HttpGet]
        [Authorize]
        [ActionName("GetAllFineWaivers")]
        [Route("GetAllFineWaivers")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllFineWaivers()
        {
            try
            {
                // Use custom method that includes navigation properties
                var result = await _fineWaiverService.GetWaiversByStatus("All");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FineWaiverController] GetAllFineWaivers error: {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // GET: Api/FineWaiver/GetPendingWaivers
        [HttpGet]
        [Authorize] // Temporarily removed role check for testing
        [ActionName("GetPendingWaivers")]
        [Route("GetPendingWaivers")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetPendingWaivers()
        {
            try
            {
                var result = await _fineWaiverService.GetPendingWaivers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FineWaiverController] GetPendingWaivers error: {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // GET: Api/FineWaiver/GetWaiversByStudent/{studentId}
        [HttpGet]
        [Authorize]
        [ActionName("GetWaiversByStudent")]
        [Route("GetWaiversByStudent/{studentId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetWaiversByStudent(int studentId)
        {
            try
            {
                var result = await _fineWaiverService.GetWaiversByStudent(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FineWaiverController] GetWaiversByStudent error: {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // GET: Api/FineWaiver/GetWaiversByStatus/{status}
        [HttpGet]
        [Authorize] // Temporarily removed role check for testing
        [ActionName("GetWaiversByStatus")]
        [Route("GetWaiversByStatus/{status}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetWaiversByStatus(string status)
        {
            try
            {
                var result = await _fineWaiverService.GetWaiversByStatus(status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FineWaiverController] GetWaiversByStatus error: {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // GET: Api/FineWaiver/GetById/{id}
        [HttpGet]
        [Authorize]
        [ActionName("GetById")]
        [Route("GetById/{id}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _fineWaiverService.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (result != null)
                {
                    return Ok(BaseModel.Succeed(data: result, message: "Fine waiver retrieved successfully"));
                }
                return NotFound(BaseModel.Failed(message: "Fine waiver not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FineWaiverController] GetById error: {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // DELETE: Api/FineWaiver/DeleteFineWaiver
        [HttpDelete]
        [Authorize] // Temporarily removed role check for testing
        [ActionName("DeleteFineWaiver")]
        [Route("DeleteFineWaiver")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteFineWaiver([FromQuery] int id)
        {
            try
            {
                var result = await _fineWaiverService.FirstOrDefaultAsync(x => x.Id == id);
                if (result != null)
                {
                    result.IsDeleted = true;
                    await _fineWaiverService.Update(result);
                    return Ok(BaseModel.Succeed(message: $"Fine waiver cancelled successfully", data: result));
                }
                return NotFound(BaseModel.Failed(message: "Fine waiver not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[FineWaiverController] DeleteFineWaiver error: {ex}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        #endregion
    }
}

