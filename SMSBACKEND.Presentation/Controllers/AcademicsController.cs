using Application.ServiceContracts;
using Common.DTO.Request;
using Common.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace RestApi.Controllers
{
    [Authorize]
    [Route("api/Academics")]
    [ApiController]
    public class AcademicsController : BaseController
    {
        private readonly IClassService _classService;
        private readonly ISectionService _sectionService;
        private readonly ISubjectService _subjectService;
        private readonly ISchoolHouseService _schoolHouseService;
        private readonly IStudentCategoryService _studentCategoryService;
        private readonly ITimetableService _timetableService;
        private readonly ILogger<AcademicsController> _logger;
        public AcademicsController(
                ILogger<AcademicsController> logger,
                IClassService classService,
                ISectionService sectionService,
                ISubjectService subjectService,
                IStudentCategoryService studentCategoryService,
                ISchoolHouseService schoolHouseService,
                ITimetableService timetableService)
        {
            _logger = logger;
            _classService = classService;
            _sectionService = sectionService;
            _subjectService = subjectService;
            _studentCategoryService = studentCategoryService;
            _schoolHouseService = schoolHouseService;
            _timetableService = timetableService;
        }


    #region public actions

        #region sections
        // GET: Api/Academics/GetSections
        [HttpGet]
        [Authorize]
        [ActionName("GetSections")]
        [Route("GetSections")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetSections(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (result, count, lastId) = await _sectionService.Get(null, orderBy: x => x.OrderBy(x => x.Name), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.Message}"));
            }
        }
        // GET: Api/Academics/CreateSection
        [HttpPost]
        [Authorize]
        [ActionName("CreateSection")]
        [Route("CreateSection")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateSection(SectionModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _sectionService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Section {result.Name}" : $"Section {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _sectionService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _sectionService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Section {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }
        // GET: Api/Academics/UpdateSection
        [HttpPost]
        [Authorize]
        [ActionName("UpdateSection")]
        [Route("UpdateSection")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateSection(SectionModel model)
        {
            try
            {
                await _sectionService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Section {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Academics/DeleteSection
        [HttpDelete]
        [ActionName("DeleteSection")]
        [Route("DeleteSection")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteSection([FromQuery] int id)
        {
            try
            {
                var result = await _sectionService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _sectionService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Section {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
            }
        }
        #endregion

        #region classes

        // GET: Api/Academics/GetClasses
        [HttpGet]
        [Authorize]
        [ActionName("GetClasses")]
        [Route("GetClasses")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetClasses(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (result, count, lastId) = await _classService.Get(null, orderBy: x => x.OrderBy(x => x.Id), includeProperties: i=> i.Include(x=>x.ClassSections).ThenInclude(c => c.Section).Include(sc=> sc.SubjectClasses).ThenInclude(s=> s.Subject), useCache: true);
                
                // Get all class teachers
                var classTeacherResult = await _classService.GetClassTeacher();
                var allClassTeachers = new List<ClassTeacherModel>();
                if (classTeacherResult.Success && classTeacherResult.Data != null)
                {
                    allClassTeachers = (List<ClassTeacherModel>)classTeacherResult.Data;
                }
                
                foreach (var classModel in result)
                {
                    // Remove deleted sections
                    classModel.ClassSections = classModel.ClassSections
                        .Where(cs => cs.Section != null && !cs.Section.IsDeleted)
                        .ToList();
                    // Populate SelectedSectionIds
                    classModel.SelectedSectionIds = classModel.ClassSections
                        .Select(cs => cs.SectionId).OrderBy(x=> x)
                        .ToList();
                    // Remove deleted sections
                    classModel.SubjectClasses = classModel.SubjectClasses
                        .Where(cs => cs.Subject != null && !cs.Subject.IsDeleted)
                        .ToList();
                    // Populate SelectedSubjectIds
                    classModel.SelectedSubjectIds = classModel.SubjectClasses
                        .Select(cs => cs.SubjectId).OrderBy(x => x)
                        .ToList();
                    
                    // Map class teachers to this class
                    classModel.ClassTeachers = allClassTeachers
                        .Where(ct => ct.ClassName == classModel.Name)
                        .ToList();
                }
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // POST: Api/Academics/CreateClass
        [HttpPost]
        [Authorize]
        [ActionName("CreateClass")]
        [Route("CreateClass")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateClass(ClassModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _classService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Class {result.Name}" : $"Class {result.Name} Already Exist";
                    model.Id = result.Id;
                    await _classService.UpdateClassSection(model);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                result = await _classService.CreateClassSection(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Class {model.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.: {ex.Message}"));
            }
        }
        // POST: Api/Academics/UpdateClass
        [HttpPost]
        [Authorize]
        [ActionName("UpdateClass")]
        [Route("UpdateClass")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateClass(ClassModel model)
        {
            try
            {
                await _classService.UpdateClassSection(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Class {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Academics/DeleteClass
        [HttpDelete]
        [ActionName("DeleteClass")]
        [Route("DeleteClass")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteClass([FromQuery] int id)
        {
            try
            {
                var result = await _classService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _classService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Class {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
                throw;
            }
        }
        #endregion

        #region subject

        // GET: Api/Academics/GetSubjects
        [HttpGet]
        [Authorize]
        [ActionName("GetSubjects")]
        [Route("GetSubjects")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetSubjects(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (result, count, lastId) = await _subjectService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // GET: Api/Academics/CreateSubject
        [HttpPost]
        [Authorize]
        [ActionName("CreateSubject")]
        [Route("CreateSubject")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateSubject(SubjectModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _subjectService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Subject {result.Name}" : $"Subject {result.Name} Already Exist";
                    model.Id = result.Id;
                    await _subjectService.UpdateSubject(model);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _subjectService.Add(model);
                //await _subjectService.CreateSubject(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Subject {model.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // GET: Api/Academics/UpdateSubject
        [HttpPost]
        [Authorize]
        [ActionName("UpdateSubject")]
        [Route("UpdateSubject")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateSubject(SubjectModel model)
        {
            try
            {
                await _subjectService.UpdateSubject(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Subject {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Academics/DeleteSubject
        [HttpDelete]
        [ActionName("DeleteSubject")]
        [Route("DeleteSubject")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteSubject([FromQuery] int id)
        {
            try
            {
                var result = await _subjectService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _subjectService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Subject {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
                throw;
            }
        }
        #endregion

        #region studentCategory

        // GET: Api/Academics/GetAllCategory
        [HttpGet]
        [Authorize]
        [ActionName("GetAllCategory")]
        [Route("GetAllCategory")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllCategory(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (result, count, lastId) = await _studentCategoryService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.Message}"));
            }
        }
        // GET: Api/Academics/CreateCategory
        [HttpPost]
        [Authorize]
        [ActionName("CreateCategory")]
        [Route("CreateCategory")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateCategory(StudentCategoryModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _studentCategoryService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated Category {result.Name}" : $"Category {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _studentCategoryService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _studentCategoryService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added Category {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }
        // GET: Api/Academics/UpdateCategory
        [HttpPost]
        [Authorize]
        [ActionName("UpdateCategory")]
        [Route("UpdateCategory")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateCategory(StudentCategoryModel model)
        {
            try
            {
                await _studentCategoryService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated Category {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Academics/DeleteCategory
        [HttpDelete]
        [ActionName("DeleteCategory")]
        [Route("DeleteCategory")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteCategory([FromQuery] int id)
        {
            try
            {
                var result = await _studentCategoryService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _studentCategoryService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted Category {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
            }
        }
        #endregion

        #region schoolHouse

        // GET: Api/Academics/GetAllSchoolHouse
        [HttpGet]
        [Authorize]
        [ActionName("GetAllSchoolHouse")]
        [Route("GetAllSchoolHouse")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllSchoolHouse(
            [FromQuery] string filters,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (result, count, lastId) = await _schoolHouseService.Get(null, orderBy: x => x.OrderBy(x => x.Id), useCache: true);
                return Ok(BaseModel.Succeed(data: result, total: count, lastId: lastId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again.{ex.Message}"));
            }
        }
        // GET: Api/Academics/CreateSchoolHouse
        [HttpPost]
        [Authorize]
        [ActionName("CreateSchoolHouse")]
        [Route("CreateSchoolHouse")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateSchoolHouse(SchoolHouseModel model)
        {
            try
            {
                var normalizedName = model.Name.Trim().ToLower();
                var result = await _schoolHouseService.FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName, useCache: true);
                if (result != null)
                {
                    string message = result.IsDeleted ? $"Succesfully Activated School House {result.Name}" : $"School House {result.Name} Already Exist";
                    result.IsDeleted = false;
                    await _schoolHouseService.Update(result);
                    return Ok(BaseModel.Succeed(message: message, data: result));
                }
                model.Id = 0;
                result = await _schoolHouseService.Add(model);
                return Ok(BaseModel.Succeed(message: $"Succesfully Added School House {result.Name}", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: $"There was an error processing your request, please try again. {ex.Message}"));
            }
        }
        // GET: Api/Academics/UpdateSchoolHouse
        [HttpPost]
        [Authorize]
        [ActionName("UpdateSchoolHouse")]
        [Route("UpdateSchoolHouse")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateSchoolHouse(SchoolHouseModel model)
        {
            try
            {
                await _schoolHouseService.Update(model);
                return Ok(BaseModel.Succeed(message: $"Succesful Updated School House {model.Name}", data: model));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] : {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again." + ex.Message));
            }
        }
        // Delete: Api/Academics/DeleteSchoolHouse
        [HttpDelete]
        [ActionName("DeleteSchoolHouse")]
        [Route("DeleteSchoolHouse")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> DeleteSchoolHouse([FromQuery] int id)
        {
            try
            {
                var result = await _schoolHouseService.FirstOrDefaultAsync(x => x.Id == id, useCache: true);
                if (result != null)
                {
                    await _schoolHouseService.SoftDelete(result);
                    return Ok(BaseModel.Succeed(message: $"Succesfully Deleted School House {result.Name}"));
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(BaseModel.Failed(message: "There was an error processing your request, please try again. " + ex.Message));
            }
        }
        #endregion

        #region timeTable


        // GET: Api/Academics/GetAllTimetables
        [HttpGet]
        [Authorize]
        [ActionName("GetAllTimetables")]
        [Route("GetAllTimetables")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllTimetables()
        {
            try
            {
                var result = await _timetableService.GetAllTimetables();
                return Ok(BaseModel.Succeed(data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] GetAllTimetables: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "Failed to get timetables"));
            }
        }



        // POST: Api/Academics/SaveTimetable
        [HttpPost]
        [Authorize]
        [ActionName("SaveTimetable")]
        [Route("SaveTimetable")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> SaveTimetable(TimetableDifferentialUpdateRequest request)
        {
            try
            {
                var result = await _timetableService.SaveTimetable(request);
                return Ok(BaseModel.Succeed(message: "Timetable updated successfully", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AcademicsController] SaveTimetable: {ex.Message}");
                return BadRequest(BaseModel.Failed(message: "Failed to update timetable"));
            }
        }


        #endregion

    #endregion

    }
}
