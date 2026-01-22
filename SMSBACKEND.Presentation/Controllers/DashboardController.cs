using Application.Interfaces;
using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.DTO.Response.Dashboard;
using Common.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Services.Communication;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Filters;
using RestApi.Controllers;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        private readonly IMenuService _menuService;
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IClassService _classService;
        private readonly ISectionService _sectionService;
        private readonly IFeeAssignmentService _feeAssignmentService;
        private readonly IFeeTransactionService _feeTransactionService;
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly IExpenseService _expenseService;
        private readonly ILogger<DashboardController> _logger;
        private readonly SseService _sseService;
        private readonly IAuthenticationHelper _authenticationHelper;

        public DashboardController(
            IDashboardService dashboardService,
            ILogger<DashboardController> logger,
            IMenuService menuService,
                IClassService classService,
                ISectionService sectionService,
                IAuthenticationHelper authenticationHelper,
                IFeeAssignmentService feeAssignmentService,
                IFeeTransactionService feeTransactionService,
                IStudentService studentService,
                IUserService userService,
                IRolePermissionService rolePermissionService,
                SseService sseService,
                IExpenseService expenseService)
        {
            _menuService = menuService;
            _dashboardService = dashboardService;
            _logger = logger;
            _classService = classService;
            _sectionService = sectionService;
            _authenticationHelper = authenticationHelper;
            _feeAssignmentService = feeAssignmentService;
            _feeTransactionService = feeTransactionService;
            _studentService = studentService;
            _userService = userService;
            _rolePermissionService = rolePermissionService;
            _sseService = sseService;
            _expenseService = expenseService;
        }
        [HttpGet("stream")]
        [AllowAnonymous]
        public async Task GetEvents()
        {
            try
            {
                Response.Headers.Add("Content-Type", "text/event-stream");
                _sseService.AddClient(Response);
                await Task.Delay(Timeout.Infinite, HttpContext.RequestAborted);
            }
            catch (TaskCanceledException)
            {
                // The client disconnected—log if needed, or just ignore
                Console.WriteLine("Client disconnected from SSE stream.");
            }
        }

        [HttpGet("test-notification")]
        [AllowAnonymous]
        public async Task<IActionResult> TestNotification()
        {
            try
            {
                // Test the new enhanced broadcasting
                await _sseService.BroadcastCustomNotificationAsync(
                    "Test Notification",
                    "This is a test of the enhanced notification system",
                    "info",
                    "Test",
                    "1"
                );

                return Ok(new { success = true, message = "Test notification sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DashboardController] TestNotification error: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        // Get: Api/Dashboard/GetAllMenus
        [HttpGet]
        [Authorize]
        [ActionName("GetAllMenus")]
        [Route("GetAllMenus")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllMenus(string id)
        {
            try
            {
                var (roleResult, roleCount, roleLastId) = (await _rolePermissionService.Get(rp => rp.RoleId == id));
                var rolePermissionIds = roleResult.Select(x => x.PermissionId)
                .ToHashSet();
                var (result, menuCount, menuLastId) = await _menuService.Get(
                x => x.ParentId == null, null,
                includeProperties: i => i.Include(x => x.Permissions).Include(x => x.Children).ThenInclude(c => c.Permissions));
                var moduleModels = result.Select(m => new ModuleModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Path = m.Path,
                    Icon = m.Icon,
                    Description = m.Description,
                    IsActive = m.IsActive,
                    IsDashboard = m.IsDashboard,
                    IsOpen = m.IsOpen,
                    OrderById = m.OrderById,
                    ParentId = m.ParentId,
                    Type = m.Type,
                    Permissions = m.Permissions
                    .Where(p => rolePermissionIds.Contains(p.Id))
                    .Select(p => new PermissionModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        ModuleId = p.ModuleId
                    }).ToList(),
                    Children = m.Children.Select(c => new ModuleModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Path = c.Path,
                        Icon = c.Icon,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        IsDashboard = c.IsDashboard,
                        IsOpen = c.IsOpen,
                        OrderById = c.OrderById,
                        ParentId = c.ParentId,
                        Type = c.Type,
                        Permissions = c.Permissions
                            .Where(p => rolePermissionIds.Contains(p.Id))
                            .Select(p => new PermissionModel
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                ModuleId = p.ModuleId
                            })
                            .ToList()
                    }).ToList()
                }).ToList();
                var test = User.GetUserRoles();
                var res = new BaseModel { Success = true, Data = moduleModels, Message = "" };
                return new ObjectResult(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DashboardController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Create(success: false, message: ex.InnerException.Message));

            }
        }

        // Get: Api/Dashboard/UpdateRolePermissionsAsync
        [HttpPost]
        [Authorize]
        [ActionName("UpdateRolePermissionsAsync")]
        [Route("UpdateRolePermissionsAsync")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateRolePermissionsAsync([FromBody] RolePermissionUpdateRequest request)
        {

            await _rolePermissionService.SyncRolePermissionsAsync(request.RoleId, request.UpdatedPermissions);
            return new ObjectResult(new BaseModel { Success = true, Message = "Successfully Update Roles" });
        }


        // Get: Api/Dashboard/GetMenus
        [HttpGet]
        [AllowAnonymous]
        [ActionName("GetMenus")]
        [Route("GetMenus")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetMenus()
        {
            try
            {
                Guid userid;
                if (User.TryGetUserId(out userid))
                {
                    Console.WriteLine($"User ID: {userid.ToString()}");
                }
                //var userId = GetUserId();
                var roles = User.GetUserRoles();
                if (userid.ToString() == null)
                {
                    return new ObjectResult(new BaseModel { Success = false, Message = "Un-Authorized Access" });
                }
                //var authHeader = Request.Headers["Authorization"].ToString();
                //var tokenString = authHeader.Substring("Bearer ".Length).Trim();
                //var handler = new JwtSecurityTokenHandler();
                //var jwtToken = handler.ReadToken(tokenString) as JwtSecurityToken;
                //var test = _authenticationHelper.GetRoles(jwtToken);
                var (moduleResult, permissions) = await _userService.GetUserModulesWithPermissionsAsync(userid.ToString());
                //var menuResult = await _menuService.Get();
                DashboardModel result = new DashboardModel
                {
                    Permissions = permissions,
                    Module = moduleResult,
                };
                var res = new BaseModel { Success = true, Data = result, Message = "" };
                return new ObjectResult(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DashboardController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Create(success: false, message: ex.InnerException.Message));

            }
        }
        /// <summary>
        /// Get Admin Dashboard Data
        /// Returns comprehensive dashboard with metrics, charts, and activities
        /// </summary>
        [HttpGet("Admin")]
        // Temporarily allow all authenticated users - adjust roles as needed
        public async Task<ActionResult<BaseModel<AdminDashboardResponse>>> GetAdminDashboard([FromQuery] string dateRange = "month")
        {
            try
            {
                Guid userid;
                if (User.TryGetUserId(out userid))
                {
                    Console.WriteLine($"User ID: {userid.ToString()}");
                }
                
                
                if (userid.ToString() == null)
                {
                    return new ObjectResult(new BaseModel { Success = false, Message = "Un-Authorized Access" });
                }
                // Try multiple sources for TenantId
                var tenantIdStr = User.FindFirst("TenantId")?.Value 
                               ?? User.FindFirst("tenantId")?.Value 
                               ?? HttpContext.Items["TenantId"]?.ToString()
                               ?? "1";
                
                var tenantId = int.Parse(tenantIdStr);
                
                _logger.LogInformation("Dashboard requested - UserId: {UserId}, TenantId: {TenantId}", userid.ToString(), tenantId);

                var dashboard = await _dashboardService.GetAdminDashboardAsync(userid.ToString(), tenantId, dateRange);

                return Ok(new BaseModel<AdminDashboardResponse>
                {
                    Success = true,
                    Data = dashboard,
                    Message = "Dashboard loaded successfully",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                return StatusCode(500, new BaseModel<AdminDashboardResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error loading dashboard: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// Get Teacher Dashboard Data
        /// Returns teacher-specific dashboard with schedule and assignments
        /// </summary>
        [HttpGet("Teacher")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<BaseModel<TeacherDashboardResponse>>> GetTeacherDashboard()
        {
            try
            {
                // Get teacher ID from claims or user mapping
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tenantId = int.Parse(User.FindFirst("TenantId")?.Value ?? "1");
                
                // TODO: Map userId to teacherId (Staff table)
                int teacherId = 1; // Placeholder

                var dashboard = await _dashboardService.GetTeacherDashboardAsync(teacherId, tenantId);

                return Ok(new BaseModel<TeacherDashboardResponse>
                {
                    Success = true,
                    Data = dashboard,
                    Message = "Dashboard loaded successfully",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
            catch (NotImplementedException)
            {
                return Ok(new BaseModel<TeacherDashboardResponse>
                {
                    Success = false,
                    Data = null,
                    Message = "Teacher dashboard coming soon!",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading teacher dashboard");
                return StatusCode(500, new BaseModel<TeacherDashboardResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error loading dashboard: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// Get Parent Dashboard Data
        /// Returns parent-specific dashboard with children info and fees
        /// </summary>
        [HttpGet("Parent")]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult<BaseModel<ParentDashboardResponse>>> GetParentDashboard()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tenantId = int.Parse(User.FindFirst("TenantId")?.Value ?? "1");
                
                // TODO: Map userId to parentId
                int parentId = 1; // Placeholder

                var dashboard = await _dashboardService.GetParentDashboardAsync(parentId, tenantId);

                return Ok(new BaseModel<ParentDashboardResponse>
                {
                    Success = true,
                    Data = dashboard,
                    Message = "Dashboard loaded successfully",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
            catch (NotImplementedException)
            {
                return Ok(new BaseModel<ParentDashboardResponse>
                {
                    Success = false,
                    Data = null,
                    Message = "Parent dashboard coming soon!",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading parent dashboard");
                return StatusCode(500, new BaseModel<ParentDashboardResponse>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error loading dashboard: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// Get Chart Data
        /// Returns specific chart data for given type and date range
        /// </summary>
        [HttpGet("Chart/{chartType}")]
        public async Task<ActionResult<BaseModel<object>>> GetChartData(
            string chartType, 
            [FromQuery] string dateRange = "month")
        {
            try
            {
                var tenantId = int.Parse(User.FindFirst("TenantId")?.Value ?? "1");
                var chartData = await _dashboardService.GetChartDataAsync(chartType, dateRange, tenantId);

                return Ok(new BaseModel<object>
                {
                    Success = true,
                    Data = chartData,
                    Message = "Chart data loaded successfully",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chart data for type: {ChartType}", chartType);
                return StatusCode(500, new BaseModel<object>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error loading chart: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// Get Real-time Metric Update
        /// Used by SignalR to broadcast updates
        /// </summary>
        [HttpGet("Metric/{metricType}")]
        public async Task<ActionResult<BaseModel<MetricUpdate>>> GetMetricUpdate(string metricType)
        {
            try
            {
                var tenantId = int.Parse(User.FindFirst("TenantId")?.Value ?? "1");
                var metricUpdate = await _dashboardService.GetMetricUpdateAsync(metricType, tenantId);

                return Ok(new BaseModel<MetricUpdate>
                {
                    Success = true,
                    Data = metricUpdate,
                    Message = "Metric updated",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metric update for: {MetricType}", metricType);
                return StatusCode(500, new BaseModel<MetricUpdate>
                {
                    Success = false,
                    Data = null,
                    Message = $"Error: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = HttpContext.TraceIdentifier
                });
            }
        }
    }
}
