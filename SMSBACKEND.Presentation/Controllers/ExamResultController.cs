using Application.ServiceContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/ExamResult")]
    [ApiController]
    public class ExamResultController : BaseController
    {
        private readonly IExamResultService _examResultService;
        private readonly ILogger<ExamResultController> _logger;

        public ExamResultController(
            ILogger<ExamResultController> logger,
            IExamResultService examResultService)
        {
            _logger = logger;
            _examResultService = examResultService;
        }

        // GET: api/ExamResult/GetResultsByExam/{examId}
        [HttpGet]
        [ActionName("GetResultsByExam")]
        [Route("GetResultsByExam/{examId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetResultsByExam(int examId)
        {
            try
            {
                var result = await _examResultService.GetResultsByExamAsync(examId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamResultController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // GET: api/ExamResult/GetStudentResult/{examId}/{studentId}
        [HttpGet]
        [ActionName("GetStudentResult")]
        [Route("GetStudentResult/{examId}/{studentId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetStudentResult(int examId, int studentId)
        {
            try
            {
                var result = await _examResultService.GetStudentExamResultAsync(examId, studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamResultController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // POST: api/ExamResult/GenerateResults
        [HttpPost]
        [ActionName("GenerateResults")]
        [Route("GenerateResults")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GenerateResults(GenerateExamResultRequest request)
        {
            try
            {
                var result = await _examResultService.GenerateExamResultsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamResultController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // POST: api/ExamResult/PublishResults
        [HttpPost]
        [ActionName("PublishResults")]
        [Route("PublishResults")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> PublishResults(PublishExamResultRequest request)
        {
            try
            {
                var result = await _examResultService.PublishResultsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamResultController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // GET: api/ExamResult/GetTopRankers/{examId}
        [HttpGet]
        [ActionName("GetTopRankers")]
        [Route("GetTopRankers/{examId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetTopRankers(int examId, [FromQuery] int topCount = 10)
        {
            try
            {
                var result = await _examResultService.GetTopRankersAsync(examId, topCount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamResultController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // POST: api/ExamResult/GenerateReportCard
        [HttpPost]
        [ActionName("GenerateReportCard")]
        [Route("GenerateReportCard")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GenerateReportCard(StudentReportCardRequest request)
        {
            try
            {
                var result = await _examResultService.GenerateReportCardAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamResultController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }
    }
}


