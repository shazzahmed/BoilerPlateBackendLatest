using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using AutoMapper;
using Common.DTO.Response;
using Common.DTO.Request;
using Microsoft.EntityFrameworkCore;
using static Common.Utilities.Enums;
using System.Linq.Expressions;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Text;

namespace Infrastructure.Repository
{
    public class StudentRepository : BaseRepository<Student, int>, IStudentRepository
    {
        public StudentRepository(IMapper mapper, ISqlServerDbContext context) : base(mapper, context)
        {
        }

        /// <summary>
        /// Generates the next admission number using tenant-specific sequence and format
        /// Thread-safe: Increments sequence within transaction
        /// Auto-initializes sequence from existing data if LastStudentSequence is 0
        /// IMPORTANT: This increments the sequence. Use PeekNextAdmissionNumberAsync for preview only.
        /// </summary>
        public async Task<string> GenerateNextAdmissionNumberAsync(int tenantId)
        {
            // Get tenant with sequence information
            var tenant = await DbContext.Tenants.FirstOrDefaultAsync(t => t.TenantId == tenantId);
            if (tenant == null)
            {
                throw new InvalidOperationException($"Tenant with ID {tenantId} not found");
            }

            // Auto-initialize sequence if it's 0 (first use or existing data)
            if (tenant.LastStudentSequence == 0)
            {
                tenant.LastStudentSequence = await GetMaxAdmissionNumberSequenceAsync(tenantId);
            }

            // Increment sequence (this will be saved with the student creation transaction)
            tenant.LastStudentSequence++;
            var sequence = tenant.LastStudentSequence;

            // Generate admission number from template
            return GenerateFromTemplate(tenant.AdmissionNoFormat, sequence);
        }

        /// <summary>
        /// Peeks at the next admission number WITHOUT incrementing the sequence
        /// Used for preview/display purposes (e.g., showing next admission number on form)
        /// </summary>
        public async Task<string> PeekNextAdmissionNumberAsync(int tenantId)
        {
            // Get tenant with sequence information
            var tenant = await DbContext.Tenants.AsNoTracking()
                .FirstOrDefaultAsync(t => t.TenantId == tenantId);
            
            if (tenant == null)
            {
                throw new InvalidOperationException($"Tenant with ID {tenantId} not found");
            }

            // Get current sequence (or initialize if 0)
            int currentSequence = tenant.LastStudentSequence;
            if (currentSequence == 0)
            {
                currentSequence = await GetMaxAdmissionNumberSequenceAsync(tenantId);
            }

            // Peek at NEXT sequence (without saving)
            var nextSequence = currentSequence + 1;

            // Generate admission number from template
            return GenerateFromTemplate(tenant.AdmissionNoFormat, nextSequence);
        }

        /// <summary>
        /// Gets the maximum admission number from existing students for this tenant
        /// Extracts numeric value from AdmissionNo (handles various formats like "501542", "ADM2025001", etc.)
        /// </summary>
        private async Task<int> GetMaxAdmissionNumberSequenceAsync(int tenantId)
        {
            var students = await DbContext.Students
                .Include(s => s.StudentInfo)
                .Where(s => s.TenantId == tenantId && !s.IsDeleted && s.StudentInfo != null)
                .Select(s => s.StudentInfo.AdmissionNo)
                .ToListAsync();

            if (!students.Any())
            {
                return 0; // No existing students
            }

            int maxSequence = 0;

            foreach (var admissionNo in students)
            {
                if (string.IsNullOrEmpty(admissionNo))
                    continue;

                // Extract all digits from admission number
                var digitsOnly = new string(admissionNo.Where(char.IsDigit).ToArray());

                if (int.TryParse(digitsOnly, out int sequence))
                {
                    maxSequence = Math.Max(maxSequence, sequence);
                }
            }

            return maxSequence;
        }

