
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using static Common.Utilities.Enums;
using Common.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using Hangfire;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using Common.Utilities.StaticClasses;
using Microsoft.EntityFrameworkCore;
using SMSBACKEND.Infrastructure.Services.Services;
using Application.ServiceContracts.IServices;

namespace Infrastructure.Services.Services
{

    public class StudentService : BaseService<StudentModel, Student, int>, IStudentService
    {
        private readonly IUserService _userService;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentCategoryService _studentCategoryService;
        private readonly ISchoolHouseService _schoolHouseService;
        private readonly IClassService _classService;
        private readonly ISectionService _sectionService;
        private readonly IFeeAssignmentService _feeAssignmentService;
        private readonly IFeeAssignmentRepository _feeAssignmentRepository;
        private readonly IStudentAttendanceService _attendanceService;
        private readonly IExamResultService _examResultService;
        protected readonly UserManager<ApplicationUser> _applicationUserManager;
        private readonly IBackgroundJobClient _jobs;
        private readonly ICacheProvider _cacheProvider;
        private readonly IAdmissionService _admissionService;
        private readonly IExcelParserService _excelParserService;
        private readonly ISessionService _sessionService;
        private readonly ITenantService _tenantService;

        public StudentService(
            IMapper mapper,
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork,
            IUserService userService,
            IClassService classService,
            ISectionService sectionService,
            UserManager<ApplicationUser> applicationUserManager,
            IStudentCategoryService studentCategoryService,
            ISchoolHouseService schoolHouseService,
            IFeeAssignmentService feeAssignmentService,
            IFeeAssignmentRepository feeAssignmentRepository,
            IStudentAttendanceService attendanceService,
            IExamResultService examResultService,
            IAdmissionService admissionService,
            SseService sseService,
            IBackgroundJobClient jobs,
            ISessionService sessionService,
            ILogger<StudentService> logger,
            ITenantService tenantService,
            ICacheProvider cacheProvider = null,
            IExcelParserService excelParserService = null) : base(mapper, studentRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _studentRepository = studentRepository;
            _userService = userService;
            _classService = classService;
            _sectionService = sectionService;
            _applicationUserManager = applicationUserManager;
            _studentCategoryService = studentCategoryService;
            _schoolHouseService = schoolHouseService;
            _feeAssignmentService = feeAssignmentService;
            _feeAssignmentRepository = feeAssignmentRepository;
            _attendanceService = attendanceService;
            _examResultService = examResultService;
            _admissionService = admissionService;
            _jobs = jobs;
            _cacheProvider = cacheProvider;
            _excelParserService = excelParserService;
            _sessionService = sessionService;
            _tenantService = tenantService;
        }

        public async Task<BaseModel> GetAll(StudentRequest request)
        {
            var filterParams = ConvertToFilterParams(request);
            
            // Create cache key based on request parameters
            var cacheKey = $"students_all_{string.Join("_", filterParams.Filters.Select(f => $"{f.PropertyName}:{f.Value}"))}";
            
            // Use cache if available
            if (_cacheProvider != null)
            {
                var cachedResult = await _cacheProvider.GetOrCreateCacheAsync(
                    cacheKey,
                    "Student",
                    async () =>
                    {
                        var (students, totalCount, lastId, lastAdmNo) = await _studentRepository.GetAllStudent(filterParams);
                        return new BaseModel
                        {
                            Success = true,
                            Data = students,
                            Total = totalCount,
                            LastId = lastId,
                            SupportData = lastAdmNo == null ? "0" : lastAdmNo
                        };
                    },
                    CancellationToken.None);
                
                return cachedResult;
            }
            
            // Fallback without cache
            var (studentsData, totalCount, lastId, lastAdmNo) = await _studentRepository.GetAllStudent(filterParams);
            
            // For offline-first, return all data without pagination metadata
            // Frontend will handle pagination locally
            return new BaseModel
            {
                Success = true,
                Data = studentsData,
                Total = totalCount, // Keep total for reference but frontend won't use it for pagination
                LastId = lastId,
                SupportData = lastAdmNo == null ? "0" : lastAdmNo
            };
        }
        /// <summary>
        /// Unified student creation method for all flows (direct, admission, workflow)
        /// </summary>
        public async Task<BaseModel> CreateStudentUnified(StudentCreateRequest model)
        {
            try
            {
                // Validate student data before creation
                var (isValid, errorMessage) = await ValidateUnifiedStudentData(model);
                if (!isValid)
                {
                    return new BaseModel { Success = false, Message = errorMessage };
                }

                // Check if student with same admission number already exists
                if (!string.IsNullOrWhiteSpace(model.AdmissionNo))
                {
                    var existingStudent = await _studentRepository.FirstOrDefaultAsync(
                        x => x.StudentInfo.AdmissionNo == model.AdmissionNo && !x.IsDeleted);
                    if (existingStudent != null)
                    {
                        return new BaseModel { Success = false, Message = "Student with this admission number already exists" };
                    }
                }

                // Check for duplicate roll number in the same class (only if roll number is provided)
                if (!string.IsNullOrWhiteSpace(model.RollNo))
                {
                    var duplicateRollNo = await _studentRepository.FirstOrDefaultAsync(
                        x => x.StudentInfo.RollNo == model.RollNo && 
                             x.ClassAssignments.Any(ca => ca.ClassId == model.ClassId && 
                                                        ca.SectionId == model.SectionId) &&
                             !x.IsDeleted);
                    if (duplicateRollNo != null)
                    {
                        return new BaseModel { Success = false, Message = "Roll number already exists in this class and section" };
                    }
                }

                // Check if student with same name and parent already exists (prevent duplicate students)
                var existingStudentByName = await _studentRepository.FirstOrDefaultAsync(
                    x => x.StudentInfo.FirstName.ToLower() == model.FirstName.ToLower() &&
                         x.StudentInfo.LastName.ToLower() == (model.LastName ?? "").ToLower() &&
                         x.StudentInfo.Dob.HasValue && model.DateOfBirth.HasValue &&
                         x.StudentInfo.Dob.Value.Date == model.DateOfBirth.Value.Date &&
                         !x.IsDeleted);
                
                if (existingStudentByName != null)
                {
                    return new BaseModel { Success = false, Message = "A student with the same name and date of birth already exists" };
                }

                // Step 1: Create Parent Account FIRST (if needed)
                string parentId = null;
                if (!string.IsNullOrEmpty(model.FatherCNIC) || !string.IsNullOrEmpty(model.MotherCNIC))
                {
                    var parentResult = await CreateParentAccount(model);
                    if (!parentResult.Success)
                    {
                        return new BaseModel { Success = false, Message = $"Failed to create parent account: {parentResult.Message}" };
                    }
                    
                    // Extract parent ID - handle both LoginResponse (new parent) and ApplicationUserModel (existing parent)
                    if (parentResult.Data is LoginResponse loginResp)
                    {
                        parentId = loginResp.UserId;
                    }
                    else if (parentResult.Data is ApplicationUserModel appUser)
                    {
                        parentId = appUser.Id;
                    }
                    
                    model.ParentId = parentId;
                }

                // Step 2: Check if student account already exists before creating
                string studentUserId = null;
                var existingStudentAccount = await CheckExistingStudentAccount(model);
                if (existingStudentAccount == null)
                {
                // Step 3: Create Student Account (only if no existing account found)
                    var (generatedUsername, generatedEmail) = await GenerateUniqueStudentCredentials(model);
                
                    // Update model with generated email if no email was provided
                    if (string.IsNullOrEmpty(model.Email))
                    {
                        model.Email = generatedEmail;
                    }
                
                    var studentAccountResult = await CreateStudentAccount(model, generatedUsername, generatedEmail);
                    if (!studentAccountResult.Success)
                    {
                        return new BaseModel { Success = false, Message = $"Failed to create student account: {studentAccountResult.Message}" };
                    }
                
                    // Extract UserId from LoginResponse
                    var loginResponse = studentAccountResult.Data as LoginResponse;
                    studentUserId = loginResponse?.UserId;
                }
                else
                {
                    studentUserId = existingStudentAccount?.Id;
                }
                
                // Validate that student account was created successfully
                if (string.IsNullOrEmpty(studentUserId))
                {
                    return new BaseModel { Success = false, Message = "Failed to retrieve student account ID after creation" };
                }

                // Step 4: Update model with account IDs before creating student entity
                if (!string.IsNullOrEmpty(parentId))
                {
                    model.ParentId = parentId;
                }
                if (!string.IsNullOrEmpty(studentUserId))
                {
                    model.StudentUserId = studentUserId;
                }
                // Get active session ID
                var activeSession = await _sessionService.GetActiveSessionId();
                if (activeSession == 0)
                {
                    throw new Exception("No active session found. Please set an active academic session.");
                }
                model.SessionId = activeSession;
                // Step 5: Create student entity with account IDs
                var studentId = await _studentRepository.CreateStudent(model);
                if (studentId <= 0)
                {
                    return new BaseModel { Success = false, Message = "Failed to create student" };
                }

                // Step 6: Get the created student with all entities
                var result = await _studentRepository.FirstOrDefaultAsync(x=> x.Id == studentId,null, includeProperties: i => i.Include(u => u.StudentInfo).Include(u => u.StudentParent).Include(u => u.StudentFinancial).Include(s => s.StudentAddress).Include(u => u.StudentMiscellaneous).Include(u => u.StudentMiscellaneous).Include(u => u.ClassAssignments));
                
                // Update sync status for offline-first functionality
                result.SyncStatus = "created";
                result.UpdatedAt = DateTime.UtcNow;

                // Step 6: Create fee assignments from fee package (if selected)
                if (model.FeeGroupFeeTypeId.HasValue)
                {
                    await CreateFeeAssignmentsFromPackage(studentId, model);
                }
                // OR create fee assignments if manual monthly fees are specified
                else if (model.MonthlyFees.HasValue && model.MonthlyFees > 0)
                {
                    //await CreateFeeAssignments(studentId, model);
                }

                // Handle flow-specific post-creation logic
                await HandleFlowSpecificLogic(model, studentId, model.AdmissionNo);

                // Clear cache after successful creation
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync("Student");
                    
                    // Remove cache for dependent entities
                    if (EntityDependencyMap.Dependencies.TryGetValue("Student", out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }
                }

                // Log audit trail
                var correlationId = AuditTrailHelper.LogStudentCreate("System", result); // TODO: Get from current user context

                return new BaseModel 
                { 
                    Success = true, 
                    Data = result, 
                    Total = result.Id, 
                    Message = "Student created successfully", 
                    CorrelationId = correlationId 
                };
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Message = $"Error creating student: {ex.Message}" };
            }
        }

        /// <summary>
        /// Legacy method - redirects to unified approach
        /// </summary>
        public async Task<BaseModel> CreateStudent(StudentCreateRequest model)
        {
            // Redirect to unified method
            return await CreateStudentUnified(model);
        }




