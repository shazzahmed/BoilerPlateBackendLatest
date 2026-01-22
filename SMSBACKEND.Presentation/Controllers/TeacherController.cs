using Application.ServiceContracts;
using AutoMapper;
using Common.DTO.Request;
using Common.DTO.Response;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static Common.Utilities.Enums;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Teachers")]
    [ApiController]
    public class TeacherController : BaseController
    {
        private readonly IClassService _classService;
        private readonly ISubjectService _subjectService;
        private readonly ITeacherService _teacherService;
        private readonly IStaffService _staffService;
        private readonly ILogger<TeacherController> _logger;
        protected readonly IMapper _mapper;
        public TeacherController(
                ILogger<TeacherController> logger,
                IClassService classService,
                ISubjectService subjectService,
                ITeacherService teacherService,
                IStaffService staffService,
                IMapper mapper)
        {
            _logger = logger;
            _classService = classService;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _staffService = staffService;
            _mapper = mapper;
        }


        #region public actions

        #region teachers
        // GET: Api/Teachers/GetTeachers
        [HttpGet]
        [Authorize]
        [ActionName("GetTeachers")]
        [Route("GetTeachers")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetTeachers(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var role = UserRoles.Teacher.ToString();

                var (result, count, lastId) = await _staffService.Get<StaffModel>(
                    pageIndex,
                    pageSize,
                    x => !x.IsDeleted && x.User.UserRoles.Any(ur => ur.Role.Name == role),
                    orderBy: x => x.OrderByDescending(x => x.Id),
                    selector: x => new StaffModel
                    {
                        Id = x.Id,
                        StaffId = x.StaffId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        PhoneNo = x.PhoneNo,
                    },
                    useCache: true
                );
                var resultDto = result.Select(x => new StaffModelDto
                {
                    Id = x.Id,
                    StaffId = x.StaffId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNo = x.PhoneNo,
                }).ToList();
                return Ok(BaseModel.Succeed(data: resultDto, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TeacherController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }

        #endregion

        #region teachersubjects

        // GET: Api/Teachers/GetTeacherSubjects
        [HttpGet]
        [Authorize]
        [ActionName("GetTeacherSubjects")]
        [Route("GetTeacherSubjects")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetTeacherSubjects(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var result = await _teacherService.GetTeacherSubjects(filterObj);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TeachersController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Teachers/AssignTeacherSubjects
        [HttpPost]
        [Authorize]
        [ActionName("AssignTeacherSubjects")]
        [Route("AssignTeacherSubjects")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> AssignTeacherSubjects(TeacherSubjectsRequest model)
        {
            try
            {
                var result = await _teacherService.AssignTeacherSubjects(model);
                return new ObjectResult(BaseModel.Succeed(message: "Successful", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TeachersController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }
        

        // GET: Api/Teachers/GetClassTeachers
        [HttpGet]
        [Authorize]
        [ActionName("GetClassTeachers")]
        [Route("GetClassTeachers")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetClassTeachers(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var filterObj = JsonConvert.DeserializeObject<BaseRequest>(filters);
                filterObj.PaginationParam.PageSize = pageSize;
                filterObj.PaginationParam.PageIndex = pageIndex;
                var result = await _teacherService.GetClassTeachers(filterObj);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TeachersController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Teachers/AssignClassTeacher
        [HttpPost]
        [Authorize]
        [ActionName("AssignClassTeacher")]
        [Route("AssignClassTeacher")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> AssignClassTeacher(ClassTeacherRequest model)
        {
            try
            {
                var result = await _teacherService.AssignClassTeacher(model);
                return new ObjectResult(BaseModel.Succeed(message: "Class teacher assigned successfully", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TeachersController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        #endregion

        #endregion
    }
}
