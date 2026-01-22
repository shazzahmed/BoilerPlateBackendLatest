using Application.ServiceContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.Helpers;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Presentation.Filters;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Admissions/Enquiries")]
    [ApiController]
    public class EnquiryController : ControllerBase
    {
        private readonly IEnquiryService _enquiryService;
        private readonly ILogger<EnquiryController> _logger;

        public EnquiryController(
            IEnquiryService enquiryService,
            ILogger<EnquiryController> logger)
        {
            _enquiryService = enquiryService;
            _logger = logger;
        }

        // GET: api/Admissions/Enquiries/GetAllEnquiries
        [HttpGet]
        [Authorize]
        [ActionName("GetAllEnquiries")]
        [Route("GetAllEnquiries")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllEnquiries()
        {
            try
            {
                // ✅ Include full workflow navigation properties for progress tracking
                var (result, count, lastId) = await _enquiryService.Get(
                    null, 
                    orderBy: x => x.OrderByDescending(x => x.Id), 
                    includeProperties: i => i
                        .Include(x => x.Class)
                        .Include(x => x.PreAdmission)
                            .ThenInclude(p => p.EntryTest)
                                .ThenInclude(e => e.Admission), 
                    useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] GetAllEnquiries: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: api/Admissions/Enquiries/CreateEnquiry
        [HttpPost]
        [Authorize]
        [ActionName("CreateEnquiry")]
        [Route("CreateEnquiry")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateEnquiry([FromBody] EnquiryModel model)
        {
            try
            {
                var result = await _enquiryService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Enquiry {result.FullName}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] CreateEnquiry: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // PUT: api/Admissions/Enquiries/UpdateEnquiry
        [HttpPost]
        [Authorize]
        [ActionName("UpdateEnquiry")]
        [Route("UpdateEnquiry")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateEnquiry([FromBody] EnquiryModel model)
        {
            try
            {
                await _enquiryService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Enquiry {model.FullName}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] UpdateEnquiry: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // DELETE: api/Admissions/Enquiries/DeleteEnquiry
        [HttpDelete]
        [Authorize]
        [ActionName("DeleteEnquiry")]
        [Route("DeleteEnquiry")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteEnquiry([FromQuery] int id)
        {
            try
            {
                var result = await _enquiryService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _enquiryService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Enquiry {result.FullName}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] DeleteEnquiry: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
            }
        }

        // POST: api/Admissions/Enquiries/ValidateBulkImport
        [HttpPost]
        [Authorize]
        [ActionName("ValidateBulkImport")]
        [Route("ValidateBulkImport")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ValidateBulkImport([FromBody] BulkEnquiryImportRequest request)
        {
            try
            {
                var result = await _enquiryService.ValidateBulkImportAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] ValidateBulkImport: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error validating the import data: {ex.Message}"));
            }
        }

        // POST: api/Admissions/Enquiries/ImportBulk
        [HttpPost]
        [Authorize]
        [ActionName("ImportBulk")]
        [Route("ImportBulk")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ImportBulk([FromBody] BulkEnquiryImportRequest request)
        {
            try
            {
                var result = await _enquiryService.ImportEnquiriesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] ImportBulk: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error starting the import process: {ex.Message}"));
            }
        }

        // GET: api/Admissions/Enquiries/ImportStatus/{jobId}
        [HttpGet]
        [Authorize]
        [ActionName("ImportStatus")]
        [Route("ImportStatus/{jobId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetImportStatus(string jobId)
        {
            try
            {
                var result = await _enquiryService.GetImportStatusAsync(jobId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] GetImportStatus: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error retrieving import status: {ex.Message}"));
            }
        }

        // GET: api/Admissions/Enquiries/ImportProgress/{jobId} - Server-Sent Events for real-time progress
        [HttpGet]
        [Authorize]
        [ActionName("ImportProgress")]
        [Route("ImportProgress/{jobId}")]
        public async Task<IActionResult> GetImportProgress(string jobId)
        {
            try
            {
                Response.Headers.Add("Content-Type", "text/event-stream");
                Response.Headers.Add("Cache-Control", "no-cache");
                Response.Headers.Add("Connection", "keep-alive");
                Response.Headers.Add("Access-Control-Allow-Origin", "*");

                var cancellationToken = HttpContext.RequestAborted;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var status = await _enquiryService.GetImportStatusAsync(jobId);
                    
                    if (status.Success && status.Data != null)
                    {
                        var statusData = (ImportJobStatus)status.Data;
                        var isCompleted = statusData.Status == "Completed" || statusData.Status == "Failed";
                        
                        var sseData = $"data: {System.Text.Json.JsonSerializer.Serialize(statusData)}\n\n";
                        await Response.WriteAsync(sseData);
                        await Response.Body.FlushAsync();
                        
                        if (isCompleted)
                        {
                            break;
                        }
                    }
                    
                    await Task.Delay(1000, cancellationToken); // Check every second
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] GetImportProgress: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Failed to stream import progress: {ex.Message}"));
            }
        }

        // GET: api/Admissions/Enquiries/ImportHistory
        [HttpGet]
        [Authorize]
        [ActionName("ImportHistory")]
        [Route("ImportHistory")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetImportHistory()
        {
            try
            {
                var result = await _enquiryService.GetImportHistoryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] GetImportHistory: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error retrieving import history: {ex.Message}"));
            }
        }

        // GET: api/Admissions/Enquiries/DownloadTemplate
        [HttpGet]
        [Authorize]
        [ActionName("DownloadTemplate")]
        [Route("DownloadTemplate")]
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                var templateData = await _enquiryService.DownloadTemplateAsync();
                var fileName = $"EnquiryImportTemplate_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                return File(templateData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] DownloadTemplate: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error generating the template: {ex.Message}"));
            }
        }

        // POST: api/Admissions/Enquiries/ProcessWorkflow
        [HttpPost]
        [Authorize]
        [ActionName("ProcessWorkflow")]
        [Route("ProcessWorkflow")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ProcessImportedEnquiriesWorkflow([FromBody] ProcessWorkflowRequest request)
        {
            try
            {
                var result = await _enquiryService.ProcessImportedEnquiriesWorkflowAsync(request.EnquiryIds, request.ProcessedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] ProcessImportedEnquiriesWorkflow: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing workflow: {ex.Message}"));
            }
        }

        // GET: api/Admissions/Enquiries/WorkflowStats
        [HttpGet]
        [Authorize]
        [ActionName("WorkflowStats")]
        [Route("WorkflowStats")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetImportedEnquiriesWorkflowStats()
        {
            try
            {
                var result = await _enquiryService.GetImportedEnquiriesWorkflowStatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EnquiryController] GetImportedEnquiriesWorkflowStats: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error retrieving workflow statistics: {ex.Message}"));
            }
        }
    }
}
