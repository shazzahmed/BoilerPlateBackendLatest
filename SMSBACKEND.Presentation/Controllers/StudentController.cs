using Application.ServiceContracts;
using Application.Interfaces;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.Helpers;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IClassService _classService;
        private readonly IStudentCategoryService _studentCategoryService;
        private readonly ISchoolHouseService _schoolHouseService;
        private readonly IAdmissionService _admissionService;
        private readonly ILogger<StudentController> _logger;
        public StudentController(
                ILogger<StudentController> logger,
                IStudentService studentService,
                IClassService classService,
                IStudentCategoryService studentCategoryService,
                ISchoolHouseService schoolHouseService,
                IAdmissionService admissionService)
        {
            _logger = logger;
            _studentService = studentService;
            _classService = classService;
            _studentCategoryService = studentCategoryService;
            _schoolHouseService = schoolHouseService;
            _admissionService = admissionService;
        }

        // GET: Api/Students/GetAllStudents
        [HttpGet]
        [Authorize]
        [ActionName("GetAllStudents")]
        [Route("GetAllStudents")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllStudents(
            [FromQuery] string filters)
        {
            try
            {
                var filterObj = JsonSerializerHelper.Deserialize<StudentRequest>(filters);
                // Removed pagination parameters - frontend will handle pagination locally
                var result = await _studentService.GetAll(filterObj);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] GetAllStudents: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Students/CreateStudent (Unified endpoint for all flows)
        [HttpPost]
        [Authorize]
        [ActionName("CreateStudent")]
        [Route("CreateStudent")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateRequest model)
        {
            try
            {
                var result = await _studentService.CreateStudent(model);
                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] CreateStudent: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }



        // GET: Api/Students/{studentId}/InitialFeePackages
        [HttpGet]
        [Authorize]
        [Route("{studentId}/InitialFeePackages")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetStudentInitialFeePackages(int studentId)
        {
            try
            {
                var result = await _studentService.GetStudentInitialFeePackages(studentId);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] GetStudentInitialFeePackages: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "Error loading initial fee packages"));
            }
        }

        // GET: Api/Students/{id} - Get student details for view page
        [HttpGet]
        [Authorize]
        [Route("{id}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetStudentById(int id)
        {
            try
            {
                var result = await _studentService.GetStudentById(id);
                if (result.Success)
                {
                    return new OkObjectResult(result);
                }
                return new BadRequestObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] GetStudentById: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "Error retrieving student details"));
            }
        }

        // GET: Api/Students/GetNextAdmissionNumber
        [HttpGet]
        [Authorize]
        [ActionName("GetNextAdmissionNumber")]
        [Route("GetNextAdmissionNumber")]
        [Produces("application/json", Type = typeof(string))]
        public async Task<IActionResult> GetNextAdmissionNumber()
        {
            try
            {
                var admissionNumber = await _studentService.GetNextAdmissionNumberAsync();
                return new OkObjectResult(new { admissionNumber });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] GetNextAdmissionNumber: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error generating admission number, please try again."));
            }
        }

        // POST: Api/Students/UpdateStudent
        [HttpPost]
        [Authorize]
        [ActionName("UpdateStudent")]
        [Route("UpdateStudent")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateStudent([FromBody] StudentCreateRequest model)
        {
            try
            {
                var result = await _studentService.UpdateStudent(model);
                if (result.Success)
                {
                    return new OkObjectResult(result);
                }
                return new BadRequestObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] UpdateStudent: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // DELETE: Api/Students/DeleteStudent
        [HttpDelete]
        [Authorize]
        [ActionName("DeleteStudent")]
        [Route("DeleteStudent")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteStudent([FromQuery] int id)
        {
            try
            {
                var result = await _studentService.DeleteStudent(id);
                if (result.Success)
                {
                    return new OkObjectResult(result);
                }
                return new BadRequestObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] DeleteStudent: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Students/RestoreStudent
        [HttpPost]
        [Authorize]
        [ActionName("RestoreStudent")]
        [Route("RestoreStudent")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> RestoreStudent([FromQuery] int id)
        {
            try
            {
                var result = await _studentService.RestoreStudent(id);
                if (result.Success)
                {
                    return new OkObjectResult(result);
                }
                return new BadRequestObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] RestoreStudent: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

       
        // POST: Api/Students/ImportStudents
        [HttpPost]
        [Authorize]
        [ActionName("ImportStudents")]
        [Route("ImportStudents")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public IActionResult ImportStudents([FromBody] object data)
        {
            try
            {
                if (data == null)
                {
                    return BadRequest("No data provided");
                }
                _studentService.BatchImportStudents(data);
                return new ObjectResult(BaseModel.Succeed(message: "Students imported successfully", data: "", total: 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] ImportStudents: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Students/ValidateParent
        [HttpPost]
        [Authorize]
        [ActionName("ValidateParent")]
        [Route("ValidateParent")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ParentValidationResponse))]
        public async Task<IActionResult> ValidateParent([FromBody] ParentValidationRequest request)
        {
            try
            {
                var result = await _studentService.ValidateParentAsync(request);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] ValidateParent: {ex.Message}");
                return new BadRequestObjectResult(new ParentValidationResponse 
                { 
                    IsValid = false, 
                    Message = "There was an error validating parent information, please try again." 
                });
            }
        }

        // POST: Api/Students/ValidateStudent
        [HttpPost]
        [Authorize]
        [ActionName("ValidateStudent")]
        [Route("ValidateStudent")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(StudentValidationResponse))]
        public async Task<IActionResult> ValidateStudent([FromBody] StudentValidationRequest request)
        {
            try
            {
                var result = await _studentService.ValidateStudentAsync(request);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] ValidateStudent: {ex.Message}");
                return new BadRequestObjectResult(new StudentValidationResponse 
                { 
                    IsValid = false, 
                    Message = "There was an error validating student information, please try again." 
                });
            }
        }

        // POST: api/Students/CreateStudentFromAdmission (Simplified using unified approach)
        [HttpPost]
        [Authorize]
        [ActionName("CreateStudentFromAdmission")]
        [Route("CreateStudentFromAdmission")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateStudentFromAdmission([FromBody] StudentCreateRequest request)
        {
            try
            {
                // Set source to admission
                request.Source = "admission";
                
                // Create student using unified method
                var result = await _studentService.CreateStudent(request);
                
                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] CreateStudentFromAdmission: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error creating student from admission: " + ex.Message));
            }
        }

        // ==================== BULK IMPORT ENDPOINTS ====================

        // POST: api/Students/StartBulkImport
        [HttpPost]
        [ActionName("StartBulkImport")]
        [Route("StartBulkImport")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> StartBulkImport([FromBody] BulkStudentImportRequest request)
        {
            try
            {
                var result = await _studentService.StartBulkImportAsync(request);
                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] StartBulkImport: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error starting bulk import: " + ex.Message));
            }
        }

        // GET: api/Students/GetImportStatus/{jobId}
        [HttpGet]
        [Authorize]
        [ActionName("GetImportStatus")]
        [Route("GetImportStatus/{jobId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetImportStatus(string jobId)
        {
            try
            {
                var result = await _studentService.GetImportStatusAsync(jobId);
                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] GetImportStatus: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error getting import status: " + ex.Message));
            }
        }

        // GET: api/Students/ImportProgress/{jobId}
        [HttpGet]
        [AllowAnonymous]
        [ActionName("ImportProgress")]
        [Route("ImportProgress/{jobId}")]
        [Produces("text/event-stream")]
        public async Task ImportProgress(string jobId)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Headers", "Cache-Control");

            try
            {
                await _studentService.StreamImportProgressAsync(jobId, Response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] ImportProgress: {ex.Message}");
                await Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(new { type = "error", message = ex.Message })}\n\n");
            }
            finally
            {
                await Response.Body.FlushAsync();
            }
        }

        // POST: api/Students/CancelImport/{jobId}
        [HttpPost]
        [Authorize]
        [ActionName("CancelImport")]
        [Route("CancelImport/{jobId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CancelImport(string jobId)
        {
            try
            {
                var result = await _studentService.CancelImportJobAsync(jobId);
                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] CancelImport: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error cancelling import: " + ex.Message));
            }
        }

        // GET: api/Students/GetAllImportJobs
        [HttpGet]
        [Authorize]
        [ActionName("GetAllImportJobs")]
        [Route("GetAllImportJobs")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllImportJobs()
        {
            try
            {
                var result = await _studentService.GetAllImportJobsAsync();
                return new ObjectResult(new BaseModel 
                { 
                    Success = true, 
                    Data = result, 
                    Message = "Import jobs retrieved successfully" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] GetAllImportJobs: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error getting import jobs: " + ex.Message));
            }
        }

        // GET: api/Students/DownloadImportTemplate
        [HttpGet]
        [Authorize]
        [ActionName("DownloadImportTemplate")]
        [Route("DownloadImportTemplate")]
        [Produces("text/csv")]
        public async Task<IActionResult> DownloadImportTemplate()
        {
            try
            {
                var result = await _studentService.DownloadImportTemplateAsync();
                return File(result, "text/csv", "student_import_template.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] DownloadImportTemplate: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error downloading template: " + ex.Message));
            }
        }

        // POST: api/Students/ParseImportFile
        [HttpPost]
        [Authorize]
        [ActionName("ParseImportFile")]
        [Route("ParseImportFile")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ParseImportFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult(BaseModel.Failed(message: "No file uploaded"));
                }

                var result = await _studentService.ParseImportFileAsync(file);
                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] ParseImportFile: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error parsing file: " + ex.Message));
            }
        }

        // GET: Api/Students/GetParentByCNIC?cnic=123
        [HttpGet]
        [Authorize]
        [ActionName("GetParentByCNIC")]
        [Route("GetParentByCNIC")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetParentByCNIC([FromQuery] string cnic)
        {
            try
            {
                var result = await _studentService.GetParentDataByCNIC(cnic);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StudentController] GetParentByCNIC: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "Error fetching parent data: " + ex.Message));
            }
        }

    }
}
