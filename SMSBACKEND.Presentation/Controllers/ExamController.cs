using Application.ServiceContracts;
using AutoMapper;
using Common.DTO.Request;
using Common.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Common.Utilities.Enums;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Exam")]
    [ApiController]
    public class ExamController : BaseController
    {
        private readonly IExamService _examService;
        private readonly IExamSubjectService _examSubjectService;
        private readonly ILogger<ExamController> _logger;
        private readonly IMapper _mapper;

        public ExamController(
            ILogger<ExamController> logger,
            IExamService examService,
            IExamSubjectService examSubjectService,
            IMapper mapper)
        {
            _logger = logger;
            _examService = examService;
            _examSubjectService = examSubjectService;
            _mapper = mapper;
        }

        #region Exam CRUD

        /// <summary>
        /// RESPONSE PATTERN DOCUMENTATION:
        /// - If service returns BaseModel: return Ok(result) directly (no double-wrapping)
        /// - If service returns entity/model: wrap in BaseModel.Succeed(data: result)
        /// This ensures consistent BaseModel structure in all responses
        /// </summary>

        // GET: api/Exam/GetExams
        [HttpGet]
        [ActionName("GetExams")]
        [Route("GetExams")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetExams(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<ExamRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;

                var (result, count, lastId) = await _examService.Get(
                    where: x => !x.IsDeleted &&
                        (!filterObj.ClassId.HasValue || x.ClassId == filterObj.ClassId) &&
                        (!filterObj.SectionId.HasValue || x.SectionId == filterObj.SectionId) &&
                        (!filterObj.Status.HasValue || x.Status == filterObj.Status) &&
                        (string.IsNullOrEmpty(filterObj.AcademicYear) || x.AcademicYear == filterObj.AcademicYear),
                    includeProperties: i => i.Include(x => x.Class).Include(x => x.Section),
                    orderBy: x => x.OrderByDescending(e => e.StartDate));

                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        // GET: api/Exam/GetExamById/{id}
        [HttpGet]
        [ActionName("GetExamById")]
        [Route("GetExamById/{id}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetExamById(int id)
        {
            try
            {
                // ✅ Validate input
                if (id <= 0)
                {
                    return BadRequest(BaseModel.Failed(message: "Invalid exam ID"));
                }

                var result = await _examService.GetExamWithDetailsAsync(id);
                
                if (result == null || !result.Success)
                {
                    return NotFound(BaseModel.Failed(message: "Exam not found"));
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamController.GetExamById] Error retrieving exam {ExamId}", id);
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        // POST: api/Exam/CreateExam
        [HttpPost]
        [ActionName("CreateExam")]
        [Route("CreateExam")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateExam(ExamCreateRequest model)
        {
            try
            {
                // ✅ Use AutoMapper instead of manual mapping
                var examModel = _mapper.Map<ExamModel>(model);
                var result = await _examService.Add(examModel);
                return Ok(BaseModel.Succeed(message: "Exam created successfully", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController.CreateExam] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        // POST: api/Exam/UpdateExam
        [HttpPost]
        [ActionName("UpdateExam")]
        [Route("UpdateExam")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateExam(ExamUpdateRequest model)
        {
            try
            {
                // ✅ Use AutoMapper instead of manual mapping
                var examModel = _mapper.Map<ExamModel>(model);
                await _examService.Update(examModel);
                return Ok(BaseModel.Succeed(message: "Exam updated successfully", data: examModel));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController.UpdateExam] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        // DELETE: api/Exam/DeleteExam/{id}
        [HttpDelete]
        [ActionName("DeleteExam")]
        [Route("DeleteExam/{id}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteExam(int id)
        {
            try
            {
                // ✅ Validate input
                if (id <= 0)
                {
                    return BadRequest(BaseModel.Failed(message: "Invalid exam ID"));
                }

                await _examService.Delete(id);
                return Ok(BaseModel.Succeed(message: "Exam deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamController.DeleteExam] Error deleting exam {ExamId}", id);
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        #endregion

        #region Additional Actions

        // POST: api/Exam/PublishExam
        [HttpPost]
        [ActionName("PublishExam")]
        [Route("PublishExam")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> PublishExam(ExamPublishRequest request)
        {
            try
            {
                var result = await _examService.PublishExamAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        // GET: api/Exam/GetExamsByClass/{classId}
        [HttpGet]
        [ActionName("GetExamsByClass")]
        [Route("GetExamsByClass/{classId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetExamsByClass(int classId, [FromQuery] int? sectionId = null)
        {
            try
            {
                var result = await _examService.GetExamsByClassAsync(classId, sectionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        // GET: api/Exam/GetExamsByAcademicYear/{academicYear}
        [HttpGet]
        [ActionName("GetExamsByAcademicYear")]
        [Route("GetExamsByAcademicYear/{academicYear}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetExamsByAcademicYear(string academicYear)
        {
            try
            {
                var result = await _examService.GetExamsByAcademicYearAsync(academicYear);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        // GET: api/Exam/GetExamsByStatus/{status}
        [HttpGet]
        [ActionName("GetExamsByStatus")]
        [Route("GetExamsByStatus/{status}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetExamsByStatus(ExamStatus status)
        {
            try
            {
                var result = await _examService.GetExamsByStatusAsync(status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        #endregion

        #region Exam Subjects

        // GET: api/Exam/GetExamSubjects/{examId}
        [HttpGet]
        [ActionName("GetExamSubjects")]
        [Route("GetExamSubjects/{examId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetExamSubjects(int examId)
        {
            try
            {
                var result = await _examSubjectService.GetSubjectsByExamAsync(examId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        // POST: api/Exam/BulkCreateExamSubjects
        [HttpPost]
        [ActionName("BulkCreateExamSubjects")]
        [Route("BulkCreateExamSubjects")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> BulkCreateExamSubjects(BulkExamSubjectCreateRequest request)
        {
            try
            {
                var result = await _examSubjectService.BulkCreateExamSubjectsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request. {ex.Message}"));
            }
        }

        #endregion
    }
}


