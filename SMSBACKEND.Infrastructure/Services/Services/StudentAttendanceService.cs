
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using Common.Utilities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static Common.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Common.Utilities.StaticClasses;

namespace Infrastructure.Services.Services
{

    public class StudentAttendanceService : BaseService<StudentAttendanceModel, StudentAttendance, int>, IStudentAttendanceService
    {
        private readonly IStudentAttendanceRepository _studentattendanceRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ICacheProvider _cacheProvider;
        private readonly ISessionService _sessionService;

        public StudentAttendanceService(
            IMapper mapper, 
            IStudentAttendanceRepository studentattendanceRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            IStudentRepository studentRepository,
            IClassRepository classRepository,
            ISectionRepository sectionRepository,
            ISessionService sessionService,
            ILogger<SectionService> logger,
            ICacheProvider cacheProvider = null
            ) : base(mapper, studentattendanceRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _studentattendanceRepository = studentattendanceRepository;
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _sectionRepository = sectionRepository;
            _cacheProvider = cacheProvider;
            _sessionService = sessionService;
        }

        /// <summary>
        /// Mark attendance for students
        /// ✅ FIXED: Uses UPSERT pattern instead of delete-insert to preserve audit trail
        /// </summary>
        public async Task<BaseModel> MarkAttendance(MarkAttendanceRequest request)
        {
            try
            {
                // Validate class and section exist
                var classInfo = await _classRepository.GetAsync(request.ClassId);
                var sectionInfo = await _sectionRepository.GetAsync(request.SectionId);

                if (classInfo == null || sectionInfo == null)
                {
                    return new BaseModel { Success = false, Message = "Class or Section not found" };
                }

                // Get existing attendance records for this date
                var existingRecords = await _studentattendanceRepository.GetAttendanceByClassSectionAsync(
                    request.ClassId, request.SectionId, request.AttendanceDate);

                var recordsToAdd = new List<StudentAttendance>();
                var recordsToUpdate = new List<StudentAttendance>();

                // UPSERT pattern: Update existing, insert new
                foreach (var record in request.Records)
                {
                    var existingRecord = existingRecords.FirstOrDefault(e => e.StudentId == record.StudentId);

                    if (existingRecord != null)
                    {
                        // Update existing record
                        existingRecord.AttendanceType = record.AttendanceType;
                        existingRecord.Remark = record.Notes;
                        existingRecord.UpdatedAt = DateTime.UtcNow;
                        existingRecord.UpdatedBy = "System"; // TODO: Get from current user context
                        recordsToUpdate.Add(existingRecord);
                    }
                    else
                    {
                        var activeSession = await _sessionService.GetActiveSessionId();
                        if (activeSession == 0)
                        {
                            throw new Exception("No active session found. Please set an active academic session.");
                        }
                        request.SessionId = activeSession;
                        // Create new record
                        var newRecord = new StudentAttendance
                        {
                            StudentId = record.StudentId,
                            SessionId = request.SessionId, // Default session if not provided
                            ClassId = request.ClassId,
                            SectionId = request.SectionId,
                            AttendanceDate = request.AttendanceDate,
                            AttendanceType = record.AttendanceType,
                            Remark = record.Notes,
                            IsBiometricAttendance = false,
                            BiometricDeviceData = null,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "System", // TODO: Get from current user context
                            UpdatedAt = DateTime.UtcNow,
                            UpdatedBy = "System"
                        };
                        recordsToAdd.Add(newRecord);
                    }
                }

                // Perform batch operations
                if (recordsToUpdate.Any())
                {
                    foreach (var record in recordsToUpdate)
                    {
                        await _studentattendanceRepository.UpdateAsync(record);
                    }
                }

                if (recordsToAdd.Any())
                {
                    await _studentattendanceRepository.AddAsync(recordsToAdd);
                }

                await SaveChanges();
                if (_cacheProvider != null)
                {
                    await _cacheProvider.RemoveByTagAsync("StudentAttendance");
                    
                    // Remove cache for dependent entities
                    if (EntityDependencyMap.Dependencies.TryGetValue("StudentAttendance", out var dependents))
                    {
                        foreach (var dependent in dependents)
                            await _cacheProvider.RemoveByTagAsync(dependent);
                    }
                }
                return new BaseModel 
                { 
                    Success = true, 
                    Message = $"Attendance marked successfully. Updated: {recordsToUpdate.Count}, Created: {recordsToAdd.Count}", 
                    Data = true 
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"An error occurred while marking attendance: {ex.Message}" 
                };
            }
        }

