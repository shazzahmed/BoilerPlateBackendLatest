using Application.ServiceContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Admin")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly IExpenseHeadService _expenseHeadService;
        private readonly IExpenseService _expenseService;
        private readonly IIncomeHeadService _incomeHeadService;
        private readonly IIncomeService _incomeService;
        private readonly ILogger<AdminController> _logger;
        public AdminController(ILogger<AdminController> logger, IExpenseHeadService expenseHeadService, IExpenseService expenseService, IIncomeHeadService incomeHeadService, IIncomeService incomeService)
        {
            _logger = logger;
            _expenseHeadService = expenseHeadService;
            _expenseService = expenseService;
            _incomeHeadService = incomeHeadService;
            _incomeService = incomeService;
        }

        #region ExpenseHead
        // GET: Api/Admin/GetAllExpenseHead
        [HttpGet]
        [Authorize]
        [ActionName("GetAllExpenseHead")]
        [Route("GetAllExpenseHead")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllExpenseHead(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var (result, count, lastId) = await _expenseHeadService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.Message}"));
            }
        }
        // GET: Api/Admin/CreateExpenseHead
        [HttpPost]
        [Authorize]
        [ActionName("CreateExpenseHead")]
        [Route("CreateExpenseHead")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateExpenseHead(ExpenseHeadModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _expenseHeadService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Expense Head {result.Name}" : $"Expense Head {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _expenseHeadService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _expenseHeadService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Expense Head {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.Message}"));
            }
        }
        // GET: Api/Admin/UpdateExpenseHead
        [HttpPost]
        [Authorize]
        [ActionName("UpdateExpenseHead")]
        [Route("UpdateExpenseHead")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateExpenseHead(ExpenseHeadModel model)
        {
            try
            {
                await _expenseHeadService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Expense Head {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Admin/DeleteExpenseHead
        [HttpDelete]
        [ActionName("DeleteExpenseHead")]
        [Route("DeleteExpenseHead")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteExpenseHead([FromQuery] int id)
        {
            try
            {
                var result = await _expenseHeadService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _expenseHeadService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Expense Head {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        #endregion

        #region Expense
        // GET: Api/Admin/GetAllExpense
        [HttpGet]
        [Authorize]
        [ActionName("GetAllExpense")]
        [Route("GetAllExpense")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllExpense(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var (result, count, lastId) = await _expenseService.Get(null, orderBy: x => x.OrderBy(x => x.Id), includeProperties: i => i.Include(x => x.ExpenseHead), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // GET: Api/Admin/CreateExpense
        [HttpPost]
        [Authorize]
        [ActionName("CreateExpense")]
        [Route("CreateExpense")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateExpense(ExpenseModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _expenseService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Expense {result.Name}" : $"Expense {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _expenseService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _expenseService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Expense {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // GET: Api/Admin/UpdateExpense
        [HttpPost]
        [Authorize]
        [ActionName("UpdateExpense")]
        [Route("UpdateExpense")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateExpense(ExpenseModel model)
        {
            try
            {
                await _expenseService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Expense {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Admin/DeleteExpense
        [HttpDelete]
        [ActionName("DeleteExpense")]
        [Route("DeleteExpense")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteExpense([FromQuery] int id)
        {
            try
            {
                var result = await _expenseService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _expenseService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Expense {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        #endregion

        #region IncomeHead
        // GET: Api/Admin/GetAllIncomeHead
        [HttpGet]
        [Authorize]
        [ActionName("GetAllIncomeHead")]
        [Route("GetAllIncomeHead")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllIncomeHead(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var (result, count, lastId) = await _incomeHeadService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // GET: Api/Admin/CreateIncomeHead
        [HttpPost]
        [Authorize]
        [ActionName("CreateIncomeHead")]
        [Route("CreateIncomeHead")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateIncomeHead(IncomeHeadModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _incomeHeadService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Income Head {result.Name}" : $"Income Head {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _incomeHeadService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _incomeHeadService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Income Head {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // GET: Api/Admin/UpdateIncomeHead
        [HttpPost]
        [Authorize]
        [ActionName("UpdateIncomeHead")]
        [Route("UpdateIncomeHead")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateIncomeHead(IncomeHeadModel model)
        {
            try
            {
                await _incomeHeadService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Income Head {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Admin/DeleteIncomeHead
        [HttpDelete]
        [ActionName("DeleteIncomeHead")]
        [Route("DeleteIncomeHead")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteIncomeHead([FromQuery] int id)
        {
            try
            {
                var result = await _incomeHeadService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _incomeHeadService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Income Head {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        #endregion

        #region Income
        // GET: Api/Admin/GetAllIncome
        [HttpGet]
        [Authorize]
        [ActionName("GetAllIncome")]
        [Route("GetAllIncome")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllIncome(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var (result, count, lastId) = await _incomeService.Get(null, orderBy: x => x.OrderBy(x => x.Id), includeProperties: i => i.Include(x => x.IncomeHead), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // GET: Api/Admin/CreateIncome
        [HttpPost]
        [Authorize]
        [ActionName("CreateIncome")]
        [Route("CreateIncome")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateIncome(IncomeModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _incomeService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Income {result.Name}" : $"Income {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _incomeService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _incomeService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Income {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // GET: Api/Admin/UpdateIncome
        [HttpPost]
        [Authorize]
        [ActionName("UpdateIncome")]
        [Route("UpdateIncome")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateIncome(IncomeModel model)
        {
            try
            {
                await _incomeService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Income {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Admin/DeleteIncome
        [HttpDelete]
        [ActionName("DeleteIncome")]
        [Route("DeleteIncome")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteIncome([FromQuery] int id)
        {
            try
            {
                var result = await _incomeService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _incomeService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Income {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AdminController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        #endregion

    }
}
