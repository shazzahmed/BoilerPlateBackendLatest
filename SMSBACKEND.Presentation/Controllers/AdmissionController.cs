using Application.ServiceContracts;
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
    [Route("api/Admissions/Admissions")]
    [ApiController]
    public class AdmissionController : ControllerBase
    {
        private readonly IAdmissionService _admissionService;
        private readonly ILogger<AdmissionController> _logger;

        public AdmissionController(
            IAdmissionService admissionService,
            ILogger<AdmissionController> logger)
        {
            _admissionService = admissionService;
            _logger = logger;
        }

        // GET: api/Admissions/Admissions/GetAllAdmissions
        [HttpGet]
        [Authorize]
        [ActionName("GetAllAdmissions")]
        [Route("GetAllAdmissions")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllAdmissions()
        {
            try
            {
                var (result, count, lastId) = await _admissionService.Get(null, orderBy: x => x.OrderByDescending(x => x.Id), includeProperties: i => i.Include(c => c.Class).Include(s => s.Section).Include(et => et.EntryTest).Include(et => et.Student).ThenInclude(x=> x.StudentInfo), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] GetAllAdmissions: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: api/Admissions/Admissions/GetAdmissionById/{id}
        [HttpGet]
        [Authorize]
        [ActionName("GetAdmissionById")]
        [Route("GetAdmissionById/{id}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAdmissionById(int id)
        {
            try
            {
                var result = await _admissionService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result == null)
                {
                    return NotFound(BaseModel.Failed(message: "Admission not found"));
                }
                return Ok(BaseModel.Succeed(data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] GetAdmissionById: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: api/Admissions/Admissions/CreateAdmission
        [HttpPost]
        [Authorize]
        [ActionName("CreateAdmission")]
        [Route("CreateAdmission")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateAdmission([FromBody] AdmissionModel model)
        {
            try
            {
                var result = await _admissionService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Successfully Added Admission {result.AdmissionNumber}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] CreateAdmission: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // PUT: api/Admissions/Admissions/UpdateAdmission
        [HttpPost]
        [Authorize]
        [ActionName("UpdateAdmission")]
        [Route("UpdateAdmission")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateAdmission([FromBody] AdmissionModel model)
        {
            try
            {
                await _admissionService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Successfully Updated Admission {model.AdmissionNumber}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] UpdateAdmission: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // DELETE: api/Admissions/Admissions/DeleteAdmission
        [HttpDelete]
        [Authorize]
        [ActionName("DeleteAdmission")]
        [Route("DeleteAdmission")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteAdmission([FromQuery] int id)
        {
            try
            {
                var result = await _admissionService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _admissionService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Successfully Deleted Admission {result.AdmissionNumber}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] DeleteAdmission: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
            }
        }

        // GET: api/Admissions/Admissions/GetAdmissionsByEntryTest/{entryTestId}
        [HttpGet]
        [Authorize]
        [ActionName("GetAdmissionsByEntryTest")]
        [Route("GetAdmissionsByEntryTest/{entryTestId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAdmissionsByEntryTest(int entryTestId)
        {
            try
            {
                var (result, count, lastId) = await _admissionService.Get(x => x.EntryTestId == entryTestId, orderBy: x => x.OrderByDescending(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] GetAdmissionsByEntryTest: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: api/Admissions/Admissions/CanCreateStudent/{admissionId}
        [HttpGet]
        [Authorize]
        [ActionName("CanCreateStudent")]
        [Route("CanCreateStudent/{admissionId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CanCreateStudent(int admissionId)
        {
            try
            {
                var canCreate = await _admissionService.CanCreateStudent(admissionId);
                return Ok(BaseModel.Succeed(data: canCreate, message: canCreate ? "Student can be created" : "Student already created or admission not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] CanCreateStudent: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error checking if student can be created."));
            }
        }

        // GET: api/Admissions/Admissions/GetAdmissionWithStudent/{admissionId}
        [HttpGet]
        [Authorize]
        [ActionName("GetAdmissionWithStudent")]
        [Route("GetAdmissionWithStudent/{admissionId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAdmissionWithStudent(int admissionId)
        {
            try
            {
                var result = await _admissionService.GetAdmissionWithStudent(admissionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] GetAdmissionWithStudent: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error retrieving admission with student information."));
            }
        }

        // GET: api/Admissions/Admissions/GetNextAdmissionNumber
        [HttpGet]
        [Authorize]
        [ActionName("GetNextAdmissionNumber")]
        [Route("GetNextAdmissionNumber")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetNextAdmissionNumber()
        {
            try
            {
                var admissionNumber = await _admissionService.GetNextAdmissionNumberAsync();
                return Ok(BaseModel.Succeed(data: admissionNumber, message: "Next admission number generated"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] GetNextAdmissionNumber: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error generating next admission number."));
            }
        }

        // GET: api/Admissions/Admissions/GetAdmissionWithWorkflowData/{admissionId}
        [HttpGet]
        [Authorize]
        [ActionName("GetAdmissionWithWorkflowData")]
        [Route("GetAdmissionWithWorkflowData/{admissionId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAdmissionWithWorkflowData(int admissionId)
        {
            try
            {
                var result = await _admissionService.GetAdmissionWithWorkflowData(admissionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdmissionController] GetAdmissionWithWorkflowData: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error retrieving admission with workflow data."));
            }
        }

    }
}