        public async Task<BaseModel> GetAttendanceStatistics(GetAttendanceStatisticsRequest request)
        {
            try
            {
                // Get class and section information
                var classInfo = await _classRepository.GetAsync(request.ClassId);
                var sectionInfo = await _sectionRepository.GetAsync(request.SectionId);

                if (classInfo == null || sectionInfo == null)
                {
                    return new BaseModel { Success = false, Message = "Class or Section not found" };
                }

                // Get attendance records within the date range
                var startDate = request.StartDate ?? DateTime.UtcNow.AddDays(-30);
                var endDate = request.EndDate ?? DateTime.UtcNow;

                var attendanceRecords = await _studentattendanceRepository.GetAttendanceStatisticsAsync(
                    request.ClassId, request.SectionId, startDate, endDate);

                var allStudents = await _studentRepository.GetAsync(
                    s => s.IsActive && !s.IsDeleted,
                    includeProperties: query => query
                        .Include(s => s.StudentInfo)
                        .Include(s => s.ClassAssignments)
                );
                
                // Filter students by class and section assignment
                var activeStudents = allStudents.Where(s => s.ClassAssignments
                    .Any(sca => sca.ClassId == request.ClassId && sca.SectionId == request.SectionId && sca.IsActive && !sca.IsDeleted))
                    .ToList();

                var statistics = new AttendanceStatisticsResponse
                {
                    ClassId = request.ClassId,
                    ClassName = classInfo.Name,
                    SectionId = request.SectionId,
                    SectionName = sectionInfo.Name,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalStudents = activeStudents.Count,
                    TotalRecords = attendanceRecords.Count,
                    PresentCount = attendanceRecords.Count(a => a.AttendanceType == Enums.AttendanceType.Present),
                    AbsentCount = attendanceRecords.Count(a => a.AttendanceType == Enums.AttendanceType.Absent),
                    LateCount = attendanceRecords.Count(a => a.AttendanceType == Enums.AttendanceType.Late),
                    HalfDayCount = attendanceRecords.Count(a => a.AttendanceType == Enums.AttendanceType.HalfDay),
                    DailyAttendance = new List<DailyAttendanceResponse>()
                };

                // Calculate percentages
                if (statistics.TotalRecords > 0)
                {
                    statistics.PresentPercentage = (double)statistics.PresentCount / statistics.TotalRecords * 100;
                    statistics.AbsentPercentage = (double)statistics.AbsentCount / statistics.TotalRecords * 100;
                    statistics.LatePercentage = (double)statistics.LateCount / statistics.TotalRecords * 100;
                    statistics.HalfDayPercentage = (double)statistics.HalfDayCount / statistics.TotalRecords * 100;
                }

                // Group by date for daily statistics
                var dailyGroups = attendanceRecords.GroupBy(a => a.AttendanceDate.Date);
                foreach (var group in dailyGroups)
                {
                    var dailyStats = new DailyAttendanceResponse
                    {
                        Date = group.Key,
                        TotalStudents = activeStudents.Count,
                        PresentCount = group.Count(a => a.AttendanceType == Enums.AttendanceType.Present),
                        AbsentCount = group.Count(a => a.AttendanceType == Enums.AttendanceType.Absent),
                        LateCount = group.Count(a => a.AttendanceType == Enums.AttendanceType.Late),
                        HalfDayCount = group.Count(a => a.AttendanceType == Enums.AttendanceType.HalfDay)
                    };

                    if (dailyStats.TotalStudents > 0)
                    {
                        dailyStats.PresentPercentage = (double)dailyStats.PresentCount / dailyStats.TotalStudents * 100;
                        dailyStats.AbsentPercentage = (double)dailyStats.AbsentCount / dailyStats.TotalStudents * 100;
                    }

                    statistics.DailyAttendance.Add(dailyStats);
                }

                return new BaseModel 
                { 
                    Success = true, 
                    Message = "Attendance statistics retrieved successfully", 
                    Data = statistics,
                    Total = statistics.TotalRecords
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"An error occurred while retrieving attendance statistics: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Get all attendance records for frontend caching
        /// Returns complete dataset for offline-first functionality
        /// </summary>
        public async Task<BaseModel> GetAllAttendance()
        {
            try
            {
                // Get all active attendance records
                var attendanceRecords = await _studentattendanceRepository.GetAsync(
                    a => a.IsActive && !a.IsDeleted);
                
                var attendanceModels = mapper.Map<List<StudentAttendanceModel>>(attendanceRecords);

                return new BaseModel 
                { 
                    Success = true, 
                    Message = $"Retrieved {attendanceModels.Count} attendance records successfully", 
                    Data = attendanceModels,
                    Total = attendanceModels.Count,
                    LastId = attendanceModels.LastOrDefault()?.Id ?? 0
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"An error occurred while retrieving attendance records: {ex.Message}" 
                };
            }
        }

        // ========================================
        // NEW METHODS FOR PHASE 1
        // ========================================

        public async Task<BaseModel> GetAttendanceByClassSection(GetAttendanceByClassSectionRequest request)
        {
            try
            {
                // Call repository to get data
                var attendanceRecords = await _studentattendanceRepository.GetAttendanceByClassSectionAsync(
                    request.ClassId, request.SectionId, request.AttendanceDate);

                // Map to DTOs
                var attendanceModels = mapper.Map<List<StudentAttendanceModel>>(attendanceRecords);

                return new BaseModel 
                { 
                    Success = true, 
                    Message = $"Retrieved {attendanceModels.Count} attendance records", 
                    Data = attendanceModels,
                    Total = attendanceModels.Count
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"An error occurred while retrieving attendance records: {ex.Message}" 
                };
            }
        }

        public async Task<BaseModel> GetAttendanceByStudent(GetAttendanceByStudentRequest request)
        {
            try
            {
                // Validate student exists
                var student = await _studentRepository.GetAsync(request.StudentId);
                if (student == null)
                {
                    return new BaseModel { Success = false, Message = "Student not found" };
                }

                // Call repository to get data
                var attendanceRecords = await _studentattendanceRepository.GetAttendanceByStudentAsync(
                    request.StudentId, request.StartDate, request.EndDate);

                // Map to DTOs
                var attendanceModels = mapper.Map<List<StudentAttendanceModel>>(attendanceRecords);

                return new BaseModel 
                { 
                    Success = true, 
                    Message = $"Retrieved {attendanceModels.Count} attendance records for student", 
                    Data = attendanceModels,
                    Total = attendanceModels.Count
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"An error occurred while retrieving student attendance: {ex.Message}" 
                };
            }
        }

        public async Task<BaseModel> GetAttendanceByDate(GetAttendanceByDateRequest request)
        {
            try
            {
                // Call repository to get data
                var attendanceRecords = await _studentattendanceRepository.GetAttendanceByDateAsync(
                    request.AttendanceDate);

                // Map to DTOs
                var attendanceModels = mapper.Map<List<StudentAttendanceModel>>(attendanceRecords);

                return new BaseModel 
                { 
                    Success = true, 
                    Message = $"Retrieved {attendanceModels.Count} attendance records for date {request.AttendanceDate:yyyy-MM-dd}", 
                    Data = attendanceModels,
                    Total = attendanceModels.Count
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"An error occurred while retrieving attendance records: {ex.Message}" 
                };
            }
        }

        public async Task<BaseModel> UpdateAttendance(int id, UpdateAttendanceRequest request)
        {
            try
            {
                // Validate: Get existing attendance
                var attendance = await _studentattendanceRepository.GetAttendanceByIdAsync(id);
                if (attendance == null)
                {
                    return new BaseModel { Success = false, Message = "Attendance record not found" };
                }

                // Validate: Cannot edit attendance older than 7 days
                var validation = ValidateAttendanceDate(attendance.AttendanceDate);
                if (!validation.isValid)
                {
                    return new BaseModel { Success = false, Message = validation.errorMessage };
                }

                // Update fields
                attendance.AttendanceType = request.AttendanceType;
                attendance.Remark = request.Notes;
                attendance.IsActive = request.IsActive;
                attendance.UpdatedBy = "System"; // TODO: Get from current user context

                // Call repository to update
                await _studentattendanceRepository.UpdateAttendanceAsync(attendance);

                // Map to DTO
                var attendanceModel = mapper.Map<StudentAttendanceModel>(attendance);

                return new BaseModel 
                { 
                    Success = true, 
                    Message = "Attendance updated successfully", 
                    Data = attendanceModel 
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"An error occurred while updating attendance: {ex.Message}" 
                };
            }
        }

        public async Task<BaseModel> DeleteAttendance(int id)
        {
            try
            {
                // Validate: Get existing attendance
                var attendance = await _studentattendanceRepository.GetAttendanceByIdAsync(id);
                if (attendance == null)
                {
                    return new BaseModel { Success = false, Message = "Attendance record not found" };
                }

                // Validate: Cannot delete attendance older than 7 days
                var validation = ValidateAttendanceDate(attendance.AttendanceDate);
                if (!validation.isValid)
                {
                    return new BaseModel { Success = false, Message = validation.errorMessage };
                }

                // Call repository to soft delete
                await _studentattendanceRepository.DeleteAttendanceAsync(id);

                return new BaseModel 
                { 
                    Success = true, 
                    Message = "Attendance deleted successfully" 
                };
            }
            catch (Exception ex)
            {
                return new BaseModel 
                { 
                    Success = false, 
                    Message = $"An error occurred while deleting attendance: {ex.Message}" 
                };
            }
        }

        // ========================================
        // PRIVATE VALIDATION METHODS
        // ========================================

        /// <summary>
        /// Validate attendance date (cannot be future, can only edit/delete within 7 days)
        /// </summary>
        private (bool isValid, string errorMessage) ValidateAttendanceDate(DateTime attendanceDate)
        {
            // Cannot mark attendance for future dates
            if (attendanceDate.Date > DateTime.Now.Date)
            {
                return (false, "Cannot mark attendance for future dates");
            }

            // Cannot edit/delete attendance older than 7 days (configurable)
            var daysDiff = (DateTime.Now.Date - attendanceDate.Date).Days;
            if (daysDiff > 7)
            {
                return (false, "Cannot edit or delete attendance older than 7 days. Please contact administrator.");
            }

            return (true, string.Empty);
        }

        // ========================================
        // PHASE 2: REPORTING METHODS
        // ========================================

        public async Task<BaseModel> GetMonthlyReportAsync(GetMonthlyReportRequest request)
        {
            try
            {
                // Get working days for the month
                var workingDays = await _studentattendanceRepository.GetWorkingDaysAsync(
                    request.Month, request.Year, request.ClassId, request.SectionId);

                // Get attendance records for the month
                var startDate = new DateTime(request.Year, request.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var activeSession = await _sessionService.GetActiveSessionId();
                if (activeSession == 0)
                {
                    throw new Exception("No active session found. Please set an active academic session.");
                }
                request.SessionId = activeSession;
                var attendanceRecords = await _studentattendanceRepository.GetAttendanceByDateRangeAsync(
                    startDate, endDate, request.ClassId, request.SectionId, request.SessionId);

                if (!attendanceRecords.Any())
                {
                    return new BaseModel
                    {
                        Success = true,
                        Data = new MonthlyReportModel
                        {
                            Month = request.Month,
                            Year = request.Year,
                            TotalWorkingDays = workingDays,
                            TotalStudents = 0,
                            ClassSummaries = new List<ClassSectionSummary>(),
                            StudentSummaries = new List<StudentAttendanceSummary>()
                        },
                        Message = "No attendance records found for the specified period"
                    };
                }

                // Group by class/section for class summaries
                var classSummaries = attendanceRecords
                    .GroupBy(a => new { a.ClassId, a.SectionId })
                    .Select(g => new ClassSectionSummary
                    {
                        ClassId = g.Key.ClassId,
                        SectionId = g.Key.SectionId,
                        ClassName = g.First().Class?.Name ?? "",
                        SectionName = g.First().Section?.Name ?? "",
                        TotalStudents = g.Select(x => x.StudentId).Distinct().Count(),
                        TotalPresent = g.Count(x => x.AttendanceType == AttendanceType.Present),
                        TotalAbsent = g.Count(x => x.AttendanceType == AttendanceType.Absent),
                        TotalLate = g.Count(x => x.AttendanceType == AttendanceType.Late),
                        TotalExcused = g.Count(x => x.AttendanceType == AttendanceType.Excused),
                        AverageAttendancePercentage = CalculateAveragePercentage(g.ToList(), workingDays)
                    })
                    .ToList();

                // Group by student for student summaries
                var studentSummaries = attendanceRecords
                    .GroupBy(a => a.StudentId)
                    .Select(g => new StudentAttendanceSummary
                    {
                        StudentId = g.Key,
                        StudentName = g.First().Student?.StudentInfo != null
                            ? $"{g.First().Student.StudentInfo.FirstName} {g.First().Student.StudentInfo.LastName}".Trim()
                            : "Unknown",
                        RollNo = g.First().Student?.StudentInfo?.RollNo ?? "",
                        AdmissionNo = g.First().Student?.StudentInfo?.AdmissionNo ?? "",
                        ClassName = g.First().Class?.Name ?? "",
                        SectionName = g.First().Section?.Name ?? "",
                        TotalDays = workingDays,
                        PresentDays = g.Count(x => x.AttendanceType == AttendanceType.Present),
                        AbsentDays = g.Count(x => x.AttendanceType == AttendanceType.Absent),
                        LateDays = g.Count(x => x.AttendanceType == AttendanceType.Late),
                        ExcusedDays = g.Count(x => x.AttendanceType == AttendanceType.Excused),
                        AttendancePercentage = CalculatePercentage(
                            g.Count(x => x.AttendanceType == AttendanceType.Present),
                            workingDays),
                        Status = DetermineStatus(
                            CalculatePercentage(g.Count(x => x.AttendanceType == AttendanceType.Present), workingDays))
                    })
                    .OrderByDescending(s => s.AttendancePercentage)
                    .ToList();

                var report = new MonthlyReportModel
                {
                    Month = request.Month,
                    Year = request.Year,
                    TotalWorkingDays = workingDays,
                    TotalStudents = studentSummaries.Count,
                    ClassSummaries = classSummaries,
                    StudentSummaries = studentSummaries
                };

                return new BaseModel
                {
                    Success = true,
                    Data = report,
                    Message = "Monthly report generated successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseModel
                {
                    Success = false,
                    Message = $"Error generating monthly report: {ex.Message}"
                };
            }
        }

        public async Task<BaseModel> GetAttendancePercentageAsync(GetAttendancePercentageRequest request)
        {
            try
            {
                // Validate date range
                if (request.EndDate < request.StartDate)
                {
                    return new BaseModel
                    {
                        Success = false,
                        Message = "End date cannot be before start date"
                    };
                }

                // Get attendance records for the student
                var attendanceRecords = await _studentattendanceRepository.GetAttendanceByStudentAsync(
                    request.StudentId, request.StartDate, request.EndDate);

                // Calculate working days
                var workingDays = CalculateWorkingDaysBetweenDates(request.StartDate, request.EndDate);

                // Calculate statistics
                var presentDays = attendanceRecords.Count(x => x.AttendanceType == AttendanceType.Present);
                var absentDays = attendanceRecords.Count(x => x.AttendanceType == AttendanceType.Absent);
                var lateDays = attendanceRecords.Count(x => x.AttendanceType == AttendanceType.Late);
                var excusedDays = attendanceRecords.Count(x => x.AttendanceType == AttendanceType.Excused);

                var percentage = CalculatePercentage(presentDays, workingDays);

                // Build daily records
                var dailyRecords = attendanceRecords
                    .Select(a => new DailyAttendanceRecord
                    {
                        Date = a.AttendanceDate,
                        AttendanceType = a.AttendanceType.ToString(),
                        Remark = a.Remark ?? ""
                    })
                    .OrderBy(d => d.Date)
                    .ToList();

                var result = new AttendancePercentageModel
                {
                    StudentId = request.StudentId,
                    StudentName = attendanceRecords.FirstOrDefault()?.Student?.StudentInfo != null
                        ? $"{attendanceRecords.First().Student.StudentInfo.FirstName} {attendanceRecords.First().Student.StudentInfo.LastName}".Trim()
                        : "Unknown",
                    RollNo = attendanceRecords.FirstOrDefault()?.Student?.StudentInfo?.RollNo ?? "",
                    AdmissionNo = attendanceRecords.FirstOrDefault()?.Student?.StudentInfo?.AdmissionNo ?? "",
                    TotalWorkingDays = workingDays,
                    PresentDays = presentDays,
                    AbsentDays = absentDays,
                    LateDays = lateDays,
                    ExcusedDays = excusedDays,
                    AttendancePercentage = percentage,
                    Status = DetermineStatus(percentage),
                    DailyRecords = dailyRecords
                };

                return new BaseModel
                {
                    Success = true,
                    Data = result,
                    Message = "Attendance percentage calculated successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseModel
                {
                    Success = false,
                    Message = $"Error calculating attendance percentage: {ex.Message}"
                };
            }
        }

        public async Task<BaseModel> GetDefaultersAsync(GetDefaultersRequest request)
        {
            try
            {
                // Validate threshold
                if (request.Threshold < 0 || request.Threshold > 100)
                {
                    return new BaseModel
                    {
                        Success = false,
                        Message = "Threshold must be between 0 and 100"
                    };
                }
                var activeSession = await _sessionService.GetActiveSessionId();
                if (activeSession == 0)
                {
                    throw new Exception("No active session found. Please set an active academic session.");
                }
                request.SessionId = activeSession;
                // Get attendance records for the date range
                var attendanceRecords = await _studentattendanceRepository.GetAttendanceByDateRangeAsync(
                    request.StartDate, request.EndDate, request.ClassId, request.SectionId, request.SessionId);

                // Calculate working days
                var workingDays = CalculateWorkingDaysBetweenDates(request.StartDate, request.EndDate);

                // Group by student and calculate percentages
                var studentAttendance = attendanceRecords
                    .GroupBy(a => a.StudentId)
                    .Select(g => new
                    {
                        StudentId = g.Key,
                        Student = g.First().Student,
                        Class = g.First().Class,
                        Section = g.First().Section,
                        PresentDays = g.Count(x => x.AttendanceType == AttendanceType.Present),
                        AbsentDays = g.Count(x => x.AttendanceType == AttendanceType.Absent),
                        LateDays = g.Count(x => x.AttendanceType == AttendanceType.Late),
                        TotalDays = workingDays,
                        Percentage = CalculatePercentage(g.Count(x => x.AttendanceType == AttendanceType.Present), workingDays)
                    })
                    .Where(s => s.Percentage < request.Threshold)  // Filter defaulters
                    .OrderBy(s => s.Percentage)
                    .ToList();

                // Build defaulters list
                var defaulters = studentAttendance
                    .Select(s => new DefaulterStudent
                    {
                        StudentId = s.StudentId,
                        StudentName = s.Student?.StudentInfo != null
                            ? $"{s.Student.StudentInfo.FirstName} {s.Student.StudentInfo.LastName}".Trim()
                            : "Unknown",
                        RollNo = s.Student?.StudentInfo?.RollNo ?? "",
                        AdmissionNo = s.Student?.StudentInfo?.AdmissionNo ?? "",
                        ClassName = s.Class?.Name ?? "",
                        SectionName = s.Section?.Name ?? "",
                        TotalDays = s.TotalDays,
                        PresentDays = s.PresentDays,
                        AbsentDays = s.AbsentDays,
                        LateDays = s.LateDays,
                        AttendancePercentage = s.Percentage,
                        FatherName = s.Student?.StudentParent?.FatherName ?? "",
                        FatherPhone = s.Student?.StudentParent?.FatherPhone ?? "",
                        MotherName = s.Student?.StudentParent?.MotherName ?? "",
                        MotherPhone = s.Student?.StudentParent?.MotherPhone ?? "",
                        GuardianName = s.Student?.StudentParent?.GuardianName ?? "",
                        GuardianPhone = s.Student?.StudentParent?.GuardianPhone ?? "",
                        GuardianEmail = s.Student?.StudentParent?.GuardianEmail ?? ""
                    })
                    .ToList();

                var reportModel = new DefaultersReportModel
                {
                    Threshold = request.Threshold,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalDefaulters = defaulters.Count,
                    TotalWorkingDays = workingDays,
                    Defaulters = defaulters
                };

                return new BaseModel
                {
                    Success = true,
                    Data = reportModel,
                    Message = $"Found {defaulters.Count} students below {request.Threshold}% attendance"
                };
            }
            catch (Exception ex)
            {
                return new BaseModel
                {
                    Success = false,
                    Message = $"Error generating defaulters report: {ex.Message}"
                };
            }
        }

        public async Task<BaseModel> GetAttendanceSummaryAsync(GetAttendanceSummaryRequest request)
        {
            try
            {
                var activeSession = await _sessionService.GetActiveSessionId();
                if (activeSession == 0)
                {
                    throw new Exception("No active session found. Please set an active academic session.");
                }
                request.SessionId = activeSession;
                // Get attendance records
                var attendanceRecords = await _studentattendanceRepository.GetAttendanceByDateRangeAsync(
                    request.StartDate, request.EndDate, request.ClassId, request.SectionId, request.SessionId);

                // Calculate working days
                var workingDays = CalculateWorkingDaysBetweenDates(request.StartDate, request.EndDate);

                // Build summary
                var summary = new
                {
                    TotalWorkingDays = workingDays,
                    TotalRecords = attendanceRecords.Count,
                    TotalStudents = attendanceRecords.Select(a => a.StudentId).Distinct().Count(),
                    TotalPresent = attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Present),
                    TotalAbsent = attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Absent),
                    TotalLate = attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Late),
                    TotalExcused = attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Excused),
                    AverageAttendancePercentage = attendanceRecords.Any()
                        ? CalculateAveragePercentage(attendanceRecords, workingDays)
                        : 0,
                    DateRange = new
                    {
                        Start = request.StartDate,
                        End = request.EndDate
                    }
                };

                return new BaseModel
                {
                    Success = true,
                    Data = summary,
                    Message = "Attendance summary generated successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseModel
                {
                    Success = false,
                    Message = $"Error generating attendance summary: {ex.Message}"
                };
            }
        }

        public async Task<BaseModel> ExportAttendanceAsync(ExportAttendanceRequest request)
        {
            try
            {
                var activeSession = await _sessionService.GetActiveSessionId();
                if (activeSession == 0)
                {
                    throw new Exception("No active session found. Please set an active academic session.");
                }
                request.SessionId = activeSession;
                // Get attendance records
                var attendanceRecords = await _studentattendanceRepository.GetAttendanceByDateRangeAsync(
                    request.StartDate, request.EndDate, request.ClassId, request.SectionId, request.SessionId);

                if (!attendanceRecords.Any())
                {
                    return new BaseModel
                    {
                        Success = false,
                        Message = "No attendance records found to export"
                    };
                }

                byte[] fileBytes;

                if (request.Format.Equals("Excel", StringComparison.OrdinalIgnoreCase))
                {
                    fileBytes = await GenerateExcelReport(attendanceRecords, request);
                }
                else if (request.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
                {
                    fileBytes = await GeneratePdfReport(attendanceRecords, request);
                }
                else
                {
                    return new BaseModel
                    {
                        Success = false,
                        Message = "Invalid export format. Use 'Excel' or 'PDF'"
                    };
                }

                return new BaseModel
                {
                    Success = true,
                    Data = fileBytes,
                    Message = $"Attendance exported successfully to {request.Format}"
                };
            }
            catch (Exception ex)
            {
                return new BaseModel
                {
                    Success = false,
                    Message = $"Error exporting attendance: {ex.Message}"
                };
            }
        }

        // ========================================
        // HELPER METHODS FOR REPORTING
        // ========================================

        private decimal CalculatePercentage(int present, int total)
        {
            if (total == 0) return 0;
            return Math.Round((decimal)present / total * 100, 2);
        }

        private decimal CalculateAveragePercentage(List<StudentAttendance> records, int workingDays)
        {
            var studentPercentages = records
                .GroupBy(r => r.StudentId)
                .Select(g => CalculatePercentage(
                    g.Count(x => x.AttendanceType == AttendanceType.Present),
                    workingDays))
                .ToList();

            return studentPercentages.Any() ? Math.Round(studentPercentages.Average(), 2) : 0;
        }

        private string DetermineStatus(decimal percentage)
        {
            if (percentage >= 90) return "Excellent";
            if (percentage >= 75) return "Good";
            if (percentage >= 60) return "Average";
            return "Poor";
        }

        private int CalculateWorkingDaysBetweenDates(DateTime startDate, DateTime endDate)
        {
            var workingDays = 0;
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            // TODO: Exclude holidays when Holiday entity is available
            return workingDays;
        }

        private async Task<byte[]> GenerateExcelReport(List<StudentAttendance> records, ExportAttendanceRequest request)
        {
            // TODO: Implement Excel generation using EPPlus or ClosedXML
            // For now, return empty byte array as placeholder
            return await Task.FromResult(new byte[0]);
        }

        private async Task<byte[]> GeneratePdfReport(List<StudentAttendance> records, ExportAttendanceRequest request)
        {
            // TODO: Implement PDF generation using iTextSharp or DinkToPdf
            // For now, return empty byte array as placeholder
            return await Task.FromResult(new byte[0]);
        }
    }
}


