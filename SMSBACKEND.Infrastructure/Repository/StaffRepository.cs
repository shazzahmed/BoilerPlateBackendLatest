
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using AutoMapper;
using Common.DTO.Request;
using Common.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Domain.Entities;

namespace Infrastructure.Repository
{
    public class StaffRepository : BaseRepository<Staff, int>, IStaffRepository
    {
        public StaffRepository(IMapper mapper, ISqlServerDbContext context) : base(mapper, context)
        {
        }
        public async Task<StaffModel> CreateStaff(StaffCreateReq model)
        {
            var staff = await FirstOrDefaultAsync(x => x.UserId == model.UserId);
            if (staff == null)
            {
                Staff newStaff = new Staff
                {
                    // Basic Information
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserId = model.UserId,
                    DepartmentId = model.DepartmentId,
                    CNIC = model.CNIC,
                    Email = model.Email,
                    Gender = model.Gender,
                    Dob = model.Dob,
                    DateOfJoining = model.DateOfJoining,
                    DateOfLeaving = model.DateOfLeaving,
                    StaffId = model.StaffId,
                    Image = model.Image,
                    LangId = model.LangId,
                    IsActive = true,
                    
                    // Family Information
                    FatherName = model.FatherName,
                    MotherName = model.MotherName,
                    MaritalStatus = model.MaritalStatus,
                    Religion = model.Religion,
                    
                    // Contact Information
                    ContactNo = model.ContactNo,
                    PhoneNo = model.PhoneNo,
                    EmergencyContactNo = model.EmergencyContactNo,
                    
                    // Address Information
                    StreetAddress = model.StreetAddress,
                    City = model.City,
                    State = model.State,
                    Country = model.Country,
                    PostalCode = model.PostalCode,
                    Zipcode = model.Zipcode,
                    LocalAddress = model.LocalAddress,
                    PermanentAddress = model.PermanentAddress,
                    
                    // Employment Information
                    Specialization = model.Specialization,
                    Designation = model.Designation,
                    EmployeeCode = model.EmployeeCode,
                    Qualification = model.Qualification,
                    WorkExp = model.WorkExp,
                    PreviousOrganization = model.PreviousOrganization,
                    Note = model.Note,
                    
                    // System Access
                    BiometricId = model.BiometricId,
                    RfidCode = model.RfidCode,
                    
                    // Banking Information
                    AccountTitle = model.AccountTitle,
                    BankAccountNo = model.BankAccountNo,
                    BankName = model.BankName,
                    IfscCode = model.IfscCode,
                    BankBranch = model.BankBranch,
                    Payscale = model.Payscale,
                    BasicSalary = model.BasicSalary,
                    Allowances = model.Allowances,
                    Deductions = model.Deductions,
                    EpfNo = model.EpfNo,
                    ContractType = model.ContractType,
                    Shift = model.Shift,
                    Location = model.Location,
                    
                    // Social Media
                    Facebook = model.Facebook,
                    Twitter = model.Twitter,
                    Linkedin = model.Linkedin,
                    Instagram = model.Instagram,
                    
                    // Documents
                    Resume = model.Resume,
                    JoiningLetter = model.JoiningLetter,
                    ResignationLetter = model.ResignationLetter,
                    OtherDocumentName = model.OtherDocumentName,
                    OtherDocumentFile = model.OtherDocumentFile,
                };
                //staff = mapper.Map<StaffModel, Staff>(newStaff);
                try
                {
                    DbContext.Staff.Add(newStaff);
                    await DbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }
            } else
            {
                // ✅ Update existing staff - all fields
                // Basic Information
                staff.FirstName = model.FirstName;
                staff.LastName = model.LastName;
                staff.UserId = model.UserId;
                staff.DepartmentId = model.DepartmentId;
                staff.CNIC = model.CNIC;
                staff.Email = model.Email;
                staff.Gender = model.Gender;
                staff.Dob = model.Dob;
                staff.DateOfJoining = model.DateOfJoining;
                staff.DateOfLeaving = model.DateOfLeaving;
                staff.StaffId = model.StaffId;
                staff.Image = model.Image;
                staff.LangId = model.LangId;
                staff.IsActive = true;
                
                // Family Information
                staff.FatherName = model.FatherName;
                staff.MotherName = model.MotherName;
                staff.MaritalStatus = model.MaritalStatus;
                staff.Religion = model.Religion;
                
                // Contact Information
                staff.ContactNo = model.ContactNo;
                staff.PhoneNo = model.PhoneNo;
                staff.EmergencyContactNo = model.EmergencyContactNo;
                
                // Address Information
                staff.StreetAddress = model.StreetAddress;
                staff.City = model.City;
                staff.State = model.State;
                staff.Country = model.Country;
                staff.PostalCode = model.PostalCode;
                staff.Zipcode = model.Zipcode;
                staff.LocalAddress = model.LocalAddress;
                staff.PermanentAddress = model.PermanentAddress;
                
                // Employment Information
                staff.Specialization = model.Specialization;
                staff.Designation = model.Designation;
                staff.EmployeeCode = model.EmployeeCode;
                staff.Qualification = model.Qualification;
                staff.WorkExp = model.WorkExp;
                staff.PreviousOrganization = model.PreviousOrganization;
                staff.Note = model.Note;
                
                // System Access
                staff.BiometricId = model.BiometricId;
                staff.RfidCode = model.RfidCode;
                
                // Banking Information
                staff.AccountTitle = model.AccountTitle;
                staff.BankAccountNo = model.BankAccountNo;
                staff.BankName = model.BankName;
                staff.IfscCode = model.IfscCode;
                staff.BankBranch = model.BankBranch;
                staff.Payscale = model.Payscale;
                staff.BasicSalary = model.BasicSalary;
                staff.Allowances = model.Allowances;
                staff.Deductions = model.Deductions;
                staff.EpfNo = model.EpfNo;
                staff.ContractType = model.ContractType;
                staff.Shift = model.Shift;
                staff.Location = model.Location;
                
                // Social Media
                staff.Facebook = model.Facebook;
                staff.Twitter = model.Twitter;
                staff.Linkedin = model.Linkedin;
                staff.Instagram = model.Instagram;
                
                // Documents
                staff.Resume = model.Resume;
                staff.JoiningLetter = model.JoiningLetter;
                staff.ResignationLetter = model.ResignationLetter;
                staff.OtherDocumentName = model.OtherDocumentName;
                staff.OtherDocumentFile = model.OtherDocumentFile;

                try
                {
                    await UpdateAsync(staff);
                    await DbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            staff = await FirstOrDefaultAsync(x => x.UserId == model.UserId && !x.IsDeleted, null, includeProperties: i => i.Include(s => s.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role).Include(s => s.Department));
            return mapper.Map<StaffModel>(staff);
        }

        public async Task<StaffModel> UpdateStaff(StaffCreateReq model)
        {
            var staff = await FirstOrDefaultAsync(x => x.UserId == model.UserId && !x.IsDeleted);
            
            // ✅ Update all fields - organized by category
            // Basic Information
            staff.FirstName = model.FirstName;
            staff.LastName = model.LastName;
            staff.UserId = model.UserId;
            staff.DepartmentId = model.DepartmentId;
            staff.CNIC = model.CNIC;
            staff.Email = model.Email;
            staff.Gender = model.Gender;
            staff.Dob = model.Dob;
            staff.DateOfJoining = model.DateOfJoining;
            staff.DateOfLeaving = model.DateOfLeaving;
            staff.StaffId = model.StaffId;
            staff.Image = model.Image;
            staff.LangId = model.LangId;
            staff.IsActive = true;
            
            // Family Information
            staff.FatherName = model.FatherName;
            staff.MotherName = model.MotherName;
            staff.MaritalStatus = model.MaritalStatus;
            staff.Religion = model.Religion;
            
            // Contact Information
            staff.ContactNo = model.ContactNo;
            staff.PhoneNo = model.PhoneNo;
            staff.EmergencyContactNo = model.EmergencyContactNo;
            
            // Address Information
            staff.StreetAddress = model.StreetAddress;
            staff.City = model.City;
            staff.State = model.State;
            staff.Country = model.Country;
            staff.PostalCode = model.PostalCode;
            staff.Zipcode = model.Zipcode;
            staff.LocalAddress = model.LocalAddress;
            staff.PermanentAddress = model.PermanentAddress;
            
            // Employment Information
            staff.Specialization = model.Specialization;
            staff.Designation = model.Designation;
            staff.EmployeeCode = model.EmployeeCode;
            staff.Qualification = model.Qualification;
            staff.WorkExp = model.WorkExp;
            staff.PreviousOrganization = model.PreviousOrganization;
            staff.Note = model.Note;
            
            // System Access
            staff.BiometricId = model.BiometricId;
            staff.RfidCode = model.RfidCode;
            
            // Banking Information
            staff.AccountTitle = model.AccountTitle;
            staff.BankAccountNo = model.BankAccountNo;
            staff.BankName = model.BankName;
            staff.IfscCode = model.IfscCode;
            staff.BankBranch = model.BankBranch;
            staff.Payscale = model.Payscale;
            staff.BasicSalary = model.BasicSalary;
            staff.Allowances = model.Allowances;
            staff.Deductions = model.Deductions;
            staff.EpfNo = model.EpfNo;
            staff.ContractType = model.ContractType;
            staff.Shift = model.Shift;
            staff.Location = model.Location;
            
            // Social Media
            staff.Facebook = model.Facebook;
            staff.Twitter = model.Twitter;
            staff.Linkedin = model.Linkedin;
            staff.Instagram = model.Instagram;
            
            // Documents
            staff.Resume = model.Resume;
            staff.JoiningLetter = model.JoiningLetter;
            staff.ResignationLetter = model.ResignationLetter;
            staff.OtherDocumentName = model.OtherDocumentName;
            staff.OtherDocumentFile = model.OtherDocumentFile;

            try
            {
                await UpdateAsync(staff);
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

            return mapper.Map<StaffModel>(staff);
        }

        public async Task<(List<StaffModel> Staffs, int Count, int lastId)> GetAllStaff(FilterParams filters)
        {
            try
            {
                var baseQuery = DbContext.Staff
                    .Include(s => s.User).ThenInclude(u => u.UserRoles).ThenInclude(ur => ur.Role)
                    .Include(s => s.Department)
                    .Include(s => s.TeacherSubjects).ThenInclude(ur => ur.Subject);
                int count = await baseQuery.CountAsync();

                var staffs = mapper.Map<List<StaffModel>>(await baseQuery
                    .AsNoTracking()
                    .ToListAsync());
                staffs.ForEach(s =>
                {
                    s.DepartmentName = s.Department?.Name;
                    s.Roles = s.User.UserRoles.Select(ur => ur.Role.Name).ToList(); // Explicit conversion to List<string>  
                    s.RoleIds = s.User.UserRoles.Select(ur => ur.Role.Id).ToList(); // Explicit conversion to List<int>

                    if (s.TeacherSubjects != null)
                    {
                        foreach (var ts in s.TeacherSubjects)
                        {
                            ts.SubjectName = ts.Subject?.Name;
                            ts.TeacherName = s.FullName; // or ts.Teacher?.FullName if needed
                        }
                    }
                });
                int lastId = await GetLastIdAsync();
                return (staffs, count, lastId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(List<TeacherSubjectsModel> teacherSubjects, int lastId)> GetTeacherSubjects(FilterParams filters)
        {
            var classTeachers = await DbContext.TeacherSubjects
                .Include(s => s.Teacher)
                .Include(s => s.Subject)
                .Where(ct => !ct.Subject.IsDeleted && !ct.Teacher.IsDeleted)
                .GroupBy(ts => new { ts.TeacherId })
                .Select(g => new TeacherSubjectsModel
                {
                    Id = g.First().Id,
                    TeacherId = g.Key.TeacherId,
                    TeacherName = g.First().Teacher.FirstName + " " + g.First().Teacher.LastName,
                    //SubjectNameList = g.Select(ts => ts.Subject.Name).Distinct().ToList(),
                    SubjectIds = g.Select(ts => ts.SubjectId).Distinct().ToList(),
                })
                .AsNoTracking()
                .ToListAsync();
            var lastId = await DbContext.TeacherSubjects
                .OrderByDescending(e => e.Id)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();
            return (classTeachers, lastId);
        }
        public async Task<int> AssignTeacherSubjects(TeacherSubjectsRequest request)
        {
            // Use manual execution strategy to handle retries with transactions
            var strategy = DbContext.Database.CreateExecutionStrategy();
            
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await DbContext.Database.BeginTransactionAsync();
                try
                {
                    // Get current active records from DB for this teacher
                    var existing = await DbContext.TeacherSubjects
                        .Where(ts => ts.TeacherId == request.TeacherId && !ts.IsDeleted)
                        .ToListAsync();

                    // Build the new set from request
                    var newSet = request.SubjectIds
                                .Select(subjectId => new
                                {
                                    SubjectId = subjectId
                                })
                                .ToList();

                    // Identify records to add (only those that don't already exist)
                    var toAdd = newSet
                        .Where(n => !existing.Any(e => e.SubjectId == n.SubjectId && !e.IsDeleted))
                        .Select(n => new TeacherSubjects
                        {
                            TeacherId = request.TeacherId,
                            SubjectId = n.SubjectId,
                            IsActive = true,
                            IsDeleted = false
                        })
                        .ToList();

                    // Identify records to remove (soft delete existing ones not in new set)
                    var toRemove = existing
                        .Where(e => !newSet.Any(n => n.SubjectId == e.SubjectId))
                        .ToList();

                    // Apply changes
                    if (toAdd.Any())
                    {
                        // Double-check for duplicates before adding (in case of race conditions)
                        var existingSubjectIds = existing.Select(e => e.SubjectId).ToList();
                        var uniqueToAdd = toAdd.Where(ta => !existingSubjectIds.Contains(ta.SubjectId)).ToList();
                        
                        if (uniqueToAdd.Any())
                        {
                            DbContext.TeacherSubjects.AddRange(uniqueToAdd);
                        }
                    }

                    if (toRemove.Any())
                        DbContext.TeacherSubjects.RemoveRange(toRemove);

                    var result = await DbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the error for debugging
                    Console.WriteLine($"Error in AssignTeacherSubjects: {ex.Message}");
                    throw;
                }
            });
        }

        public async Task<(List<ClassTeacherModel> classTeachers, int lastId)> GetClassTeachers(FilterParams filters)
        {
            var query = from ct in DbContext.ClassTeacher
                        join cs in DbContext.ClassSections on ct.ClassSectionId equals cs.Id
                        join c in DbContext.Classes on cs.ClassId equals c.Id
                        join s in DbContext.Sections on cs.SectionId equals s.Id
                        join staff in DbContext.Staff on ct.StaffId equals staff.Id
                        where ct.IsActive && !ct.IsDeleted
                        select new ClassTeacherModel
                        {
                            Id = ct.Id,
                            ClassSectionId = ct.ClassSectionId,
                            StaffId = ct.StaffId,
                            ClassName = c.Name,
                            SectionName = s.Name,
                            SessionId = ct.SessionId,
                            StaffNameList = new List<string> { staff.FirstName + " " + staff.LastName },
                            StaffIds = new List<int> { staff.Id }
                        };

            var classTeachers = await query
                .Skip((filters.PaginationParam.PageIndex - 1) * filters.PaginationParam.PageSize)
                .Take(filters.PaginationParam.PageSize)
                .ToListAsync();

            var lastId = await DbContext.ClassTeacher
                .OrderByDescending(e => e.Id)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            return (classTeachers, lastId);
        }

        public async Task<int> AssignClassTeacher(ClassTeacherRequest request)
        {
            // Get current records from DB for this class section
            var existing = await DbContext.ClassTeacher
                .Where(ct => ct.ClassSectionId == request.ClassSectionId)
                .ToListAsync();

            // Build the new set from request
            var newSet = request.StaffIds
                        .Select(staffId => new
                        {
                            ClassSectionId = request.ClassSectionId,
                            StaffId = staffId
                        })
                        .ToList();

            // Identify records to add
            var toAdd = newSet
                .Where(n => !existing.Any(e => e.ClassSectionId == n.ClassSectionId && e.StaffId == n.StaffId))
                .Select(n => new ClassTeacher
                {
                    ClassSectionId = n.ClassSectionId,
                    StaffId = n.StaffId,
                    SessionId = 2,
                    IsActive = true
                })
                .ToList();

            // Identify records to remove
            var toRemove = existing
                .Where(e => !newSet.Any(n => n.ClassSectionId == e.ClassSectionId && n.StaffId == e.StaffId))
                .ToList();

            // Apply changes
            if (toAdd.Any())
                DbContext.ClassTeacher.AddRange(toAdd);

            if (toRemove.Any())
                DbContext.ClassTeacher.RemoveRange(toRemove);

            return await DbContext.SaveChangesAsync();
        }
    }
}