        /// <summary>
        /// Generates admission number from template with placeholders
        /// Supported placeholders:
        /// - {SEQ} = sequence number (e.g., "1", "2", "501")
        /// - {SEQ:000} = padded sequence (e.g., "001", "002", "501")
        /// - {YEAR} = current year (e.g., "2025")
        /// </summary>
        private string GenerateFromTemplate(string template, int sequence)
        {
            if (string.IsNullOrEmpty(template))
            {
                template = "{SEQ}"; // Default fallback
            }

            var result = new StringBuilder(template);

            // Replace {SEQ:000} with padded sequence (must be done before {SEQ})
            var paddedMatch = Regex.Match(template, @"\{SEQ:([0]+)\}");
            if (paddedMatch.Success)
            {
                var padding = paddedMatch.Groups[1].Value.Length;
                var paddedSeq = sequence.ToString().PadLeft(padding, '0');
                result.Replace(paddedMatch.Value, paddedSeq);
            }
            else
            {
                // Replace {SEQ} with plain sequence
                result.Replace("{SEQ}", sequence.ToString());
            }

            // Replace {YEAR} with current year
            result.Replace("{YEAR}", DateTime.UtcNow.Year.ToString());

            return result.ToString();
        }

        public async Task<int> CreateStudent(StudentCreateRequest model)
        {
            var strategy = DbContext.Database.CreateExecutionStrategy();
            
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await DbContext.Database.BeginTransactionAsync();
                try
                {
                // Generate admission number if not provided
                if (string.IsNullOrEmpty(model.AdmissionNo))
                {
                    model.AdmissionNo = await GenerateNextAdmissionNumberAsync(model.TenantId);
                }

                // Create Student entity
                var student = new Student
                {
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };
                
                DbContext.Students.Add(student);
                await DbContext.SaveChangesAsync();
                
                // Create StudentInfo entity (uses Student.Id as primary key)
                var studentInfo = new StudentInfo
                {
                    Id = student.Id, // Use Student.Id as primary key for one-to-one relationship
                    AdmissionNo = model.AdmissionNo,
                    Image = model.Image,
                    RollNo = model.RollNo,
                    AdmissionDate = model.AdmissionDate,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Dob = model.DateOfBirth,
                    Gender = model.Gender,
                    BFormNumber = model.BFormNumber,
                    BloodGroup = model.BloodGroup,
                    Cast = model.Cast,
                    Religion = model.Religion,
                    PhoneNo = model.PhoneNo,
                    MobileNo = model.MobileNo,
                    Email = model.Email,
                    Height = model.Height,
                    Weight = model.Weight,
                    MeasurementDate = model.MeasurementDate,
                    ParentId = model.ParentId,
                    SchoolHouseId = model.SchoolHouseId,
                    CategoryId = model.CategoryId,
                    StudentUserId = model.StudentUserId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };
                
                DbContext.StudentInfo.Add(studentInfo);
                
                // Create StudentAddress entity
                var studentAddress = new StudentAddress
                {
                    StudentId = student.Id,
                    CurrentAddress = model.CurrentAddress,
                    AreaAddress = model.AreaAddress,
                    PermanentAddress = model.PermanentAddress,
                    City = model.City,
                    State = model.State,
                    Zipcode = model.ZipCode,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };
                
                DbContext.StudentAddress.Add(studentAddress);
                
                // Create StudentFinancial entity
                var studentFinancial = new StudentFinancial
                {
                    StudentId = student.Id,
                    MonthlyFees = model.MonthlyFees ?? 0,
                    AdmissionFees = (float?)model.AdmissionFees,
                    SecurityDeposit = (float?)model.SecurityDeposit,
                    Arrears = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };
                
                DbContext.StudentFinancial.Add(studentFinancial);
                
                // Create StudentParent entity
                var studentParent = new StudentParent
                {
                    StudentId = student.Id,
                    FatherName = model.FatherName,
                    FatherPhone = model.FatherPhone,
                    FatherOccupation = model.FatherOccupation,
                    FatherCNIC = model.FatherCNIC,
                    MotherName = model.MotherName,
                    MotherPhone = model.MotherPhone,
                    MotherOccupation = model.MotherOccupation,
                    MotherCNIC = model.MotherCNIC,
                    GuardianIs = model.GuardianType,
                    GuardianName = model.GuardianName,
                    GuardianPhone = model.GuardianPhone,
                    GuardianRelation = model.GuardianRelation,
                    GuardianEmail = model.GuardianEmail ?? "",
                    GuardianOccupation = model.GuardianOccupation,
                    GuardianAddress = model.GuardianAddress,
                    FatherPic = model.FatherPic,
                    MotherPic = model.MotherPic,
                    GuardianPic = model.GuardianPic,
                    GuardianAreaAddress = model.GuardianAreaAddress ?? "",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };
                
                DbContext.StudentParent.Add(studentParent);
                
                // Create StudentMiscellaneous entity
                var studentMiscellaneous = new StudentMiscellaneous
                {
                    StudentId = student.Id,
                    MedicalHistory = model.MedicalHistory,
                    Note = model.MedicalNotes,
                    PickupPerson = model.PickupPerson,
                    PreviousSchool = model.PreviousSchool,
                    DisNote = null,
                    DisReason = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };
                
                DbContext.StudentMiscellaneous.Add(studentMiscellaneous);
                
                await DbContext.SaveChangesAsync();
                // Create StudentClassAssignment entity
                var studentClassAssignment = new StudentClassAssignment
                {
                    StudentId = student.Id,
                    ClassId = model.ClassId,
                    SectionId = model.SectionId,
                    SessionId = model.SessionId,
                    AssignedDate = DateTime.Now,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = model.CreatedBy ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };
                
                DbContext.StudentClasses.Add(studentClassAssignment);
                await DbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return student.Id;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<StudentModel> UpdateStudent(StudentCreateRequest model)
        {
            // Use execution strategy to handle retries with manual transaction
            var strategy = DbContext.Database.CreateExecutionStrategy();
            
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await DbContext.Database.BeginTransactionAsync();
                try
                {
                    // Find existing student by ID
                    var existingStudent = await DbContext.Students
                        .Include(s => s.StudentInfo)
                        .Include(s => s.StudentAddress)
                        .Include(s => s.StudentFinancial)
                        .Include(s => s.StudentParent)
                        .Include(s => s.StudentMiscellaneous)
                        .Include(s => s.ClassAssignments)
                        .FirstOrDefaultAsync(s => s.Id == model.Id && !s.IsDeleted);
                    
                    if (existingStudent == null)
                    {
                        throw new InvalidOperationException("Student not found");
                    }

                    // Update student basic info
                    existingStudent.UpdatedAt = DateTime.UtcNow;
                    existingStudent.UpdatedBy = model.UpdatedBy ?? "System";
                    existingStudent.SyncStatus = "updated";

                    // Update StudentInfo
                    if (existingStudent.StudentInfo != null)
                    {
                        existingStudent.StudentInfo.AdmissionNo = model.AdmissionNo;
                        existingStudent.StudentInfo.Image = model.Image;
                        existingStudent.StudentInfo.RollNo = model.RollNo;
                        existingStudent.StudentInfo.AdmissionDate = model.AdmissionDate;
                        existingStudent.StudentInfo.FirstName = model.FirstName;
                        existingStudent.StudentInfo.LastName = model.LastName;
                        existingStudent.StudentInfo.Dob = model.DateOfBirth;
                        existingStudent.StudentInfo.Gender = model.Gender;
                        existingStudent.StudentInfo.BFormNumber = model.BFormNumber;
                        existingStudent.StudentInfo.Cast = model.Cast;
                        existingStudent.StudentInfo.Religion = model.Religion;
                        existingStudent.StudentInfo.PhoneNo = model.PhoneNo;
                        existingStudent.StudentInfo.MobileNo = model.MobileNo;
                        existingStudent.StudentInfo.Email = model.Email;
                        existingStudent.StudentInfo.BloodGroup = model.BloodGroup;
                        existingStudent.StudentInfo.Height = model.Height;
                        existingStudent.StudentInfo.Weight = model.Weight;
                        existingStudent.StudentInfo.MeasurementDate = model.MeasurementDate;
                        existingStudent.StudentInfo.SchoolHouseId = model.SchoolHouseId;
                        existingStudent.StudentInfo.CategoryId = model.CategoryId;
                        existingStudent.StudentInfo.ParentId = model.ParentId; // Updated parent ID from service
                        existingStudent.StudentInfo.UpdatedAt = DateTime.UtcNow;
                        existingStudent.StudentInfo.UpdatedBy = model.UpdatedBy ?? "System";
                    }

                    // Update StudentAddress
                    if (existingStudent.StudentAddress != null)
                    {
                        existingStudent.StudentAddress.CurrentAddress = model.CurrentAddress;
                        existingStudent.StudentAddress.AreaAddress = model.AreaAddress;
                        existingStudent.StudentAddress.PermanentAddress = model.PermanentAddress;
                        existingStudent.StudentAddress.City = model.City;
                        existingStudent.StudentAddress.State = model.State;
                        existingStudent.StudentAddress.Zipcode = model.ZipCode;
                        existingStudent.StudentAddress.UpdatedAt = DateTime.UtcNow;
                        existingStudent.StudentAddress.UpdatedBy = model.UpdatedBy ?? "System";
                    }

                    // Update StudentFinancial
                    if (existingStudent.StudentFinancial != null)
                    {
                        existingStudent.StudentFinancial.MonthlyFees = model.MonthlyFees ?? 0;
                        existingStudent.StudentFinancial.AdmissionFees = (float?)model.AdmissionFees;
                        existingStudent.StudentFinancial.SecurityDeposit = (float?)model.SecurityDeposit;
                        existingStudent.StudentFinancial.UpdatedAt = DateTime.UtcNow;
                        existingStudent.StudentFinancial.UpdatedBy = model.UpdatedBy ?? "System";
                    }

                    // Update StudentParent
                    if (existingStudent.StudentParent != null)
                    {
                        existingStudent.StudentParent.FatherName = model.FatherName;
                        existingStudent.StudentParent.FatherPhone = model.FatherPhone;
                        existingStudent.StudentParent.FatherOccupation = model.FatherOccupation;
                        existingStudent.StudentParent.FatherCNIC = model.FatherCNIC;
                        existingStudent.StudentParent.FatherPic = model.FatherPic;
                        existingStudent.StudentParent.MotherName = model.MotherName;
                        existingStudent.StudentParent.MotherPhone = model.MotherPhone;
                        existingStudent.StudentParent.MotherOccupation = model.MotherOccupation;
                        existingStudent.StudentParent.MotherCNIC = model.MotherCNIC;
                        existingStudent.StudentParent.MotherPic = model.MotherPic;
                        existingStudent.StudentParent.GuardianIs = model.GuardianType;
                        existingStudent.StudentParent.GuardianName = model.GuardianName;
                        existingStudent.StudentParent.GuardianPhone = model.GuardianPhone;
                        existingStudent.StudentParent.GuardianRelation = model.GuardianRelation;
                        existingStudent.StudentParent.GuardianEmail = model.GuardianEmail ?? "";
                        existingStudent.StudentParent.GuardianOccupation = model.GuardianOccupation;
                        existingStudent.StudentParent.GuardianAddress = model.GuardianAddress;
                        existingStudent.StudentParent.GuardianAreaAddress = model.GuardianAreaAddress ?? "";
                        existingStudent.StudentParent.GuardianPic = model.GuardianPic;
                        existingStudent.StudentParent.UpdatedAt = DateTime.UtcNow;
                        existingStudent.StudentParent.UpdatedBy = model.UpdatedBy ?? "System";
                    }

                    // Update StudentMiscellaneous
                    if (existingStudent.StudentMiscellaneous != null)
                    {
                        existingStudent.StudentMiscellaneous.MedicalHistory = model.MedicalHistory;
                        existingStudent.StudentMiscellaneous.Note = model.MedicalNotes;
                        existingStudent.StudentMiscellaneous.PickupPerson = model.PickupPerson;
                        existingStudent.StudentMiscellaneous.PreviousSchool = model.PreviousSchool;
                        existingStudent.StudentMiscellaneous.UpdatedAt = DateTime.UtcNow;
                        existingStudent.StudentMiscellaneous.UpdatedBy = model.UpdatedBy ?? "System";
                    }

                    // Update StudentClassAssignment
                    // NOTE: ClassId, SectionId, SessionId are part of composite key and cannot be modified
                    // Solution: Delete old assignment and create new one if class/section changes
                    var currentAssignment = existingStudent.ClassAssignments?.FirstOrDefault();
                    
                    // Check if class/section/session has changed
                    bool hasClassChanged = currentAssignment != null && 
                        (currentAssignment.ClassId != model.ClassId || 
                         currentAssignment.SectionId != model.SectionId ||
                         currentAssignment.SessionId != model.SessionId);
                    
                    if (hasClassChanged)
                    {
                        // Soft delete old assignment (keep history)
                        currentAssignment.IsDeleted = true;
                        currentAssignment.DeletedAt = DateTime.UtcNow;
                        currentAssignment.DeletedBy = model.UpdatedBy ?? "System";
                        
                        // Create new assignment with new class/section
                        var newAssignment = new StudentClassAssignment
                        {
                            StudentId = existingStudent.Id,
                            ClassId = model.ClassId,
                            SectionId = model.SectionId,
                            SessionId = model.SessionId,
                            AssignedDate = DateTime.UtcNow,
                            TenantId = existingStudent.TenantId,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = model.UpdatedBy ?? "System",
                            UpdatedAt = DateTime.UtcNow,
                            UpdatedBy = model.UpdatedBy ?? "System",
                            IsDeleted = false
                        };
                        DbContext.StudentClasses.Add(newAssignment);
                    }
                    else if (currentAssignment != null)
                    {
                        // No key change, just update timestamps
                        currentAssignment.UpdatedAt = DateTime.UtcNow;
                        currentAssignment.UpdatedBy = model.UpdatedBy ?? "System";
                    }
                    else if (model.ClassId > 0 && model.SectionId > 0)
                    {
                        // Create new assignment if none exists
                        var newAssignment = new StudentClassAssignment
                        {
                            StudentId = existingStudent.Id,
                            ClassId = model.ClassId,
                            SectionId = model.SectionId,
                            SessionId = model.SessionId,
                            AssignedDate = DateTime.UtcNow,
                            TenantId = existingStudent.TenantId,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = model.UpdatedBy ?? "System",
                            UpdatedAt = DateTime.UtcNow,
                            UpdatedBy = model.UpdatedBy ?? "System",
                            IsDeleted = false
                        };
                        DbContext.StudentClasses.Add(newAssignment);
                    }

                    await DbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Return updated student model
                    return mapper.Map<StudentModel>(existingStudent);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
        public async Task<bool> SoftDeleteStudent(int id, string deletedBy)
        {
            // TODO: Implement soft delete
            // For now, return a placeholder
            return await Task.FromResult(true);
        }

        public async Task<bool> RestoreStudent(int id, string restoredBy)
        {
            // TODO: Implement restore
            // For now, return a placeholder
            return await Task.FromResult(true);
        }
        public async Task<bool> IsAdmissionNoUnique(string admissionNo, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(admissionNo))
                return true;

            var query = DbContext.StudentInfo
                .Where(si => si.AdmissionNo == admissionNo && !si.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(si => si.Student.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> IsRollNoUniqueInClass(string rollNo, int classId, int sectionId, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(rollNo))
                return true;

            var query = DbContext.StudentInfo
                .Where(si => si.RollNo == rollNo && !si.IsDeleted)
                .Join(DbContext.StudentClasses,
                    si => si.Student.Id,
                    sca => sca.Student.Id,
                    (si, sca) => new { si, sca })
                .Where(x => x.sca.ClassId == classId && x.sca.SectionId == sectionId && !x.sca.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.si.Student.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<(List<StudentModel> Students, int Count, int LastId, string lastAdmNo)> GetAllStudent(FilterParams filters)
        {
            try
            {
                var query = DbContext.Students
                    .Include(s => s.StudentInfo)
                    .Include(s => s.StudentAddress)
                    .Include(s => s.StudentFinancial)
                    .Include(s => s.StudentParent)
                    .Include(s => s.StudentMiscellaneous)
                    .Include(s => s.ClassAssignments)
                        .ThenInclude(ca => ca.Class)
                    .Include(s => s.ClassAssignments)
                        .ThenInclude(ca => ca.Section)
                    .Include(s => s.ClassAssignments)
                        .ThenInclude(ca => ca.Session)
                    .Where(s => !s.IsDeleted);


                var totalCount = await query.CountAsync();
                var students = await query
                    .OrderByDescending(s => s.Id)
                    .ToListAsync();

                var studentModels = mapper.Map<List<StudentModel>>(students);
                
                // ✅ Enrich each student with additional data
                foreach (var studentModel in studentModels)
                {
                    var studentId = studentModel.Id;
                    
                    // Get user account info (student)
                    if (!string.IsNullOrEmpty(studentModel.StudentUserId))
                    {
                        var studentUser = await DbContext.User.FirstOrDefaultAsync(u => u.Id == studentModel.StudentUserId);
                        if (studentUser != null)
                        {
                            studentModel.StudentUserAccount = new
                            {
                                studentUser.Email,
                                studentUser.PhoneNumber,
                                studentUser.IsActive,
                                studentUser.LockoutEnd,
                                studentUser.AccessFailedCount
                            };
                        }
                    }
                    
                    // Get parent user account info
                    if (!string.IsNullOrEmpty(studentModel.ParentId))
                    {
                        var parentUser = await DbContext.User.FirstOrDefaultAsync(u => u.Id == studentModel.ParentId);
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
                    
                    // ✅ Get siblings (students with same parent) - moved outside parent user block
                    // Check by ParentId OR FatherCNIC for backward compatibility
                    studentModel.Siblings = new List<StudentModel>(); // Initialize empty list
                    
                    if (!string.IsNullOrEmpty(studentModel.ParentId) || !string.IsNullOrEmpty(studentModel.FatherCNIC))
                    {
                        var siblings = await DbContext.Students
                            .Include(s => s.StudentInfo)
                            .Include(s => s.StudentParent)
                            .Include(s => s.ClassAssignments).ThenInclude(ca => ca.Class)
                            .Include(s => s.ClassAssignments).ThenInclude(ca => ca.Section)
                            .Where(s => (s.StudentInfo.ParentId == studentModel.ParentId && !string.IsNullOrEmpty(studentModel.ParentId))
                                     || (s.StudentParent.FatherCNIC == studentModel.FatherCNIC && !string.IsNullOrEmpty(studentModel.FatherCNIC)))
                            .Where(s => s.Id != studentId && !s.IsDeleted)
                            .Select(s => new
                            {
                                s.Id,
                                s.StudentInfo.AdmissionNo,
                                FirstName = s.StudentInfo.FirstName,
                                LastName = s.StudentInfo.LastName,
                                FullName = s.StudentInfo.FirstName + " " + s.StudentInfo.LastName,
                                Gender = s.StudentInfo.Gender.ToString(),
                                ClassName = s.ClassAssignments.FirstOrDefault().Class.Name,
                                SectionName = s.ClassAssignments.FirstOrDefault().Section.Name,
                                s.StudentInfo.Dob,
                                Image = s.StudentInfo.Image
                            })
                            .ToListAsync();
                        
                        studentModel.Siblings = siblings.Select(s => new StudentModel
                        {
                            Id = s.Id,
                            AdmissionNo = s.AdmissionNo,
                            FirstName = s.FirstName,
                            LastName = s.LastName,
                            Gender = s.Gender,
                            ClassName = s.ClassName,
                            SectionName = s.SectionName,
                            Dob = s.Dob,
                            // Add any other essential fields
                        }).ToList();
                    }
                    
                    // Get attendance summary
                    var attendanceRecords = await DbContext.StudentAttendance
                        .Where(a => a.StudentId == studentId && !a.IsDeleted)
                        .ToListAsync();
                    
                    // ✅ Debug logging
                    Console.WriteLine($"📊 Student {studentId}: Found {attendanceRecords.Count} attendance records");
                    
                    studentModel.AttendanceSummary = new
                    {
                        TotalPresent = attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Present),
                        TotalAbsent = attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Absent),
                        TotalLeave = attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Excused),
                        TotalLate = attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Late),
                        Total = attendanceRecords.Count,
                        AttendancePercentage = attendanceRecords.Count > 0 
                            ? Math.Round((double)attendanceRecords.Count(a => a.AttendanceType == AttendanceType.Present) / attendanceRecords.Count * 100, 2)
                            : 0
                    };
                    
                    // Get fee summary
                    var feeAssignments = await DbContext.FeeAssignment
                        .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeType)
                        .Include(f => f.FeeGroupFeeType).ThenInclude(fgft => fgft.FeeGroup)
                        .Include(f => f.AppliedDiscount)
                        .Include(f => f.FeeTransactions)
                        .Where(f => f.StudentId == studentId && !f.IsDeleted)
                        .ToListAsync();
                    
                    // ✅ Calculate fee summary with correct fine and discount logic
                    var today = DateTime.Now.Date;
                    decimal totalFees = 0;
                    decimal totalPaid = 0;
                    decimal totalDiscountFromAssignment = 0;  // Base discount from FeeAssignment
                    decimal additionalDiscountFromTransactions = 0;  // Additional discount applied during payment
                    decimal totalFine = 0;
                    decimal totalOverdue = 0;
                    int paidCount = 0;
                    int pendingCount = 0;
                    
                    foreach (var fee in feeAssignments)
                    {
                        totalFees += fee.Amount;
                        
                        // ✅ Calculate PaidAmount from FeeTransactions (don't rely on fee.PaidAmount field)
                        var paidFromTransactions = fee.FeeTransactions.Sum(t => t.AmountPaid);
                        totalPaid += paidFromTransactions;
                        
                        // ✅ Calculate discount correctly:
                        // 1. Add assignment discount (base/initial discount)
                        totalDiscountFromAssignment += fee.AmountDiscount;
                        
                        // 2. Check if there's ADDITIONAL discount from transactions
                        if (fee.FeeTransactions.Any())
                        {
                            var transactionTotalDiscount = fee.FeeTransactions.Sum(t => t.DiscountApplied);
                            var assignmentDiscount = fee.AmountDiscount;
                            // Add only the ADDITIONAL discount (if transaction discount > assignment discount)
                            var additionalDiscount = Math.Max(0, transactionTotalDiscount - assignmentDiscount);
                            additionalDiscountFromTransactions += additionalDiscount;
                        }
                        
                        // ✅ Calculate total discount for this specific fee (for status calculation)
                        var totalDiscountForThisFee = fee.AmountDiscount + (fee.FeeTransactions.Any() 
                            ? Math.Max(0, fee.FeeTransactions.Sum(t => t.DiscountApplied) - fee.AmountDiscount) 
                            : 0);
                        
                        // ✅ Smart fine logic:
                        // 1. If PAID: Check if fine was collected (from transactions)
                        // 2. If UNPAID/PARTIAL: Only show fine if overdue
                        var isPaidInFull = paidFromTransactions >= (fee.Amount - totalDiscountForThisFee);
                        var isOverdue = fee.DueDate < today;
                        
                        if (isPaidInFull)
                        {
                            paidCount++;
                            // For paid fees: Check if fine was collected
                            var hasCollectedFine = fee.FeeTransactions.Any(t => t.FineApplied > 0);
                            if (hasCollectedFine)
                            {
                                totalFine += fee.AmountFine;
                            }
                        }
                        else if (isOverdue)
                        {
                            // For unpaid/partial fees: Add fine only if overdue
                            totalFine += fee.AmountFine;
                            
                            // Calculate overdue amount
                            var overdueAmount = fee.Amount - paidFromTransactions + fee.AmountFine - totalDiscountForThisFee;
                            totalOverdue += Math.Max(0, overdueAmount);
                        }
                        else if (paidFromTransactions == 0)
                        {
                            pendingCount++;
                        }
                    }
                    
                    // ✅ Total discount = Assignment discount + Additional transaction discount
                    var totalDiscount = totalDiscountFromAssignment + additionalDiscountFromTransactions;
                    
                    // ✅ Calculate total pending (never negative)
                    var totalPending = Math.Max(0, totalFees - totalPaid + totalFine - totalDiscount);
                    
                    // ✅ Manually map FeeAssignments to include calculated PaidAmount and TotalDiscount from transactions
                    var mappedFeeAssignments = feeAssignments.Select(f => {
                        var model = mapper.Map<Common.DTO.Response.FeeAssignmentModel>(f);
                        
                        // ✅ Override PaidAmount with calculated value from transactions
                        model.PaidAmount = f.FeeTransactions.Sum(t => t.AmountPaid);
                        
                        // ✅ Calculate TOTAL discount (base + additional from transactions)
                        if (f.FeeTransactions.Any())
                        {
                            var transactionTotalDiscount = f.FeeTransactions.Sum(t => t.DiscountApplied);
                            // Use the higher value (transaction discount includes base + additional)
                            model.AmountDiscount = Math.Max(f.AmountDiscount, transactionTotalDiscount);
                        }
                        
                        return model;
                    }).ToList();
                    
                    studentModel.FeeSummary = new
                    {
                        TotalFees = totalFees,
                        TotalPaid = totalPaid,
                        TotalDiscount = totalDiscount,
                        TotalFine = totalFine,
                        TotalPending = totalPending,
                        TotalOverdue = totalOverdue,
                        PendingCount = pendingCount,
                        PaidCount = paidCount,
                        FeeAssignments = mappedFeeAssignments
                    };
                    
                    // Get recent exam results (last 10)
                    var examResults = await DbContext.ExamResults
                        .Include(er => er.Exam)
                        .Where(er => er.StudentId == studentId && !er.IsDeleted)
                        .OrderByDescending(er => er.CreatedAt)
                        .Take(10)
                        .ToListAsync();
                    
                    studentModel.RecentExamResults = mapper.Map<List<Common.DTO.Response.ExamResultModel>>(examResults);
                }
                
                // Get last admission number
                var lastAdmissionNo = await DbContext.StudentInfo
                    .Where(si => !si.IsDeleted && !string.IsNullOrEmpty(si.AdmissionNo))
                    .OrderByDescending(si => si.Id)
                    .Select(si => si.AdmissionNo)
                    .FirstOrDefaultAsync();

                var lastId = students.Any() ? students.Max(s => s.Id) : 0;

                return (studentModels, totalCount, lastId, lastAdmissionNo ?? "");
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                throw new Exception($"Failed to get students: {ex.Message}", ex);
            }
        }
    }
}