        public async Task<BaseModel> UpdateStudent(StudentCreateRequest model)
        {
            try
            {
                // ===== PHASE 1: VALIDATION =====
                if (model.Id <= 0)
                {
                    return new BaseModel { Success = false, Message = "Student ID is required for update" };
                }

                // Validate updated data (Service Layer - Business Logic)
                var (isValid, errorMessage) = await ValidateUnifiedStudentData(model, isUpdate: true);
                if (!isValid)
                {
                    return new BaseModel { Success = false, Message = errorMessage };
                }

                // ===== PHASE 2: FETCH & DETECT CHANGES (Using Repository) =====
                var existingStudent = await _studentRepository.FirstOrDefaultAsync(
                    x => x.Id == model.Id,
                    null,
                    includeProperties: i => i
                        .Include(s => s.StudentInfo)
                        .Include(s => s.StudentAddress)
                        .Include(s => s.StudentFinancial)
                        .Include(s => s.StudentParent)
                        .Include(s => s.StudentMiscellaneous)
                        .Include(s => s.ClassAssignments));

                if (existingStudent == null)
                {
                    return new BaseModel { Success = false, Message = "Student not found" };
                }

                if (existingStudent.IsDeleted)
                {
                    return new BaseModel { Success = false, Message = "Cannot update deleted student. Please restore first." };
                }

                // Detect changes (Service Layer - Business Logic)
                var changes = DetectChanges(existingStudent, model);
                
                // ===== PHASE 3: VALIDATE NO CONFLICTS (Using Repository) =====
                if (changes.AdmissionNoChanged && !string.IsNullOrEmpty(model.AdmissionNo))
                {
                    var isUnique = await _studentRepository.IsAdmissionNoUnique(model.AdmissionNo, model.Id);
                    if (!isUnique)
                    {
                        return new BaseModel { Success = false, Message = "Admission number already exists for another student" };
                    }
                }

                if ((changes.RollNoChanged || changes.ClassOrSectionChanged) && !string.IsNullOrEmpty(model.RollNo))
                {
                    var isUnique = await _studentRepository.IsRollNoUniqueInClass(model.RollNo, model.ClassId, model.SectionId, model.Id);
                    if (!isUnique)
                    {
                        return new BaseModel { Success = false, Message = "Roll number already exists in this class and section" };
                    }
                }

                // ===== PHASE 4: HANDLE IDENTITY CHANGES (Service Layer - Identity Management) =====
                string updatedParentId = existingStudent.StudentInfo.ParentId;
                
                if (changes.ParentCNICChanged || changes.GuardianTypeChanged)
                {
                    // Critical: Parent account change
                    var parentResult = await HandleParentAccountChange(model, existingStudent);
                    if (!parentResult.Success)
                    {
                        return parentResult;
                    }
                    updatedParentId = parentResult.Data as string;
                    model.ParentId = updatedParentId; // Update model for repository
                }
                else if (changes.ParentInfoChanged)
                {
                    // Non-critical: Parent contact info update
                    var parentUpdateResult = await UpdateParentAccountInfo(model, existingStudent);
                    if (!parentUpdateResult.Success)
                    {
                        return parentUpdateResult;
                    }
                }

                // Update student account in identity system
                if (changes.EmailChanged || changes.PhoneChanged || changes.NameChanged)
                {
                    var studentAccountResult = await UpdateStudentAccount(model, existingStudent);
                    if (!studentAccountResult.Success)
                    {
                        return studentAccountResult;
                    }
                }

                // ===== PHASE 5: UPDATE DATABASE (Repository Layer) =====
                // Repository handles all database operations and transactions
                var updatedStudentModel = await _studentRepository.UpdateStudent(model);
                if (updatedStudentModel == null)
                {
                    return new BaseModel { Success = false, Message = "Failed to update student in database" };
                }

                // ===== PHASE 5.5: HANDLE FEE PACKAGE OPERATIONS =====
                
                // Handle package removal (if requested)
                if (!string.IsNullOrEmpty(model.PackageIdToRemove))
                {
                    var removeResult = await RemoveFeePackage(model.Id, model.PackageIdToRemove);
                    if (!removeResult.Success)
                    {
                        return new BaseModel { Success = false, Message = removeResult.Message };
                    }
                }
                
                // Handle package replacement (if new package selected)
                if (model.FeeGroupFeeTypeId.HasValue)
                {
                    var feeReplaceResult = await HandleFeePackageReplacement(model.Id, model);
                    if (!feeReplaceResult.Success)
                    {
                        return new BaseModel { Success = false, Message = feeReplaceResult.Message };
                    }
                }

                // ===== PHASE 6: POST-UPDATE OPERATIONS (Service Layer) =====
                // Log audit trail
                var correlationId = AuditTrailHelper.LogStudentUpdate(
                    model.UpdatedBy ?? "System", 
                    existingStudent, 
                    changes);

                // Clear cache
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync("Student");
                    
                    if (EntityDependencyMap.Dependencies.TryGetValue("Student", out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }
                }

                updatedStudentModel.SyncStatus = "updated";

                return new BaseModel 
                { 
                    Success = true, 
                    Message = "Student updated successfully", 
                    Data = updatedStudentModel,
                    CorrelationId = correlationId 
                };
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Message = "Failed to update student: " + ex.Message };
            }
        }
        /// <summary>
        /// Get student by ID with ALL related data for detailed view
        /// </summary>
        public async Task<BaseModel> GetStudentById(int id)
        {
            try
            {
                // ✅ Load student with ALL related entities
                var student = await _studentRepository.FirstOrDefaultAsync(
                    x => x.Id == id && !x.IsDeleted,
                    includeProperties: i => i
                        // Student basic entities
                        .Include(s => s.StudentInfo)
                        .Include(s => s.StudentAddress)
                        .Include(s => s.StudentFinancial)
                        .Include(s => s.StudentParent)
                        .Include(s => s.StudentMiscellaneous)
                        
                        // Academic info
                        .Include(s => s.ClassAssignments)
                            .ThenInclude(ca => ca.Class)
                        .Include(s => s.ClassAssignments)
                            .ThenInclude(ca => ca.Section)
                        //.Include(s => s.StudentCategory)
                        //.Include(s => s.SchoolHouse)
                        
                        // Attendance (we'll get summary separately)
                        // Fees (we'll get separately)
                        // User account (we'll get separately)
                );

                if (student == null)
                {
                    return new BaseModel { Success = false, Message = "Student not found" };
                }

                var studentModel = mapper.Map<StudentModel>(student);

                // ✅ Get user account info (student & parent)
                if (!string.IsNullOrEmpty(student.StudentInfo?.StudentUserId))
                {
                    var studentUser = await _applicationUserManager.FindByIdAsync(student.StudentInfo.StudentUserId);
                    if (studentUser != null)
                    {
                        studentModel.StudentUserAccount = new
                        {
                            studentUser.Email,
                            studentUser.PhoneNumber,
                            studentUser.IsActive,
                            studentUser.LockoutEnd,
                            FailedLoginAttempts = studentUser.AccessFailedCount
                        };
                    }
                }

                if (!string.IsNullOrEmpty(student.StudentInfo?.ParentId))
                {
                    var parentUser = await _applicationUserManager.FindByIdAsync(student.StudentInfo.ParentId);
                    if (parentUser != null)
                    {
                        studentModel.ParentUserAccount = new
                        {
                            parentUser.Email,
                            parentUser.PhoneNumber,
                            parentUser.IsActive,
                            Username = parentUser.UserName
                        };
                    }
                }

                // ✅ Get siblings (students with same parent)
                if (!string.IsNullOrEmpty(student.StudentInfo?.ParentId))
                {
                    var siblings = await _studentRepository.GetAsync(
                        where: s => s.StudentInfo.ParentId == student.StudentInfo.ParentId 
                                   && s.Id != id 
                                   && !s.IsDeleted,
                        includeProperties: i => i
                            .Include(s => s.StudentInfo)
                            .Include(s => s.ClassAssignments)
                                .ThenInclude(ca => ca.Class)
                            .Include(s => s.ClassAssignments)
                                .ThenInclude(ca => ca.Section)
                    );

                    studentModel.Siblings = mapper.Map<List<StudentModel>>(siblings.ToList());
                }

                // ✅ Get attendance summary
                // Attendance data already loaded in GetAllStudents from repository
                // For GetStudentById, just initialize empty - data comes from GetAllStudents
                studentModel.AttendanceSummary = new
                {
                    TotalPresent = 0,
                    TotalAbsent = 0,
                    TotalLeave = 0,
                    TotalLate = 0,
                    Total = 0,
                    AttendancePercentage = 0.0
                };

                // ✅ Get fee summary
                var feeAssignments = await _feeAssignmentRepository.GetAsync(
                    where: f => f.StudentId == id && !f.IsDeleted,
                    includeProperties: i => i
                        .Include(f => f.FeeGroupFeeType)
                            .ThenInclude(fgft => fgft.FeeType)
                        .Include(f => f.FeeGroupFeeType)
                            .ThenInclude(fgft => fgft.FeeGroup)
                        .Include(f => f.AppliedDiscount)
                        .Include(f => f.FeeTransactions)
                );

                var feeList = feeAssignments.ToList();
                studentModel.FeeSummary = new
                {
                    TotalFees = feeList.Sum(f => f.Amount),
                    TotalPaid = feeList.Sum(f => f.PaidAmount),
                    TotalPending = feeList.Sum(f => f.Amount - f.PaidAmount),
                    TotalOverdue = feeList.Where(f => f.DueDate < DateTime.Now && f.PaidAmount < f.Amount).Sum(f => f.Amount - f.PaidAmount),
                    FeeAssignments = mapper.Map<List<FeeAssignmentModel>>(feeList)
                };

                // ✅ Get exam results using repository (no service method for GetAll)
                // ExamResultService doesn't have GetAll, so we'll get this from repository already loaded in GetAllStudents
                // For now, leave empty - data will come from GetAllStudents
                studentModel.RecentExamResults = new List<ExamResultModel>();

                return new BaseModel
                {
                    Success = true,
                    Data = studentModel,
                    Message = "Student details retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting student details: {ex.Message}");
                return new BaseModel { Success = false, Message = $"Error retrieving student: {ex.Message}" };
            }
        }

        public async Task<BaseModel> GetParentDataByCNIC(string cnic)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cnic))
                {
                    return new BaseModel { Success = false, Message = "CNIC is required" };
                }

                // Search for existing students with this FatherCNIC or MotherCNIC or ParentId
                var existingStudent = await _studentRepository.FirstOrDefaultAsync(
                    x => (x.StudentParent.FatherCNIC == cnic || x.StudentParent.MotherCNIC == cnic || x.StudentInfo.ParentId == cnic) && !x.IsDeleted,
                    null,
                    includeProperties: i => i
                        .Include(s => s.StudentInfo)
                        .Include(s => s.StudentParent)
                );

                if (existingStudent == null || existingStudent.StudentParent == null)
                {
                    return new BaseModel 
                    { 
                        Success = false, 
                        Message = "No parent found with this CNIC/ParentId",
                        Data = null
                    };
                }

                // Return parent data
                var parentData = new
                {
                    ParentId = existingStudent.StudentInfo?.ParentId,
                    FatherCNIC = existingStudent.StudentParent.FatherCNIC,
                    FatherName = existingStudent.StudentParent.FatherName,
                    FatherPhone = existingStudent.StudentParent.FatherPhone,
                    FatherOccupation = existingStudent.StudentParent.FatherOccupation,
                    MotherCNIC = existingStudent.StudentParent.MotherCNIC,
                    MotherName = existingStudent.StudentParent.MotherName,
                    MotherPhone = existingStudent.StudentParent.MotherPhone,
                    MotherOccupation = existingStudent.StudentParent.MotherOccupation,
                    GuardianIs = existingStudent.StudentParent.GuardianIs,
                    GuardianName = existingStudent.StudentParent.GuardianName,
                    GuardianRelation = existingStudent.StudentParent.GuardianRelation,
                    GuardianEmail = existingStudent.StudentParent.GuardianEmail,
                    GuardianPhone = existingStudent.StudentParent.GuardianPhone,
                    GuardianOccupation = existingStudent.StudentParent.GuardianOccupation,
                    GuardianAddress = existingStudent.StudentParent.GuardianAddress,
                    GuardianAreaAddress = existingStudent.StudentParent.GuardianAreaAddress
                };

                return new BaseModel 
                { 
                    Success = true, 
                    Data = parentData,
                    Message = "Parent data found",
                    Total = 1
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching parent data: {ex.Message}");
                return new BaseModel { Success = false, Message = $"Error retrieving parent data: {ex.Message}" };
            }
        }

        public async Task<BaseModel> DeleteStudent(int id)
        {
            try
            {
                var student = await FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                
                if (student == null)
                {
                    return new BaseModel { Success = false, Message = "Student not found" };
                }

                // Soft delete student using repository method
                var deleteResult = await _studentRepository.SoftDeleteStudent(id, "System"); // TODO: Get from current user context
                if (!deleteResult)
                {
                    return new BaseModel { Success = false, Message = "Failed to delete student" };
                }

                // Deactivate associated user
                var user = await _applicationUserManager.FindByIdAsync(student.StudentUserId);
                if (user != null)
                {
                    user.IsActive = false;
                    await _applicationUserManager.UpdateAsync(user);
                    
                    var roles = await _applicationUserManager.GetRolesAsync(user);
                    if (roles.Any())
                    {
                        await _applicationUserManager.RemoveFromRolesAsync(user, roles);
                    }
                }
                
                // Update sync status for offline-first functionality
                student.SyncStatus = "deleted";
                student.UpdatedAt = DateTime.UtcNow;
                
                // Clear cache after successful deletion
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync("Student");
                    
                    // Remove cache for dependent entities
                    if (EntityDependencyMap.Dependencies.TryGetValue("Student", out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }
                }
                
                var correlationId = AuditTrailHelper.LogStudentDelete("System", student); // TODO: Get from current user context
                return new BaseModel { Success = true, Message = "Student deleted successfully", CorrelationId = correlationId };
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                return new BaseModel { Success = false, Message = "Failed to delete student: " + ex.Message };
            }
        }

        public async Task<BaseModel> RestoreStudent(int id)
        {
            try
            {
                var student = await FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted);
                
                if (student == null)
                {
                    return new BaseModel { Success = false, Message = "Student not found or not deleted" };
                }

                // Restore student using repository method
                var restoreResult = await _studentRepository.RestoreStudent(id, "System"); // TODO: Get from current user context
                if (!restoreResult)
                {
                    return new BaseModel { Success = false, Message = "Failed to restore student" };
                }

                // Reactivate associated user
                var user = await _applicationUserManager.FindByIdAsync(student.StudentUserId);
                if (user != null)
                {
                    user.IsActive = true;
                    await _applicationUserManager.UpdateAsync(user);
                    
                    // Re-add student role
                    var roles = await _applicationUserManager.GetRolesAsync(user);
                    if (!roles.Contains(UserRoles.Student.ToString()))
                    {
                        await _applicationUserManager.AddToRoleAsync(user, UserRoles.Student.ToString());
                    }
                }
                
                // Clear cache after successful restoration
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync("Student");
                    
                    // Remove cache for dependent entities
                    if (EntityDependencyMap.Dependencies.TryGetValue("Student", out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }
                }
                
                var correlationId = AuditTrailHelper.LogStudentRestore("System", student); // TODO: Get from current user context
                return new BaseModel { Success = true, Message = "Student restored successfully", CorrelationId = correlationId };
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                return new BaseModel { Success = false, Message = "Failed to restore student: " + ex.Message };
            }
        }
        public void BatchImportStudents(object data)
        {
            _jobs.Enqueue(() => BatchImportStudentsJob(data));
        }

        public async Task BatchImportStudentsJob(object data)
        {
            // TODO: Implement batch import with object parameter
            // For now, return a placeholder
            await Task.CompletedTask;
        }

        #region Validation Methods for Stepper Form

        /// <summary>
        /// Validates parent uniqueness with CNIC primary and Phone+Name fallback
        /// </summary>
        public async Task<ParentValidationResponse> ValidateParentAsync(ParentValidationRequest request)
        {
            var response = new ParentValidationResponse();

            try
            {
                // Primary validation: CNIC-based
                if (!string.IsNullOrEmpty(request.CNIC))
                {
                    var existingParentByCNIC = await _userService.IsCNICAlreadyExist(request.CNIC, UserRoles.Parent.ToString());
                    if (existingParentByCNIC)
                    {
                        var parentUser = await _userService.GetUser(request.CNIC);
                        if (parentUser.Success && parentUser.Data != null)
                        {
                            response.IsValid = true;
                            response.ParentExists = true;
                            response.ExistingParent = (ApplicationUserModel)parentUser.Data;
                            response.Message = "Parent found with CNIC";
                            response.ValidationResult = new ParentValidationResult
                            {
                                ValidationMethod = "CNIC",
                                IsUnique = false,
                                SuggestedAction = "LINK_EXISTING"
                            };
                            return response;
                        }
                    }
                }

                // Fallback validation: Phone + Name combination
                if (!string.IsNullOrEmpty(request.PhoneNumber) && !string.IsNullOrEmpty(request.FirstName) && !string.IsNullOrEmpty(request.LastName))
                {
                    var existingParentByPhoneName = await _userService.GetUserByPhoneAndName(request.PhoneNumber, request.FirstName, request.LastName);
                    if (existingParentByPhoneName != null)
                    {
                        response.IsValid = true;
                        response.ParentExists = true;
                        response.ExistingParent = existingParentByPhoneName;
                        response.Message = "Parent found with Phone + Name combination";
                        response.ValidationResult = new ParentValidationResult
                        {
                            ValidationMethod = "Phone+Name",
                            IsUnique = false,
                            SuggestedAction = "LINK_EXISTING"
                        };
                        return response;
                    }
                }

                // No existing parent found - can create new
                response.IsValid = true;
                response.ParentExists = false;
                response.Message = "No existing parent found - can create new parent";
                response.ValidationResult = new ParentValidationResult
                {
                    ValidationMethod = string.IsNullOrEmpty(request.CNIC) ? "Phone+Name" : "CNIC",
                    IsUnique = true,
                    SuggestedAction = "CREATE_NEW"
                };
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.Message = $"Validation error: {ex.Message}";
            }

            return response;
        }

        /// <summary>
        /// Validates student uniqueness for admission number and roll number
        /// </summary>
        public async Task<StudentValidationResponse> ValidateStudentAsync(StudentValidationRequest request)
        {
            var response = new StudentValidationResponse();
            var validationResult = new StudentValidationResult();

            try
            {
                // Validate Admission Number
                if (!string.IsNullOrEmpty(request.AdmissionNo))
                {
                    var existingStudentByAdmission = await _studentRepository.FirstOrDefaultAsync(
                        x => x.StudentInfo.AdmissionNo == request.AdmissionNo && !x.IsDeleted);
                    
                    if (existingStudentByAdmission != null && existingStudentByAdmission.Id != request.ExistingStudentId)
                    {
                        validationResult.AdmissionNoValid = false;
                        validationResult.AdmissionNoMessage = "Admission number already exists";
                    }
                    else
                    {
                        validationResult.AdmissionNoValid = true;
                        validationResult.AdmissionNoMessage = "Admission number is available";
                    }
                }
                else
                {
                    // Generate admission number if not provided
                    var generatedAdmissionNo = await GetNextAdmissionNumberAsync();
                    response.GeneratedAdmissionNo = generatedAdmissionNo;
                    validationResult.AdmissionNoValid = true;
                    validationResult.AdmissionNoMessage = $"Generated admission number: {generatedAdmissionNo}";
                }

                // Validate Roll Number (only if provided and within same class/section)
                if (!string.IsNullOrEmpty(request.RollNo))
                {
                    var existingStudentByRoll = await _studentRepository.FirstOrDefaultAsync(
                        x => x.StudentInfo.RollNo == request.RollNo && 
                             x.ClassAssignments.Any(ca => ca.ClassId == request.ClassId && ca.SectionId == request.SectionId) &&
                             x.Id != request.ExistingStudentId &&
                             !x.IsDeleted);
                    
                    if (existingStudentByRoll != null)
                    {
                        validationResult.RollNoValid = false;
                        validationResult.RollNoMessage = "Roll number already exists in this class and section";
                    }
                    else
                    {
                        validationResult.RollNoValid = true;
                        validationResult.RollNoMessage = "Roll number is available";
                    }
                }
                else
                {
                    validationResult.RollNoValid = true;
                    validationResult.RollNoMessage = "Roll number is optional";
                }

                // Validate Age
                if (request.Dob.HasValue)
                {
                    var age = DateTime.Now.Year - request.Dob.Value.Year;
                    if (age < 3 || age > 25)
                    {
                        validationResult.AgeValid = false;
                        validationResult.AgeMessage = "Student age should be between 3 and 25 years";
                    }
                    else
                    {
                        validationResult.AgeValid = true;
                        validationResult.AgeMessage = $"Age is valid ({age} years)";
                    }
                }
                else
                {
                    validationResult.AgeValid = true;
                    validationResult.AgeMessage = "Date of birth is optional";
                }

                response.ValidationResult = validationResult;
                response.IsValid = validationResult.AdmissionNoValid && validationResult.RollNoValid && validationResult.AgeValid;
                response.Message = response.IsValid ? "Student validation passed" : "Student validation failed";
            }
            catch (Exception ex)
            {
                response.IsValid = false;
                response.Message = $"Validation error: {ex.Message}";
            }

            return response;
        }

        /// <summary>
        /// Public method to preview the next admission number for frontend display
        /// Uses tenant-specific sequence and format from Tenant.AdmissionNoFormat
        /// DOES NOT increment sequence - this is just a preview
        /// Actual sequence increment happens during CreateStudent
        /// </summary>
        public async Task<string> GetNextAdmissionNumberAsync()
        {
            try
            {
                // Get current tenant ID
                var tenantId = _tenantService.GetCurrentTenantId();
                if (tenantId == 0)
                {
                    _logger?.LogWarning("GetNextAdmissionNumberAsync called without tenant context");
                    return "ADM2025001"; // Fallback
                }

                // Use PEEK method - doesn't actually increment, just shows what's next
                var admissionNumber = await _studentRepository.PeekNextAdmissionNumberAsync(tenantId);
                
                _logger?.LogInformation($"Peeked next admission number: {admissionNumber} for tenant {tenantId}");
                
                return admissionNumber;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error peeking next admission number");
                throw;
            }
        }

        private DateTime? TryParseDate(string dateStr)
        {
            if (DateTime.TryParse(dateStr, out var dt)) return dt;
            return null;
        }

        private int ParseInt(object obj)
        {
            return int.TryParse(obj?.ToString(), out int val) ? val : 0;
        }

        private float? ParseFloat(object obj)
        {
            return float.TryParse(obj?.ToString(), out float val) ? val : null;
        }

        private decimal ParseDecimal(object obj)
        {
            return decimal.TryParse(obj?.ToString(), out decimal val) ? val : 0;
        }

        private string GenerateEmail(string name)
        {
            return name?.ToLower().Replace(" ", ".") + "@example.com";
        }

        private async Task<(bool IsValid, string ErrorMessage)> ValidateStudentData(StudentCreateRequest model)
        {
            // Check for duplicate admission number
            if (!string.IsNullOrWhiteSpace(model.AdmissionNo))
            {
                var existingStudent = await _studentRepository.FirstOrDefaultAsync(
                    x => x.StudentInfo.AdmissionNo == model.AdmissionNo && !x.IsDeleted);
                if (existingStudent != null)
                {
                    return (false, "Admission number already exists");
                }
            }

            // Check for duplicate roll number in the same class (only if roll number is provided)
            if (!string.IsNullOrWhiteSpace(model.RollNo))
            {
                var existingRollNo = await _studentRepository.FirstOrDefaultAsync(
                    x => x.StudentInfo.RollNo == model.RollNo && 
                         x.ClassAssignments.Any(ca => ca.ClassId == model.ClassId && 
                                                    ca.SectionId == model.SectionId) &&
                         !x.IsDeleted);
                if (existingRollNo != null)
                {
                    return (false, "Roll number already exists in this class and section");
                }
            }

            // Validate age (should be between 3 and 25 for school students)
            if (model.DateOfBirth.HasValue)
            {
                var age = DateTime.Now.Year - model.DateOfBirth.Value.Year;
                if (age < 2 || age > 25)
                {
                    return (false, "Student age should be between 2 and 25 years");
                }
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                return (false, "First name is required");
            }

            if (string.IsNullOrWhiteSpace(model.FatherCNIC))
            {
                return (false, "Father CNIC is required");
            }

            if (string.IsNullOrWhiteSpace(model.GuardianPhone))
            {
                return (false, "Guardian phone number is required");
            }

            return (true, null);
        }
        
        private FilterParams ConvertToFilterParams(StudentRequest request)
        {
            var filterParams = new FilterParams();
            
            // Add filters based on the new request structure
            if (!string.IsNullOrEmpty(request.Gender))
            {
                filterParams.Filters.Add(new Filter { PropertyName = "Gender", Value = request.Gender });
            }
            if (!string.IsNullOrEmpty(request.SectionId))
            {
                filterParams.Filters.Add(new Filter { PropertyName = "SectionId", Value = request.SectionId });
            }
            if (!string.IsNullOrEmpty(request.ClassId))
            {
                filterParams.Filters.Add(new Filter { PropertyName = "ClassId", Value = request.ClassId });
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                filterParams.Filters.Add(new Filter { PropertyName = "Name", Value = request.Name });
            }
            
            // Add sync-related filters
            if (request.LastSyncTime.HasValue)
            {
                filterParams.Filters.Add(new Filter { PropertyName = "LastSyncTime", Value = request.LastSyncTime.Value });
            }
            
            // Set default pagination for backward compatibility (though not used in offline-first)
            filterParams.PaginationParam = new PaginationParams { PageIndex = 1, PageSize = 1000 };
            
            return filterParams;
        }

        #endregion

        #region Unified Student Creation Methods

        /// <summary>
        /// Validate unified student creation data
        /// </summary>
        private async Task<(bool isValid, string errorMessage)> ValidateUnifiedStudentData(StudentCreateRequest model)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(model.FirstName))
                return (false, "First name is required");

            if (model.ClassId <= 0)
                return (false, "Class is required");

            if (model.SectionId <= 0)
                return (false, "Section is required");

            //if (string.IsNullOrWhiteSpace(model.PickupPerson))
            //    return (false, "Pickup person is required");

            if (string.IsNullOrWhiteSpace(model.CurrentAddress))
                return (false, "Current address is required");

            // Validate class and section exist
            var classExists = await _classService.FirstOrDefaultAsync(x => x.Id == model.ClassId && !x.IsDeleted);
            if (classExists == null)
                return (false, "Selected class does not exist");

            var sectionExists = await _sectionService.FirstOrDefaultAsync(x => x.Id == model.SectionId && !x.IsDeleted);
            if (sectionExists == null)
                return (false, "Selected section does not exist");

            // Validate category if provided
            if (model.CategoryId.HasValue && model.CategoryId > 0)
            {
                var categoryExists = await _studentCategoryService.FirstOrDefaultAsync(x => x.Id == model.CategoryId && !x.IsDeleted);
                if (categoryExists == null)
                    return (false, "Selected category does not exist");
            }

            // Validate school house if provided
            if (model.SchoolHouseId.HasValue && model.SchoolHouseId > 0)
            {
                var schoolHouseExists = await _schoolHouseService.FirstOrDefaultAsync(x => x.Id == model.SchoolHouseId && !x.IsDeleted);
                if (schoolHouseExists == null)
                    return (false, "Selected school house does not exist");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Handle flow-specific logic after student creation
        /// </summary>
        private async Task HandleFlowSpecificLogic(StudentCreateRequest model, int studentId, string AdmissionNo)
        {
            switch (model.Source.ToLower())
            {
                case "admission":
                    if (model.AdmissionId.HasValue)
                    {
                        // Mark admission as having student created
                        await _admissionService.MarkStudentCreated(model.AdmissionId.Value, studentId, AdmissionNo);
                    }
                    break;
                
                case "preadmission":
                case "application":
                    if (model.ApplicationId.HasValue)
                    {
                        // Migrate provisional fees from application to student
                        await _feeAssignmentService.MigrateProvisionalFees(new MigrateProvisionalFeesRequest
                        {
                            ApplicationId = model.ApplicationId.Value,
                            StudentId = studentId
                        });
                    }
                    break;
                
                case "workflow":
                    // Handle workflow-specific logic if needed
                    break;
                
                case "direct":
                default:
                    // No additional logic needed for direct creation
                    break;
            }
        }

        #endregion

        #region Account Creation Methods

        /// <summary>
        /// Create parent account for student
        /// </summary>
        private async Task<BaseModel> CreateParentAccount(StudentCreateRequest model)
        {
            try
            {
                // Determine primary parent (father takes precedence)
                var primaryParentName = !string.IsNullOrEmpty(model.FatherName) ? model.FatherName : model.MotherName;
                var primaryParentCNIC = !string.IsNullOrEmpty(model.FatherCNIC) ? model.FatherCNIC : model.MotherCNIC;
                var primaryParentPhone = !string.IsNullOrEmpty(model.FatherPhone) ? model.FatherPhone : model.MotherPhone;

                if (string.IsNullOrEmpty(primaryParentName) || string.IsNullOrEmpty(primaryParentCNIC))
                {
                    return new BaseModel { Success = false, Message = "Parent name and CNIC are required" };
                }

                // Check if parent already exists by multiple criteria
                // Priority: CNIC > Phone+Name > Email > Username
                ApplicationUserModel existingParent = null;
                
                // 1. Check by CNIC (most reliable identifier)
                if (!string.IsNullOrEmpty(primaryParentCNIC))
                {
                    var cnicExists = await _userService.IsCNICAlreadyExist(primaryParentCNIC, UserRoles.Parent.ToString());
                    if (cnicExists)
                    {
                        // Get user by CNIC - we need to get the full user object
                        existingParent = (await _userService.GetUser(primaryParentCNIC)).Data as ApplicationUserModel;
                    }
                }
                
                // 2. Check by Phone and Name (if not found by CNIC)
                if (existingParent == null && !string.IsNullOrEmpty(primaryParentPhone))
                {
                    existingParent = await _userService.GetUserByPhoneAndName(
                        primaryParentPhone, 
                        primaryParentName.Split(' ')[0], 
                        primaryParentName.Split(' ').Length > 1 ? string.Join(" ", primaryParentName.Split(' ').Skip(1)) : "");
                }
                
                // 3. Check by Email (if not found by CNIC or Phone+Name)
                if (existingParent == null && !string.IsNullOrEmpty(model.GuardianEmail))
                {
                    var emailExists = await _userService.IsEmailAlreadyExist(model.GuardianEmail);
                    if (emailExists)
                    {
                        var allUsers = await _userService.GetAllUsersByRole(UserRoles.Parent.ToString());
                        if (allUsers.Success && allUsers.Data != null)
                        {
                            var usersList = allUsers.Data as List<ApplicationUserModel>;
                            existingParent = usersList?.FirstOrDefault(u => u.Email == model.GuardianEmail);
                        }
                    }
                }
                
                // 4. Check by Username (if not found by any above criteria)
                if (existingParent == null)
                {
                    var username = $"parent_{primaryParentCNIC.Replace("-", "")}";
                    var userByUsername = await _userService.GetUser(username);
                    if (userByUsername.Success && userByUsername.Data != null)
                    {
                        existingParent = userByUsername.Data as ApplicationUserModel;
                    }
                }
                
                // If parent already exists, return existing account
                if (existingParent != null)
                {
                    return new BaseModel { Success = true, Data = existingParent, Message = "Parent account already exists" };
                }

                // Create new parent account
                var parentRequest = new RegisterUserModel
                {
                    FirstName = primaryParentName.Split(' ')[0],
                    LastName = primaryParentName.Split(' ').Length > 1 ? string.Join(" ", primaryParentName.Split(' ').Skip(1)) : "",
                    CNIC = primaryParentCNIC,
                    PhoneNumber = primaryParentPhone,
                    Email = string.IsNullOrEmpty(model.GuardianEmail) ?
                                EmailGenerateHelper.GenerateEmail(model.GuardianName.Replace(" ", ""), "gmail.com") :
                                model.GuardianEmail,
                    UserName = $"parent_{primaryParentCNIC.Replace("-", "")}",
                    Password = "123", // Temporary password
                    ConfirmPassword = "123",
                    Roles = new List<string> { UserRoles.Parent.ToString() }
                };

                var parentResult = await _userService.CreateUser(parentRequest);
                if (parentResult.Status == "Succeded" || !string.IsNullOrEmpty(parentResult.UserId))
                {
                    return new BaseModel { Success = true, Data = parentResult, Message = "Parent account created successfully" };
                }
                else
                {
                    return new BaseModel { Success = false, Message = parentResult.Message };
                }
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Message = $"Error creating parent account: {ex.Message}" };
            }
        }

        /// <summary>
        /// Create student account
        /// </summary>
        private async Task<BaseModel> CreateStudentAccount(StudentCreateRequest model, string username, string email)
        {
            try
            {
                // Create student account
                var studentAccountRequest = new RegisterUserModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = email, // Use provided email
                    PhoneNumber = string.IsNullOrEmpty(model.PhoneNo) ? model.GuardianPhone : model.PhoneNo, // Use student phone or guardian phone
                    UserName = username,
                    Password = "TempPassword123!", // Temporary password
                    ConfirmPassword = "TempPassword123!",
                    Roles = new List<string> { UserRoles.Student.ToString() },
                    ParentId = model.ParentId // Link to parent account
                };

                var studentAccountResult = await _userService.CreateUser(studentAccountRequest);
                if (studentAccountResult.Status == "Succeded" || !string.IsNullOrEmpty(studentAccountResult.UserId))
                {
                    return new BaseModel { Success = true, Data = studentAccountResult, Message = "Student account created successfully" };
                }
                else
                {
                    return new BaseModel { Success = false, Message = studentAccountResult.Message };
                }
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Message = $"Error creating student account: {ex.Message}" };
            }
        }

        /// <summary>
        /// Check if student account already exists in the identity table (AspNetUsers)
        /// </summary>
        private async Task<ApplicationUserModel> CheckExistingStudentAccount(StudentCreateRequest model)
        {
            try
            {
                // Check if student account exists in identity table by multiple criteria
                ApplicationUserModel existingStudentAccount = null;
                
                // 1. Check by email (if provided)
                var allStudents = await _userService.GetAllUsersByRole(UserRoles.Student.ToString());
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var emailExists = await _userService.IsEmailAlreadyExist(model.Email);
                    if (emailExists)
                    {
                        if (allStudents.Success && allStudents.Data != null)
                        {
                            var studentsList = mapper.Map<List<ApplicationUserModel>>(allStudents.Data);
                            existingStudentAccount = studentsList?.FirstOrDefault(u => u.Email == model.Email);
                        }
                    }
                }
                
                // 2. Check by phone number (if provided)
                if (existingStudentAccount == null && !string.IsNullOrEmpty(model.PhoneNo))
                {
                    var phoneExists = await _userService.IsPhoneAlreadyExist(model.PhoneNo);
                    if (phoneExists)
                    {
                        if (allStudents.Success && allStudents.Data != null)
                        {
                            var studentsList = mapper.Map<List<ApplicationUserModel>>(allStudents.Data);
                            existingStudentAccount = studentsList?.FirstOrDefault(u => u.PhoneNumber == model.PhoneNo);
                        }
                    }
                }
                
                // 3. Check by name and parent (most comprehensive check)
                if (existingStudentAccount == null && !string.IsNullOrEmpty(model.ParentId))
                {
                    var isStudentExist = await _userService.IsStudentExist(model.Email ?? "", model.ParentId);
                    if (isStudentExist)
                    {
                        if (allStudents.Success && allStudents.Data != null)
                        {
                            var studentsList = mapper.Map<List<ApplicationUserModel>>(allStudents.Data);
                            // Find student by name and parent
                            existingStudentAccount = studentsList?.FirstOrDefault(u => 
                                u.FirstName.ToLower() == model.FirstName.ToLower() &&
                                u.LastName.ToLower() == (model.LastName ?? "").ToLower() &&
                                u.ParentId == model.ParentId);
                        }
                    }
                }

                return existingStudentAccount; // Returns ApplicationUserModel if found, null otherwise
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                return null; // Return null to allow creation if check fails
            }
        }

        /// <summary>
        /// Generate unique username and email for student account
        /// </summary>
        private async Task<(string username, string email)> GenerateUniqueStudentCredentials(StudentCreateRequest model)
        {
            // Use admission number if available, otherwise use name-based approach
            var baseUsername = !string.IsNullOrEmpty(model.AdmissionNo) 
                ? $"student_{model.AdmissionNo}" 
                : $"student_{model.FirstName.ToLower().Replace(" ", "")}_{model.LastName?.ToLower().Replace(" ", "") ?? ""}";

            // Generate unique email - use provided email if available, otherwise generate one
            string email;
            if (!string.IsNullOrEmpty(model.Email))
            {
                email = model.Email;
            }
            else
            {
                // Generate unique email using timestamp and random number
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var randomSuffix = new Random().Next(1000, 9999);
                var baseEmail = $"{model.FirstName.ToLower().Replace(" ", "")}.{model.LastName?.ToLower().Replace(" ", "") ?? ""}";
                email = $"{baseEmail}_{timestamp}_{randomSuffix}@student.school.com";
            }

            // Generate unique username using timestamp and random number
            var timestampForUsername = DateTime.Now.ToString("yyyyMMddHHmmss");
            var randomSuffixForUsername = new Random().Next(1000, 9999);
            var username = $"{baseUsername}_{timestampForUsername}_{randomSuffixForUsername}";

            return (username, email);
        }

        /// <summary>
        /// Create fee assignments for student
        /// </summary>
        private async Task CreateFeeAssignments(int studentId, StudentCreateRequest model)
        {
            try
            {
                if (!model.MonthlyFees.HasValue || model.MonthlyFees <= 0)
                {
                    return; // Skip if no monthly fees
                }

                // Get current session
                var currentSession = await _sessionService.GetActiveSessionId();
                if (currentSession == 0)
                {
                    throw new Exception("No active session found. Please set an active academic session.");
                }

              // Create monthly fee assignments for the academic year
              var feeAssignments = new List<FeeAssignmentModel>();
              var startMonth = DateTime.Now.Month;
              var endMonth = 12; // Academic year typically ends in December

              for (int month = startMonth; month <= endMonth; month++)
              {
                  var feeAssignment = new FeeAssignmentModel
                  {
                      StudentId = studentId,
                      FeeGroupFeeTypeId = 1, // Default fee type ID for monthly fees
                      Amount = model.MonthlyFees.Value,
                      Month = month,
                      Year = DateTime.Now.Year,
                      DueDate = new DateTime(DateTime.Now.Year, month, 1).AddMonths(1).AddDays(-1), // Last day of month
                      IsPartialPaymentAllowed = true
                  };
                  feeAssignments.Add(feeAssignment);
              }

              // Create fee assignments individually since batch method might not exist
              foreach (var feeAssignment in feeAssignments)
              {
                  await _feeAssignmentService.Add(feeAssignment);
              }
            }
            catch (Exception ex)
            {
                // Log error but don't fail student creation
                Console.WriteLine($"Error creating fee assignments: {ex.Message}");
            }
        }

        /// <summary>
        /// Create fee assignments from fee package for student
        /// </summary>
        private async Task CreateFeeAssignmentsFromPackage(int studentId, StudentCreateRequest model)
        {
            try
            {
                if (!model.FeeGroupFeeTypeId.HasValue)
                {
                    return; // Skip if no fee package selected
                }

                Console.WriteLine($"📝 Creating fee assignments from package {model.FeeGroupFeeTypeId} for student {studentId}");

                // Get fee preview for the selected package
                var feePreviewRequest = new FeePreviewRequest
                {
                    FeeGroupFeeTypeId = model.FeeGroupFeeTypeId.Value,
                    ClassId = model.ClassId
                };

                var feePreviewResult = await _feeAssignmentService.GetFeePreview(feePreviewRequest);
                if (!feePreviewResult.Success || feePreviewResult.Data == null)
                {
                    Console.WriteLine($"❌ Failed to get fee preview: {feePreviewResult.Message}");
                    return;
                }

                var feePreview = feePreviewResult.Data as FeePreviewResponse;
                if (feePreview == null || feePreview.FeeTypes == null)
                {
                    Console.WriteLine($"❌ Fee preview data is null or invalid");
                    return;
                }

                Console.WriteLine($"✅ Fee preview loaded with {feePreview.FeeTypes.Count} fee types");

                // Get the FeeGroupFeeTypeId from the preview response
                var feeGroupFeeTypeId = feePreview.FeeGroupFeeTypeId ?? model.FeeGroupFeeTypeId.Value;

                // Create fee assignments for each fee type in the package
                foreach (var feeType in feePreview.FeeTypes)
                {
                    if (feeType.FeeFrequency == "Monthly")
                    {
                        // ✅ For monthly fees, create assignment(s) based on plan
                        if (!model.ApplyMonth.HasValue || !model.ApplyYear.HasValue)
                        {
                            Console.WriteLine($"⚠️ Skipping monthly fee {feeType.FeeTypeName} - no month/year specified");
                            continue;
                        }

                        int monthsToCreate = 1; // Default: single month

                        if (model.CreateFullYearPlan)
                        {
                            // ✅ Full year plan: 12 months
                            monthsToCreate = 12;
                            Console.WriteLine($"📅 Creating full year plan (12 months) for {feeType.FeeTypeName}");
                        }
                        else if (model.NumberOfMonths.HasValue && model.NumberOfMonths.Value > 0)
                        {
                            // ✅ Partial plan: specified number of months
                            monthsToCreate = model.NumberOfMonths.Value;
                            Console.WriteLine($"📅 Creating {monthsToCreate} months plan for {feeType.FeeTypeName}");
                        }

                        // Create fee assignments for each month
                        for (int i = 0; i < monthsToCreate; i++)
                        {
                            var targetDate = new DateTime(model.ApplyYear.Value, model.ApplyMonth.Value, 1).AddMonths(i);
                            var targetMonth = targetDate.Month;
                            var targetYear = targetDate.Year;
                            var dueDate = targetDate.AddMonths(1).AddDays(-1); // Last day of month

                            var assignment = new FeeAssignmentModel
                            {
                                StudentId = studentId,
                                ApplicationId = null,  // ✅ Direct to student (not provisional)
                                FeeGroupFeeTypeId = feeGroupFeeTypeId,
                                Amount = feeType.Amount,
                                Month = targetMonth,
                                Year = targetYear,
                                DueDate = dueDate,
                                IsPartialPaymentAllowed = true,
                                CreatedBy = model.CreatedBy ?? "System",
                                // ✅ Mark as initial assignment from fee package
                                IsInitialAssignment = true,
                                InitialPackageId = feeGroupFeeTypeId.ToString(),
                                Description = $"{feeType.FeeTypeName} - {targetDate:MMMM yyyy}"
                            };

                            await _feeAssignmentService.Add(assignment);
                            Console.WriteLine($"✅ Created monthly fee: {feeType.FeeTypeName} for {targetMonth}/{targetYear}");
                        }

                        Console.WriteLine($"✅ Created {monthsToCreate} month(s) of {feeType.FeeTypeName}");
                    }
                    else
                    {
                        // ✅ For one-time, quarterly, annual fees
                        var assignment = new FeeAssignmentModel
                        {
                            StudentId = studentId,
                            ApplicationId = null,
                            FeeGroupFeeTypeId = feeGroupFeeTypeId,  // ✅ Use FeeGroupFeeTypeId from preview
                            Amount = feeType.Amount,
                            Month = 0,  // ✅ 0 = non-monthly fee
                            Year = 0,   // ✅ 0 = no specific year (non-monthly fee)
                            DueDate = feeType.DueDate ?? DateTime.Now.AddMonths(1),
                            IsPartialPaymentAllowed = true,
                            CreatedBy = model.CreatedBy ?? "System",
                            // ✅ Mark as initial assignment from fee package
                            IsInitialAssignment = true,
                            InitialPackageId = feeGroupFeeTypeId.ToString()
                        };

                        await _feeAssignmentService.Add(assignment);
                        Console.WriteLine($"✅ Created {feeType.FeeFrequency} fee assignment: {feeType.FeeTypeName}");
                    }
                }

                Console.WriteLine($"✅ Successfully created fee assignments from package for student {studentId}");
            }
            catch (Exception ex)
            {
                // Log error but don't fail student creation
                Console.WriteLine($"❌ Error creating fee assignments from package: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            }
        }


        #endregion

        #region Bulk Import Methods

        // In-memory storage for import jobs (in production, use Redis or database)
        private static readonly Dictionary<string, Common.DTO.Response.ImportJobStatus> _importJobs = new Dictionary<string, Common.DTO.Response.ImportJobStatus>();
        private static readonly object _importJobsLock = new object();

        /// <summary>
        /// Start bulk student import process
        /// </summary>
        public async Task<BaseModel> StartBulkImportAsync(Common.DTO.Request.BulkStudentImportRequest request)
        {
            try
            {
                var jobId = Guid.NewGuid().ToString();
                
                // Initialize job status
                var jobStatus = new ImportJobStatus
                {
                    JobId = jobId,
                    TotalRecords = request.students.Count,
                    ProcessedRecords = 0,
                    SuccessfulRecords = 0,
                    FailedRecords = 0,
                    Status = "Pending",
                    StartTime = DateTime.UtcNow,
                    Errors = new List<string>()
                };

                lock (_importJobsLock)
                {
                    _importJobs[jobId] = jobStatus;
                }

                // Start background processing
                _jobs.Enqueue(() => ProcessBulkImportJobAsync(jobId, request));

                return new BaseModel
                {
                    Success = true,
                    Message = "Bulk import started successfully",
                    Data = new { JobId = jobId }
                };
            }
            catch (Exception ex)
            {
                return new BaseModel
                {
                    Success = false,
                    Message = $"Failed to start bulk import: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Get import job status
        /// </summary>
        public async Task<BaseModel> GetImportStatusAsync(string jobId)
        {
            lock (_importJobsLock)
            {
                if (_importJobs.ContainsKey(jobId))
                {
                    var status = _importJobs[jobId];
                    Console.WriteLine($"📊 Job {jobId} status: {status.Status}, Processed: {status.ProcessedRecords}/{status.TotalRecords}");
                    return new BaseModel
                    {
                        Success = true,
                        Data = status,
                        Message = "Import status retrieved successfully"
                    };
                }
                else
                {
                    Console.WriteLine($"❌ Job {jobId} not found in dictionary. Available jobs: {string.Join(", ", _importJobs.Keys)}");
                    return new BaseModel
                    {
                        Success = false,
                        Message = "Import job not found"
                    };
                }
            }
        }

        /// <summary>
        /// Process bulk import job in background
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task ProcessBulkImportJobAsync(string jobId, Common.DTO.Request.BulkStudentImportRequest request)
        {
            Console.WriteLine($"🚀 Starting job processing for {jobId} with {request.students.Count} students");
            ImportJobStatus jobStatus;
            
            lock (_importJobsLock)
            {
                if (!_importJobs.ContainsKey(jobId))
                {
                    Console.WriteLine($"❌ Job {jobId} not found in dictionary");
                    return;
                }
                
                jobStatus = _importJobs[jobId];
                jobStatus.Status = "Processing";
                Console.WriteLine($"📊 Job {jobId} status set to Processing");
            }

            try
            {
                foreach (var studentRow in request.students)
                {
                    try
                    {
                        // Update current processing record
                        lock (_importJobsLock)
                        {
                            jobStatus.CurrentProcessingRecord = studentRow.FullName;
                        }
                        // Get active session ID
                        var activeSession = await _sessionService.GetActiveSessionId();
                        if (activeSession == 0)
                        {
                            throw new Exception("No active session found. Please set an active academic session.");
                        }
                        // Convert StudentImportRow to UnifiedStudentCreateRequest
                        var studentRequest = ConvertToUnifiedRequest(studentRow, request.CreatedBy, activeSession);

                        // Create student using existing method
                        var result = await CreateStudentUnified(studentRequest);

                        if (result.Success)
                        {
                            lock (_importJobsLock)
                            {
                                jobStatus.SuccessfulRecords++;
                            }
                        }
                        else
                        {
                            lock (_importJobsLock)
                            {
                                jobStatus.FailedRecords++;
                                jobStatus.DetailedErrors.Add(new ImportError
                                {
                                    RowNumber = studentRow.RowNumber,
                                    StudentName = studentRow.FullName,
                                    ErrorMessage = result.Message,
                                    FieldName = "General"
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (_importJobsLock)
                        {
                            jobStatus.FailedRecords++;
                            jobStatus.DetailedErrors.Add(new ImportError
                            {
                                RowNumber = studentRow.RowNumber,
                                StudentName = studentRow.FullName,
                                ErrorMessage = ex.Message,
                                FieldName = "General"
                            });
                        }
                    }

                    // Update processed count
                    lock (_importJobsLock)
                    {
                        jobStatus.ProcessedRecords++;
                    }

                    // Small delay to prevent overwhelming the system and make progress visible
                    await Task.Delay(2000); // 2 second delay to make progress visible
                }

                // Mark job as completed
                lock (_importJobsLock)
                {
                    jobStatus.Status = "Completed";
                    jobStatus.EndTime = DateTime.UtcNow;
                    Console.WriteLine($"✅ Job {jobId} completed successfully. Processed: {jobStatus.ProcessedRecords}/{jobStatus.TotalRecords}");
                }
            }
            catch (Exception ex)
            {
                lock (_importJobsLock)
                {
                    jobStatus.Status = "Failed";
                    jobStatus.EndTime = DateTime.UtcNow;
                    jobStatus.Errors.Add($"Job failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Convert StudentImportRow to UnifiedStudentCreateRequest
        /// </summary>
        private StudentCreateRequest ConvertToUnifiedRequest(Common.DTO.Request.StudentImportRow row, string createdBy, int sessionId)
        {
            return new StudentCreateRequest
            {
                FirstName = row.FullName?.Split(' ').FirstOrDefault() ?? "Unknown",
                LastName = row.FullName?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                DateOfBirth = row.DateOfBirth ?? DateTime.Now.AddYears(-10), // Default age if not provided
                Gender = row.Gender,
                PhoneNo = row.PhoneNo,
                Email = row.Email,
                CurrentAddress = row.Address,
                FatherCNIC = row.FatherCNIC,
                FatherName = row.FatherName,
                FatherPhone = row.FatherPhone,
                MotherCNIC = row.MotherCNIC,
                MotherName = row.MotherName,
                MotherPhone = row.MotherPhone,
                GuardianType = row.GuardianType,
                GuardianName = row.GuardianName,
                GuardianPhone = row.GuardianPhone,
                GuardianEmail = row.GuardianEmail,
                RollNo = row.RollNo,
                AdmissionNo = row.AdmissionNo,
                AdmissionDate = row.AdmissionDate ?? DateTime.Now,
                MonthlyFees = row.MonthlyFees ?? 0,
                AdmissionFees = row.AdmissionFees ?? 0,
                SecurityDeposit = row.SecurityDeposit ?? 0,
                Height = row.Height,
                Weight = row.Weight,
                MeasurementDate = row.MeasurementDate ?? DateTime.Now,
                BloodGroup = row.BloodGroup,
                Religion = row.Religion,
                Cast = row.Cast,
                MedicalHistory = row.MedicalHistory,
                Note = row.Notes,
                ClassId = ParseClassId(row.Class),
                SectionId = ParseSectionId(row.Section),
                SessionId = sessionId,
                CreatedBy = createdBy,
                Source = "BulkImport"
            };
        }

        /// <summary>
        /// Parse class name to class ID
        /// </summary>
        private int ParseClassId(string className)
        {
            if (string.IsNullOrEmpty(className))
                return 1; // Default class

            if (int.TryParse(className, out int classId))
                return classId;

            // You might want to add logic to map class names to IDs
            return 1; // Default fallback
        }

        /// <summary>
        /// Parse section name to section ID
        /// </summary>
        private int ParseSectionId(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
                return 1; // Default section

            // You might want to add logic to map section names to IDs
            return 1; // Default fallback
        }

        /// <summary>
        /// Cancel import job
        /// </summary>
        public async Task<BaseModel> CancelImportJobAsync(string jobId)
        {
            lock (_importJobsLock)
            {
                if (_importJobs.ContainsKey(jobId))
                {
                    _importJobs[jobId].Status = "Failed";
                    _importJobs[jobId].EndTime = DateTime.UtcNow;
                    _importJobs[jobId].Errors.Add("Job cancelled by user");
                }
            }

            return new BaseModel
            {
                Success = true,
                Message = "Import job cancelled successfully"
            };
        }

        /// <summary>
        /// Get all import jobs (for admin monitoring)
        /// </summary>
        public async Task<List<Common.DTO.Response.ImportJobStatus>> GetAllImportJobsAsync()
        {
            lock (_importJobsLock)
            {
                return _importJobs.Values.ToList();
            }
        }

        /// <summary>
        /// Download import template as Excel file
        /// </summary>
        public async Task<byte[]> DownloadImportTemplateAsync()
        {
            try
            {
                return await _excelParserService.GenerateImportTemplateAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate import template: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Parse uploaded file for bulk import
        /// </summary>
        public async Task<BaseModel> ParseImportFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new BaseModel { Success = false, Message = "No file uploaded" };
                }

                using var stream = file.OpenReadStream();
                var parsedData = await _excelParserService.ParseExcelFileAsync(stream);
                // Get active session ID
                var activeSession = await _sessionService.GetActiveSessionId();
                if (activeSession == 0)
                {
                    throw new Exception("No active session found. Please set an active academic session.");
                }
                // Create BulkStudentImportRequest structure
                var bulkRequest = new Common.DTO.Request.BulkStudentImportRequest
                {
                    students = parsedData,
                    CreatedBy = "System", // TODO: Get from current user context
                    SessionId = activeSession // TODO: Get from current session
                };
                
                return new BaseModel
                {
                    Success = true,
                    Message = "File parsed successfully",
                    Data = bulkRequest
                };
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Message = $"Failed to parse file: {ex.Message}" };
            }
        }

        /// <summary>
        /// Stream import progress using Server-Sent Events
        /// </summary>
        public async Task StreamImportProgressAsync(string jobId, HttpResponse response)
        {
            Console.WriteLine($"📡 Starting SSE stream for job: {jobId}");
            
            try
            {
                // Send initial connection message
                await response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(new { type = "connected", jobId = jobId })}\n\n");
                await response.Body.FlushAsync();

                var lastStatus = "";
                var lastProcessedCount = -1;
                var isJobCompleted = false;

                while (!isJobCompleted)
                {
                    ImportJobStatus? currentStatus = null;
                    
                    lock (_importJobsLock)
                    {
                        if (_importJobs.ContainsKey(jobId))
                        {
                            currentStatus = _importJobs[jobId];
                        }
                    }

                    if (currentStatus == null)
                    {
                        // Job not found or completed
                        await response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(new { type = "error", message = "Job not found" })}\n\n");
                        await response.Body.FlushAsync();
                        break;
                    }

                    // Send update if status or progress changed
                    if (currentStatus.Status != lastStatus || currentStatus.ProcessedRecords != lastProcessedCount)
                    {
                        var progressData = new
                        {
                            type = "progress",
                            jobId = jobId,
                            status = currentStatus.Status,
                            totalRecords = currentStatus.TotalRecords,
                            processedRecords = currentStatus.ProcessedRecords,
                            successfulRecords = currentStatus.SuccessfulRecords,
                            failedRecords = currentStatus.FailedRecords,
                            currentProcessingRecord = currentStatus.CurrentProcessingRecord,
                            progressPercentage = currentStatus.ProgressPercentage,
                            errors = currentStatus.Errors,
                            detailedErrors = currentStatus.DetailedErrors
                        };

                        await response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(progressData)}\n\n");
                        await response.Body.FlushAsync();

                        lastStatus = currentStatus.Status;
                        lastProcessedCount = currentStatus.ProcessedRecords;

                        Console.WriteLine($"📡 SSE update sent for job {jobId}: {currentStatus.Status} ({currentStatus.ProcessedRecords}/{currentStatus.TotalRecords})");
                    }

                    // Check if job is completed
                    if (currentStatus.Status == "Completed" || currentStatus.Status == "Failed")
                    {
                        isJobCompleted = true;
                        
                        // Send final completion message
                        var completionData = new
                        {
                            type = "completed",
                            jobId = jobId,
                            status = currentStatus.Status,
                            totalRecords = currentStatus.TotalRecords,
                            successfulRecords = currentStatus.SuccessfulRecords,
                            failedRecords = currentStatus.FailedRecords,
                            endTime = currentStatus.EndTime
                        };

                        await response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(completionData)}\n\n");
                        await response.Body.FlushAsync();
                        
                        Console.WriteLine($"📡 SSE stream completed for job {jobId}: {currentStatus.Status}");
                    }

                    // Wait before next check
                    await Task.Delay(1000); // Check every second
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SSE stream error for job {jobId}: {ex.Message}");
                try
                {
                    await response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(new { type = "error", message = ex.Message })}\n\n");
                    await response.Body.FlushAsync();
                }
                catch
                {
                    // Ignore errors when writing error response
                }
            }
        }

        #endregion

        #region Update Student Helper Methods

        /// <summary>
        /// Change tracking class to detect what has changed in student data
        /// </summary>
        private class StudentChanges
        {
            public bool AdmissionNoChanged { get; set; }
            public bool RollNoChanged { get; set; }
            public bool NameChanged { get; set; }
            public bool EmailChanged { get; set; }
            public bool PhoneChanged { get; set; }
            public bool ParentCNICChanged { get; set; }
            public bool GuardianTypeChanged { get; set; }
            public bool ParentInfoChanged { get; set; }
            public bool ClassOrSectionChanged { get; set; }
            public bool FeesChanged { get; set; }
            public bool ImageChanged { get; set; }
            public bool AddressChanged { get; set; }
            public bool MedicalInfoChanged { get; set; }
            
            // Detailed change log for audit
            public List<string> ChangeLog { get; set; } = new List<string>();
        }

        /// <summary>
        /// Detect all changes between existing student and update model
        /// </summary>
        private StudentChanges DetectChanges(Student existingStudent, StudentCreateRequest model)
        {
            var changes = new StudentChanges();
            var info = existingStudent.StudentInfo;
            var address = existingStudent.StudentAddress;
            var financial = existingStudent.StudentFinancial;
            var parent = existingStudent.StudentParent;
            var misc = existingStudent.StudentMiscellaneous;
            var currentClassAssignment = existingStudent.ClassAssignments?.FirstOrDefault();

            // Student Info Changes
            if (info?.AdmissionNo != model.AdmissionNo)
            {
                changes.AdmissionNoChanged = true;
                changes.ChangeLog.Add($"AdmissionNo: '{info?.AdmissionNo}' → '{model.AdmissionNo}'");
            }

            if (info?.RollNo != model.RollNo)
            {
                changes.RollNoChanged = true;
                changes.ChangeLog.Add($"RollNo: '{info?.RollNo}' → '{model.RollNo}'");
            }

            if (info?.FirstName != model.FirstName || info?.LastName != model.LastName)
            {
                changes.NameChanged = true;
                changes.ChangeLog.Add($"Name: '{info?.FirstName} {info?.LastName}' → '{model.FirstName} {model.LastName}'");
            }

            if (info?.Email != model.Email)
            {
                changes.EmailChanged = true;
                changes.ChangeLog.Add($"Email: '{info?.Email}' → '{model.Email}'");
            }

            if (info?.PhoneNo != model.PhoneNo || info?.MobileNo != model.MobileNo)
            {
                changes.PhoneChanged = true;
                changes.ChangeLog.Add($"Phone: '{info?.PhoneNo}/{info?.MobileNo}' → '{model.PhoneNo}/{model.MobileNo}'");
            }

            if (info?.Image != model.Image)
            {
                changes.ImageChanged = true;
                changes.ChangeLog.Add("Image updated");
            }

            // Parent CNIC Changes (Critical)
            if (parent?.FatherCNIC != model.FatherCNIC || parent?.MotherCNIC != model.MotherCNIC)
            {
                changes.ParentCNICChanged = true;
                changes.ChangeLog.Add($"Parent CNIC changed - Father: '{parent?.FatherCNIC}' → '{model.FatherCNIC}', Mother: '{parent?.MotherCNIC}' → '{model.MotherCNIC}'");
            }

            // Guardian Type Change (Critical)
            if (parent?.GuardianIs != model.GuardianType)
            {
                changes.GuardianTypeChanged = true;
                changes.ChangeLog.Add($"GuardianType: '{parent?.GuardianIs}' → '{model.GuardianType}'");
            }

            // Parent Info Changes (Non-critical)
            if (parent?.FatherName != model.FatherName || 
                parent?.FatherPhone != model.FatherPhone ||
                parent?.MotherName != model.MotherName || 
                parent?.MotherPhone != model.MotherPhone ||
                parent?.GuardianName != model.GuardianName ||
                parent?.GuardianPhone != model.GuardianPhone ||
                parent?.GuardianEmail != model.GuardianEmail)
            {
                changes.ParentInfoChanged = true;
                changes.ChangeLog.Add("Parent contact information updated");
            }

            // Class/Section Changes
            if (currentClassAssignment?.ClassId != model.ClassId || 
                currentClassAssignment?.SectionId != model.SectionId)
            {
                changes.ClassOrSectionChanged = true;
                changes.ChangeLog.Add($"Class/Section: '{currentClassAssignment?.ClassId}/{currentClassAssignment?.SectionId}' → '{model.ClassId}/{model.SectionId}'");
            }

            // Financial Changes
            if (financial?.MonthlyFees != model.MonthlyFees || 
                financial?.AdmissionFees != (float?)model.AdmissionFees ||
                financial?.SecurityDeposit != (float?)model.SecurityDeposit)
            {
                changes.FeesChanged = true;
                changes.ChangeLog.Add($"Fees updated - Monthly: {financial?.MonthlyFees} → {model.MonthlyFees}");
            }

            // Address Changes
            if (address?.CurrentAddress != model.CurrentAddress || 
                address?.PermanentAddress != model.PermanentAddress ||
                address?.City != model.City ||
                address?.State != model.State)
            {
                changes.AddressChanged = true;
                changes.ChangeLog.Add("Address information updated");
            }

            // Medical Info Changes
            if (info?.BloodGroup != model.BloodGroup ||
                info?.Height != model.Height ||
                info?.Weight != model.Weight ||
                misc?.MedicalHistory != model.MedicalHistory)
            {
                changes.MedicalInfoChanged = true;
                changes.ChangeLog.Add("Medical information updated");
            }

            return changes;
        }

        /// <summary>
        /// Handle parent account change when CNIC or Guardian Type changes
        /// </summary>
        private async Task<BaseModel> HandleParentAccountChange(StudentCreateRequest model, Student existingStudent)
        {
            try
            {
                // Find or create new parent account
                var newParentResult = await CreateParentAccount(model);
                if (!newParentResult.Success)
                {
                    return new BaseModel { Success = false, Message = $"Failed to create/find new parent account: {newParentResult.Message}" };
                }

                // Extract new parent ID
                string newParentId = null;
                if (newParentResult.Data is LoginResponse loginResp)
                {
                    newParentId = loginResp.UserId;
                }
                else if (newParentResult.Data is ApplicationUserModel appUser)
                {
                    newParentId = appUser.Id;
                }

                if (string.IsNullOrEmpty(newParentId))
                {
                    return new BaseModel { Success = false, Message = "Failed to retrieve new parent account ID" };
                }

                // Update student's ParentId in StudentInfo
                existingStudent.StudentInfo.ParentId = newParentId;

                // Update student account's ParentId in AspNetUsers
                if (!string.IsNullOrEmpty(existingStudent.StudentInfo.StudentUserId))
                {
                    var studentUser = await _applicationUserManager.FindByIdAsync(existingStudent.StudentInfo.StudentUserId);
                    if (studentUser != null)
                    {
                        studentUser.ParentId = newParentId;
                        await _applicationUserManager.UpdateAsync(studentUser);
                    }
                }

                return new BaseModel { Success = true, Data = newParentId };
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Message = $"Error handling parent account change: {ex.Message}" };
            }
        }

        /// <summary>
        /// Update parent account information (name, phone, email) without changing parent account
        /// </summary>
        private async Task<BaseModel> UpdateParentAccountInfo(StudentCreateRequest model, Student existingStudent)
        {
            try
            {
                var parentId = existingStudent.StudentInfo.ParentId;
                if (string.IsNullOrEmpty(parentId))
                {
                    return new BaseModel { Success = true, Message = "No parent account to update" };
                }

                var parentUser = await _applicationUserManager.FindByIdAsync(parentId);
                if (parentUser == null)
                {
                    return new BaseModel { Success = true, Message = "Parent account not found in identity system" };
                }

                // Update parent account based on GuardianType
                var primaryParentName = model.GuardianType == "Father" ? model.FatherName :
                                       model.GuardianType == "Mother" ? model.MotherName :
                                       model.GuardianName;
                var primaryParentPhone = model.GuardianType == "Father" ? model.FatherPhone :
                                        model.GuardianType == "Mother" ? model.MotherPhone :
                                        model.GuardianPhone;

                bool updated = false;

                // Update name if provided
                if (!string.IsNullOrEmpty(primaryParentName))
                {
                    var nameParts = primaryParentName.Split(' ');
                    parentUser.FirstName = nameParts[0];
                    parentUser.LastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
                    updated = true;
                }

                // Update phone if provided and different
                if (!string.IsNullOrEmpty(primaryParentPhone) && parentUser.PhoneNumber != primaryParentPhone)
                {
                    // Check phone uniqueness
                    var existingUserWithPhone = await _applicationUserManager.Users
                        .FirstOrDefaultAsync(u => u.PhoneNumber == primaryParentPhone && u.Id != parentId);
                    
                    if (existingUserWithPhone != null)
                    {
                        return new BaseModel { Success = false, Message = "Phone number already in use by another account" };
                    }

                    parentUser.PhoneNumber = primaryParentPhone;
                    updated = true;
                }

                // Update email if provided and different
                if (!string.IsNullOrEmpty(model.GuardianEmail) && parentUser.Email != model.GuardianEmail)
                {
                    // Check email uniqueness
                    var existingUserWithEmail = await _applicationUserManager.FindByEmailAsync(model.GuardianEmail);
                    if (existingUserWithEmail != null && existingUserWithEmail.Id != parentId)
                    {
                        return new BaseModel { Success = false, Message = "Email already in use by another account" };
                    }

                    parentUser.Email = model.GuardianEmail;
                    parentUser.UserName = model.GuardianEmail; // Update username to match email
                    updated = true;
                }

                if (updated)
                {
                    var result = await _applicationUserManager.UpdateAsync(parentUser);
                    if (!result.Succeeded)
                    {
                        return new BaseModel { Success = false, Message = $"Failed to update parent account: {string.Join(", ", result.Errors.Select(e => e.Description))}" };
                    }
                }

                return new BaseModel { Success = true, Message = "Parent account updated successfully" };
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Message = $"Error updating parent account info: {ex.Message}" };
            }
        }

        /// <summary>
        /// Update student account in AspNetUsers (email, phone, name)
        /// </summary>
        private async Task<BaseModel> UpdateStudentAccount(StudentCreateRequest model, Student existingStudent)
        {
            try
            {
                var studentUserId = existingStudent.StudentInfo.StudentUserId;
                if (string.IsNullOrEmpty(studentUserId))
                {
                    return new BaseModel { Success = true, Message = "No student account to update" };
                }

                var studentUser = await _applicationUserManager.FindByIdAsync(studentUserId);
                if (studentUser == null)
                {
                    return new BaseModel { Success = true, Message = "Student account not found in identity system" };
                }

                bool updated = false;

                // Update name
                if (studentUser.FirstName != model.FirstName || studentUser.LastName != model.LastName)
                {
                    studentUser.FirstName = model.FirstName;
                    studentUser.LastName = model.LastName;
                    updated = true;
                }

                // Update phone
                var primaryPhone = !string.IsNullOrEmpty(model.MobileNo) ? model.MobileNo : model.PhoneNo;
                if (!string.IsNullOrEmpty(primaryPhone) && studentUser.PhoneNumber != primaryPhone)
                {
                    // Check phone uniqueness
                    var existingUserWithPhone = await _applicationUserManager.Users
                        .FirstOrDefaultAsync(u => u.PhoneNumber == primaryPhone && u.Id != studentUserId);
                    
                    if (existingUserWithPhone != null)
                    {
                        return new BaseModel { Success = false, Message = "Phone number already in use by another account" };
                    }

                    studentUser.PhoneNumber = primaryPhone;
                    updated = true;
                }

                // Update email
                if (!string.IsNullOrEmpty(model.Email) && studentUser.Email != model.Email)
                {
                    // Check email uniqueness
                    var existingUserWithEmail = await _applicationUserManager.FindByEmailAsync(model.Email);
                    if (existingUserWithEmail != null && existingUserWithEmail.Id != studentUserId)
                    {
                        return new BaseModel { Success = false, Message = "Email already in use by another account" };
                    }

                    studentUser.Email = model.Email;
                    // Optionally update username to match email
                    // studentUser.UserName = model.Email;
                    updated = true;
                }

                if (updated)
                {
                    var result = await _applicationUserManager.UpdateAsync(studentUser);
                    if (!result.Succeeded)
                    {
                        return new BaseModel { Success = false, Message = $"Failed to update student account: {string.Join(", ", result.Errors.Select(e => e.Description))}" };
                    }
                }

                return new BaseModel { Success = true, Message = "Student account updated successfully" };
            }
            catch (Exception ex)
            {
                return new BaseModel { Success = false, Message = $"Error updating student account: {ex.Message}" };
            }
        }


        /// <summary>
        /// Validate student data with optional isUpdate flag to skip certain validations
        /// </summary>
        private async Task<(bool isValid, string errorMessage)> ValidateUnifiedStudentData(
            StudentCreateRequest model, 
            bool isUpdate = false)
        {
            // Basic required field validation
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                return (false, "First name is required");
            }

            if (string.IsNullOrWhiteSpace(model.Gender))
            {
                return (false, "Gender is required");
            }

            if (model.ClassId <= 0)
            {
                return (false, "Class is required");
            }

            if (model.SectionId <= 0)
            {
                return (false, "Section is required");
            }

            // Validate email format if provided
            if (!string.IsNullOrEmpty(model.Email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(model.Email);
                    if (addr.Address != model.Email)
                    {
                        return (false, "Invalid email format");
                    }
                }
                catch
                {
                    return (false, "Invalid email format");
                }
            }

            // Validate phone format (Pakistan: 11 digits starting with 03, with or without dashes)
            if (!string.IsNullOrEmpty(model.PhoneNo))
            {
                var phoneValidation = ValidatePhoneNumber(model.PhoneNo, "Phone");
                if (!phoneValidation.isValid)
                {
                    return (false, phoneValidation.errorMessage);
                }
            }

            if (!string.IsNullOrEmpty(model.MobileNo))
            {
                var mobileValidation = ValidatePhoneNumber(model.MobileNo, "Mobile");
                if (!mobileValidation.isValid)
                {
                    return (false, mobileValidation.errorMessage);
                }
            }

            // Validate parent phone numbers
            if (!string.IsNullOrEmpty(model.FatherPhone))
            {
                var fatherPhoneValidation = ValidatePhoneNumber(model.FatherPhone, "Father Phone");
                if (!fatherPhoneValidation.isValid)
                {
                    return (false, fatherPhoneValidation.errorMessage);
                }
            }

            if (!string.IsNullOrEmpty(model.MotherPhone))
            {
                var motherPhoneValidation = ValidatePhoneNumber(model.MotherPhone, "Mother Phone");
                if (!motherPhoneValidation.isValid)
                {
                    return (false, motherPhoneValidation.errorMessage);
                }
            }

            if (!string.IsNullOrEmpty(model.GuardianPhone))
            {
                var guardianPhoneValidation = ValidatePhoneNumber(model.GuardianPhone, "Guardian Phone");
                if (!guardianPhoneValidation.isValid)
                {
                    return (false, guardianPhoneValidation.errorMessage);
                }
            }

            // Validate CNIC or ParentId format based on tenant configuration
            var tenant = await _tenantService.GetCurrentTenantAsync();
            bool allowParentId = tenant?.AllowParentIdInCnicField ?? true;

            if (!string.IsNullOrEmpty(model.FatherCNIC))
            {
                var validationResult = ValidateCnicOrParentId(model.FatherCNIC, allowParentId, "Father");
                if (!validationResult.isValid)
                {
                    return (false, validationResult.errorMessage);
                }
            }

            if (!string.IsNullOrEmpty(model.MotherCNIC))
            {
                var validationResult = ValidateCnicOrParentId(model.MotherCNIC, allowParentId, "Mother");
                if (!validationResult.isValid)
                {
                    return (false, validationResult.errorMessage);
                }
            }

            return await Task.FromResult((true, string.Empty));
        }

        /// <summary>
        /// Validates phone number format - accepts both with and without dashes
        /// Formats: 03001234567 or 0301-8210558 (mobile), 021-12345678 (landline)
        /// </summary>
        /// <param name="phoneNumber">Phone number to validate</param>
        /// <param name="fieldLabel">Label for error messages</param>
        /// <returns>Validation result</returns>
        private (bool isValid, string errorMessage) ValidatePhoneNumber(string phoneNumber, string fieldLabel)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return (true, string.Empty);
            }

            phoneNumber = phoneNumber.Trim();
            var phoneDigits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Pattern 1: Without dashes - 11 digits starting with 03 (mobile)
            if (System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{11}$"))
            {
                if (phoneDigits.Length == 11 && phoneDigits.StartsWith("03"))
                {
                    return (true, string.Empty);
                }
            }

            // Pattern 2: With dash - 0301-8210558 format (mobile)
            if (System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{4}-\d{7}$"))
            {
                if (phoneDigits.Length == 11 && phoneDigits.StartsWith("03"))
                {
                    return (true, string.Empty);
                }
            }

            // Pattern 3: Landline format - 021-12345678
            if (System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{3}-\d{8}$"))
            {
                if (phoneDigits.Length == 11 && phoneDigits.StartsWith("0"))
                {
                    return (true, string.Empty);
                }
            }

            return (false, $"{fieldLabel} must be in valid format: 03001234567 or 0301-8210558 (11 digits starting with 03)");
        }

        /// <summary>
        /// Validates CNIC or ParentId based on tenant configuration
        /// </summary>
        /// <param name="value">The CNIC or ParentId value to validate</param>
        /// <param name="allowParentId">Whether ParentId is allowed (from tenant config)</param>
        /// <param name="fieldLabel">Label for error messages (e.g., "Father", "Mother")</param>
        /// <returns>Validation result with isValid flag and error message</returns>
        private (bool isValid, string errorMessage) ValidateCnicOrParentId(
            string value, 
            bool allowParentId, 
            string fieldLabel)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return (true, string.Empty); // Empty is valid, handled by required attribute
            }

            value = value.Trim();

            // Pattern 1: Check if it's a valid CNIC format (XXXXX-XXXXXXX-X = 13 digits with dashes)
            var cnicDigits = new string(value.Where(char.IsDigit).ToArray());
            bool isCnicFormat = System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{5}-\d{7}-\d{1}$");
            
            if (isCnicFormat && cnicDigits.Length == 13)
            {
                return (true, string.Empty); // Valid CNIC
            }

            // Pattern 2: Check if it's a numeric ParentId (e.g., "151", "14")
            bool isNumericOnly = System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d+$");
            
            if (isNumericOnly && allowParentId)
            {
                return (true, string.Empty); // Valid ParentId (tenant allows it)
            }

            // Invalid format
            if (allowParentId)
            {
                // Tenant allows both formats, but input matches neither
                return (false, $"{fieldLabel} CNIC/Parent ID must be either a valid CNIC (12345-1234567-1) or a numeric Parent ID (e.g., 151)");
            }
            else
            {
                // Tenant requires strict CNIC format only
                return (false, $"{fieldLabel} CNIC must be in format 12345-1234567-1 (13 digits)");
            }
        }

        #endregion

        #region Fee Package Replacement

        /// <summary>
        /// Remove a specific fee package from student (hard delete all unpaid assignments)
        /// </summary>
        private async Task<BaseModel> RemoveFeePackage(int studentId, string packageIdToRemove)
        {
            try
            {
                Console.WriteLine($"🗑️ Removing fee package {packageIdToRemove} from student {studentId}");

                // Get all assignments for this package
                var assignments = await _feeAssignmentRepository.GetAsync(
                    where: f => f.StudentId == studentId &&
                               f.IsInitialAssignment == true &&
                               f.InitialPackageId == packageIdToRemove &&
                               !f.IsDeleted,
                    includeProperties: i => i.Include(f => f.FeeTransactions)
                );

                var assignmentsList = assignments.ToList();

                if (!assignmentsList.Any())
                {
                    return new BaseModel { Success = true, Message = "No assignments found for this package" };
                }

                // Check if any have payments
                var hasPayments = assignmentsList.Any(a => a.FeeTransactions != null && a.FeeTransactions.Any());
                if (hasPayments)
                {
                    return new BaseModel 
                    { 
                        Success = false, 
                        Message = "Cannot remove package - one or more assignments have been paid" 
                    };
                }

                // Hard delete all assignments
                foreach (var assignment in assignmentsList)
                {
                    await _feeAssignmentRepository.DeleteAsync(assignment.Id);
                    Console.WriteLine($"🗑️ Deleted assignment ID {assignment.Id}");
                }

                await unitOfWork.SaveChangesAsync();
                Console.WriteLine($"✅ Removed package {packageIdToRemove} - deleted {assignmentsList.Count} assignments");

                return new BaseModel 
                { 
                    Success = true, 
                    Message = $"Package removed successfully - {assignmentsList.Count} assignments deleted" 
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error removing fee package: {ex.Message}");
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"Error removing package: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Get initial fee packages assigned to student at creation
        /// Groups by InitialPackageId and shows lock status based on payments
        /// </summary>
        public async Task<BaseModel> GetStudentInitialFeePackages(int studentId)
        {
            try
            {
                // ✅ Use repository pattern - Get all fee assignments for this student
                var allAssignments = await _feeAssignmentRepository.GetAsync(
                    where: f => f.StudentId == studentId && f.IsInitialAssignment == true && !f.IsDeleted,
                    includeProperties: i => i
                        .Include(f => f.FeeTransactions)
                        .Include(f => f.FeeGroupFeeType)
                            .ThenInclude(fgft => fgft.FeeGroup)
                        .Include(f => f.FeeGroupFeeType)
                            .ThenInclude(fgft => fgft.FeeType)
                );

                var initialAssignments = allAssignments.ToList();

                // Group by InitialPackageId
                var packageGroups = initialAssignments
                    .GroupBy(f => f.InitialPackageId)
                    .Select(g => new
                    {
                        InitialPackageId = g.Key,
                        FeeGroupFeeTypeId = g.First().FeeGroupFeeTypeId,
                        FeeGroupName = g.First().FeeGroupFeeType?.FeeGroup?.Name ?? "Unknown Package",
                        Assignments = mapper.Map<List<FeeAssignmentModel>>(g.ToList()),
                        HasPayments = g.Any(a => a.FeeTransactions != null && a.FeeTransactions.Any()),
                        IsEditable = !g.Any(a => a.FeeTransactions != null && a.FeeTransactions.Any()),
                        TotalAmount = g.Sum(a => a.Amount),
                        TotalPaid = g.Sum(a => a.PaidAmount),
                        TotalPending = g.Sum(a => a.Amount - a.PaidAmount)
                    })
                    .ToList();

                return new BaseModel
                {
                    Success = true,
                    Data = packageGroups,
                    Total = packageGroups.Count,
                    Message = "Initial fee packages loaded successfully"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading initial fee packages: {ex.Message}");
                return new BaseModel
                {
                    Success = false,
                    Message = $"Error loading fee packages: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Handle fee package replacement during student update
        /// Replaces specific package's unpaid initial assignments with new package
        /// </summary>
        private async Task<BaseModel> HandleFeePackageReplacement(int studentId, StudentCreateRequest model)
        {
            try
            {
                // Get the package ID to replace (from FeeGroupFeeTypeId)
                var packageIdToReplace = model.FeeGroupFeeTypeId.ToString();

                Console.WriteLine($"📝 Handling fee package replacement for student {studentId}, package {packageIdToReplace}");

                // ✅ STEP 1: Get all initial assignments for this specific package using repository
                var allAssignments = await _feeAssignmentRepository.GetAsync(
                    where: f => f.StudentId == studentId &&
                               f.IsInitialAssignment == true &&
                               f.InitialPackageId == packageIdToReplace &&
                               !f.IsDeleted,
                    includeProperties: i => i.Include(f => f.FeeTransactions)
                );

                var initialAssignments = allAssignments.ToList();

                if (!initialAssignments.Any())
                {
                    // No initial assignments for this package - create new ones
                    Console.WriteLine($"✅ No existing initial assignments for package {packageIdToReplace}, creating new ones");
                    await CreateFeeAssignmentsFromPackage(studentId, model);
                    return new BaseModel { Success = true, Message = "Fee assignments created successfully" };
                }

                Console.WriteLine($"📊 Found {initialAssignments.Count} initial assignments for package {packageIdToReplace}");

                // ✅ STEP 2: Check if ANY assignment in this package has payments
                var hasPayments = initialAssignments.Any(a => a.FeeTransactions != null && a.FeeTransactions.Any());

                if (hasPayments)
                {
                    // ✅ This package has payments - BLOCK replacement
                    var paidAssignments = initialAssignments.Where(a => a.FeeTransactions.Any()).ToList();
                    Console.WriteLine($"❌ Package {packageIdToReplace} has {paidAssignments.Count} paid assignments - BLOCKED");
                    
                    return new BaseModel 
                    { 
                        Success = false, 
                        Message = $"Cannot replace fee package - {paidAssignments.Count} assignment(s) have been paid. Package is locked." 
                    };
                }

                Console.WriteLine($"✅ All assignments in package {packageIdToReplace} are unpaid - proceeding with replacement");

                // ✅ STEP 3: HARD DELETE unpaid initial assignments for this package using repository
                foreach (var assignment in initialAssignments)
                {
                    await _feeAssignmentRepository.DeleteAsync(assignment.Id);
                    Console.WriteLine($"🗑️ Deleted assignment ID {assignment.Id} - {assignment.Description}");
                }

                await unitOfWork.SaveChangesAsync();
                Console.WriteLine($"✅ Deleted {initialAssignments.Count} unpaid initial assignments");

                // ✅ STEP 4: Create NEW fee assignments from the selected package
                await CreateFeeAssignmentsFromPackage(studentId, model);
                Console.WriteLine($"✅ Created new fee assignments from package {model.FeeGroupFeeTypeId}");

                return new BaseModel 
                { 
                    Success = true, 
                    Message = $"Fee package replaced successfully - {initialAssignments.Count} old assignments deleted, new assignments created" 
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in HandleFeePackageReplacement: {ex.Message}");
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"Error replacing fee package: {ex.Message}" 
                };
            }
        }

        #endregion
    }
}

