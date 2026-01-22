using Application.Interfaces;
using Application.ServiceContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.Extensions;
using Common.Helpers;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Presentation.Filters;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Staff")]
    [ApiController]
    public class StaffController : BaseController
    {
        private readonly IDepartmentService _departmentService;
        private readonly IStaffService _staffService;
        private readonly ILogger<StaffController> _logger;
        public StaffController(ILogger<StaffController> logger, IDepartmentService departmentService, IStaffService staffService)
        {
            _logger = logger;
            _departmentService = departmentService;
            _staffService = staffService;
        }


        #region public actions

        #region Staff
        // GET: Api/Staff/GetAllStaff
        [HttpGet]
        [Authorize]
        [ActionName("GetAllStaff")]
        [Route("GetAllStaff")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllStaff(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonSerializerHelper.Deserialize<StaffRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var result = await _staffService.GetAllStaff(filterObj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StaffController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Staff/CreateStaff
        [HttpPost]
        [Authorize]
        [ActionName("CreateStaff")]
        [Route("CreateStaff")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateStaff([FromBody] StaffCreateReq model)
        {
            try
            {
                var result = await _staffService.CreateStaff(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StaffController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Create(success: false, message: ex.InnerException.Message));

            }
        }

        // POST: Api/Staff/UpdateStaff
        [HttpPost]
        [AllowAnonymous]
        [ActionName("UpdateStaff")]
        [Route("UpdateStaff")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateStaff([FromBody] StaffCreateReq model)
        {
            try
            {
                var result = await _staffService.UpdateStaff(model);
                // ✅ Service already returns BaseModel - don't wrap it again
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StaffController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Create(success: false, message: ex.InnerException.Message));

            }
        }

        // Delete: Api/Staff/DeleteStaff
        [HttpDelete]
        [ActionName("DeleteStaff")]
        [Route("DeleteStaff")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteStaff([FromQuery] int id)
        {    
            try
            {
                var result = await _staffService.DeleteStaff(id);
                return result.Message == "Succesful" ? new OkObjectResult(result) : NotFound();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again. " +ex.Message));
                throw;
            }
           
        }
        #endregion

        #region Department
        // GET: Api/Staff/GetDepartments
        [HttpGet]
        [Authorize]
        [ActionName("GetDepartments")]
        [Route("GetDepartments")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetDepartments(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<DepartmentRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var (result, count, lastId) = await _departmentService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StaffController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }
        // GET: Api/Staff/CreateDepartment
        [HttpPost]
        [Authorize]
        [ActionName("CreateDepartment")]
        [Route("CreateDepartment")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateDepartment(DepartmentModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _departmentService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Department {result.Name}" : $"Department {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _departmentService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _departmentService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Department {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StaffController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }
        // GET: Api/Staff/UpdateDepartment
        [HttpPost]
        [Authorize]
        [ActionName("UpdateDepartment")]
        [Route("UpdateDepartment")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateDepartment(DepartmentModel model)
        {
            try
            {
                await _departmentService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Department {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[StaffController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException.Message}"));
            }
        }
        // Delete: Api/Staff/DeleteDepartment
        [HttpDelete]
        [ActionName("DeleteDepartment")]
        [Route("DeleteDepartment")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteDepartment([FromQuery] int id)
        {
            try
            {
                var result = await _departmentService.FirstOrDefaultAsync(x => x.Id == id);
                if (result != null)
                {
                    await _departmentService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Successfully Deleted Department {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.InnerException?.Message ?? ex.Message}"));
            }
        }
        #endregion

        #endregion
    }
}
