using Application.ServiceContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/ExamMarks")]
    [ApiController]
    public class ExamMarksController : BaseController
    {
        private readonly IExamMarksService _examMarksService;
        private readonly ILogger<ExamMarksController> _logger;

        public ExamMarksController(
            ILogger<ExamMarksController> logger,
            IExamMarksService examMarksService)
        {
            _logger = logger;
            _examMarksService = examMarksService;
        }

        // GET: api/ExamMarks/GetMarksByExamSubject/{examSubjectId}
        [HttpGet]
        [ActionName("GetMarksByExamSubject")]
        [Route("GetMarksByExamSubject/{examSubjectId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetMarksByExamSubject(int examSubjectId)
        {
            try
            {
                var result = await _examMarksService.GetMarksByExamSubjectAsync(examSubjectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamMarksController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // GET: api/ExamMarks/GetMarksByStudent/{studentId}/{examId}
        [HttpGet]
        [ActionName("GetMarksByStudent")]
        [Route("GetMarksByStudent/{studentId}/{examId}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetMarksByStudent(int studentId, int examId)
        {
            try
            {
                var result = await _examMarksService.GetMarksByStudentAsync(studentId, examId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamMarksController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // POST: api/ExamMarks/CreateMarks
        [HttpPost]
        [ActionName("CreateMarks")]
        [Route("CreateMarks")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateMarks(ExamMarksCreateRequest model)
        {
            try
            {
                var marksModel = new ExamMarksModel
                {
                    ExamSubjectId = model.ExamSubjectId,
                    StudentId = model.StudentId,
                    TheoryMarks = model.TheoryMarks,
                    PracticalMarks = model.PracticalMarks,
                    IsAbsent = model.IsAbsent,
                    Remarks = model.Remarks,
                    Status = model.Status
                };

                var result = await _examMarksService.Add(marksModel);
                return Ok(BaseModel.Succeed(message: "Marks created successfully", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamMarksController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // POST: api/ExamMarks/BulkCreateMarks
        [HttpPost]
        [ActionName("BulkCreateMarks")]
        [Route("BulkCreateMarks")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> BulkCreateMarks(BulkExamMarksCreateRequest request)
        {
            try
            {
                var result = await _examMarksService.BulkCreateMarksAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamMarksController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // POST: api/ExamMarks/UpdateMarks
        [HttpPost]
        [ActionName("UpdateMarks")]
        [Route("UpdateMarks")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateMarks(ExamMarksUpdateRequest model)
        {
            try
            {
                var marksModel = new ExamMarksModel
                {
                    Id = model.Id,
                    ExamSubjectId = model.ExamSubjectId,
                    StudentId = model.StudentId,
                    TheoryMarks = model.TheoryMarks,
                    PracticalMarks = model.PracticalMarks,
                    IsAbsent = model.IsAbsent,
                    Remarks = model.Remarks,
                    Status = model.Status
                };

                await _examMarksService.Update(marksModel);
                return Ok(BaseModel.Succeed(message: "Marks updated successfully", data: marksModel));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamMarksController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // POST: api/ExamMarks/VerifyMarks
        [HttpPost]
        [ActionName("VerifyMarks")]
        [Route("VerifyMarks")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> VerifyMarks(VerifyMarksRequest request)
        {
            try
            {
                var result = await _examMarksService.VerifyMarksAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamMarksController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }

        // DELETE: api/ExamMarks/DeleteMarks/{id}
        [HttpDelete]
        [ActionName("DeleteMarks")]
        [Route("DeleteMarks/{id}")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteMarks(int id)
        {
            try
            {
                await _examMarksService.Delete(id);
                return Ok(BaseModel.Succeed(message: "Marks deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ExamMarksController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"Error: {ex.Message}"));
            }
        }
    }
}


