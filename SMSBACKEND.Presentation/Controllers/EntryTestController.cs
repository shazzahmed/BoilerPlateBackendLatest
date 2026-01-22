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
    [Route("api/Admissions/EntryTests")]
    [ApiController]
    public class EntryTestController : ControllerBase
    {
        private readonly IEntryTestService _entryTestService;
        private readonly ILogger<EntryTestController> _logger;

        public EntryTestController(
            IEntryTestService entryTestService,
            ILogger<EntryTestController> logger)
        {
            _entryTestService = entryTestService;
            _logger = logger;
        }

        // GET: api/Admissions/EntryTests/GetAllEntryTests
        [HttpGet]
        [Authorize]
        [ActionName("GetAllEntryTests")]
        [Route("GetAllEntryTests")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllEntryTests()
        {
            try
            {
                var (result, count, lastId) = await _entryTestService.Get(
                    null, 
                    orderBy: x => x.OrderByDescending(x => x.Id), 
                    includeProperties: i => i
                        .Include(c => c.Class)
                        .Include(e => e.Application)
                            .ThenInclude(a => a.Enquiry)
                        .Include(e => e.Admission), // ✅ Include workflow tracking
                    useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EntryTestController] GetAllEntryTests: {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: api/Admissions/EntryTests/CreateEntryTest
        [HttpPost]
        [Authorize]
        [ActionName("CreateEntryTest")]
        [Route("CreateEntryTest")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateEntryTest([FromBody] EntryTestModel model)
        {
            try
            {
                var result = await _entryTestService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Entry Test {result.TestNumber}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EntryTestController] CreateEntryTest: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // PUT: api/Admissions/EntryTests/UpdateEntryTest
        [HttpPost]
        [Authorize]
        [ActionName("UpdateEntryTest")]
        [Route("UpdateEntryTest")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateEntryTest([FromBody] EntryTestModel model)
        {
            try
            {
                await _entryTestService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Entry Test {model.TestNumber}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EntryTestController] UpdateEntryTest: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }

        // DELETE: api/Admissions/EntryTests/DeleteEntryTest
        [HttpDelete]
        [Authorize]
        [ActionName("DeleteEntryTest")]
        [Route("DeleteEntryTest")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteEntryTest([FromQuery] int id)
        {
            try
            {
                var result = await _entryTestService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _entryTestService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Entry Test {result.TestNumber}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EntryTestController] DeleteEntryTest: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
            }
        }
    }
}