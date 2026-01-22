using AutoMapper;
using Domain.Entities;
using Common.DTO;
using Common.DTO.Response;
using Common.DTO.Request;
using SMSBACKEND.Domain.Entities;

namespace Application.Mappings
{
    public class ModelMapper : Profile
    {
        public ModelMapper()
        {
            
            CreateMap<ApplicationUser, ApplicationUserModel>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles)).ReverseMap();
            CreateMap<ApplicationUserRole, ApplicationUserRoleModel>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role)).ReverseMap();
            CreateMap<NotificationTemplate, NotificationTemplateModel>().ReverseMap();

            // automatic code generation area

            CreateMap<StudentClassAssignment, StudentClassAssignmentModel>()
                .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => src.Section))
                .ForMember(dest => dest.Student, opt => opt.Ignore()) // Prevent circular reference
                .ReverseMap()
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.Section, opt => opt.Ignore())
                .ForMember(dest => dest.Session, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore());
            CreateMap<ApplicationRole, ApplicationRoleModel>().ReverseMap();

            // Student normalized entities mappings
            CreateMap<StudentInfo, StudentInfoModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<StudentAddress, StudentAddressModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<StudentFinancial, StudentFinancialModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<StudentParent, StudentParentModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<StudentMiscellaneous, StudentMiscellaneousModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ReverseMap();

            // Main Student entity mappings (now using computed properties)
            CreateMap<StudentModel, Student>()
            .ForMember(dest => dest.StudentInfo, opt => opt.Ignore())
            .ForMember(dest => dest.StudentAddress, opt => opt.Ignore())
            .ForMember(dest => dest.StudentFinancial, opt => opt.Ignore())
            .ForMember(dest => dest.StudentParent, opt => opt.Ignore())
            .ForMember(dest => dest.StudentMiscellaneous, opt => opt.Ignore())
            .ForMember(dest => dest.ClassAssignments, opt => opt.Ignore());

            CreateMap<Student, StudentModel>()
                // BaseEntity properties mapping
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => src.CreatedAtUtc))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt))
                .ForMember(dest => dest.DeletedBy, opt => opt.MapFrom(src => src.DeletedBy))
                .ForMember(dest => dest.Tenant, opt => opt.Ignore()) // Navigation property in BaseClass
                
                // Basic student info from StudentInfo (with null checks)
                .ForMember(dest => dest.AdmissionNo, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.AdmissionNo : null))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.FirstName : null))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.LastName : null))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.StudentInfo != null ? $"{src.StudentInfo.FirstName} {src.StudentInfo.LastName}" : ""))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Dob : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Gender : null))
                .ForMember(dest => dest.BFormNumber, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.BFormNumber : null))
                .ForMember(dest => dest.Religion, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Religion : null))
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Cast : null))
                .ForMember(dest => dest.PhoneNo, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.PhoneNo : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Email : null))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.MobileNo : null))
                .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.BloodGroup : null))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Height : null))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Weight : null))
                .ForMember(dest => dest.MeasurementDate, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.MeasurementDate : null))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Image : null))
                .ForMember(dest => dest.AdmissionDate, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.AdmissionDate : null))
                .ForMember(dest => dest.RollNo, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.RollNo : null))
                .ForMember(dest => dest.StudentUserId, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.StudentUserId : null))
                
                // Address information from StudentAddress (with null checks)
                .ForMember(dest => dest.CurrentAddress, opt => opt.MapFrom(src => src.StudentAddress != null ? src.StudentAddress.CurrentAddress : null))
                .ForMember(dest => dest.AreaAddress, opt => opt.MapFrom(src => src.StudentAddress != null ? src.StudentAddress.AreaAddress : null))
                .ForMember(dest => dest.PermanentAddress, opt => opt.MapFrom(src => src.StudentAddress != null ? src.StudentAddress.PermanentAddress : null))
                .ForMember(dest => dest.Zipcode, opt => opt.MapFrom(src => src.StudentAddress != null ? src.StudentAddress.Zipcode : null))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.StudentAddress != null ? src.StudentAddress.City : null))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.StudentAddress != null ? src.StudentAddress.State : null))
                
                // Financial information from StudentFinancial (with null checks)
                .ForMember(dest => dest.MonthlyFees, opt => opt.MapFrom(src => src.StudentFinancial != null ? src.StudentFinancial.MonthlyFees : 0))
                .ForMember(dest => dest.Arrears, opt => opt.MapFrom(src => src.StudentFinancial != null ? src.StudentFinancial.Arrears : 0))
                .ForMember(dest => dest.AdmissionFees, opt => opt.MapFrom(src => src.StudentFinancial != null ? src.StudentFinancial.AdmissionFees : null))
                .ForMember(dest => dest.SecurityDeposit, opt => opt.MapFrom(src => src.StudentFinancial != null ? src.StudentFinancial.SecurityDeposit : null))
                
                // Parent information from StudentParent (with null checks)
                .ForMember(dest => dest.FatherName, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.FatherName : null))
                .ForMember(dest => dest.FatherPhone, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.FatherPhone : null))
                .ForMember(dest => dest.FatherOccupation, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.FatherOccupation : null))
                .ForMember(dest => dest.FatherCNIC, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.FatherCNIC : null))
                .ForMember(dest => dest.FatherPic, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.FatherPic : null))
                .ForMember(dest => dest.MotherName, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.MotherName : null))
                .ForMember(dest => dest.MotherPhone, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.MotherPhone : null))
                .ForMember(dest => dest.MotherOccupation, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.MotherOccupation : null))
                .ForMember(dest => dest.MotherCNIC, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.MotherCNIC : null))
                .ForMember(dest => dest.MotherPic, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.MotherPic : null))
                .ForMember(dest => dest.GuardianIs, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianIs : null))
                .ForMember(dest => dest.GuardianName, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianName : null))
                .ForMember(dest => dest.GuardianRelation, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianRelation : null))
                .ForMember(dest => dest.GuardianEmail, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianEmail : null))
                .ForMember(dest => dest.GuardianPhone, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianPhone : null))
                .ForMember(dest => dest.GuardianOccupation, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianOccupation : null))
                .ForMember(dest => dest.GuardianAddress, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianAddress : null))
                .ForMember(dest => dest.GuardianAreaAddress, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianAreaAddress : null))
                .ForMember(dest => dest.GuardianPic, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.GuardianPic : null))
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.ParentId : null))
                
                // Miscellaneous information from StudentMiscellaneous (with null checks)
                .ForMember(dest => dest.PreviousSchool, opt => opt.MapFrom(src => src.StudentMiscellaneous != null ? src.StudentMiscellaneous.PreviousSchool : null))
                .ForMember(dest => dest.MedicalHistory, opt => opt.MapFrom(src => src.StudentMiscellaneous != null ? src.StudentMiscellaneous.MedicalHistory : null))
                .ForMember(dest => dest.PickupPerson, opt => opt.MapFrom(src => src.StudentMiscellaneous != null ? src.StudentMiscellaneous.PickupPerson : null))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.StudentMiscellaneous != null ? src.StudentMiscellaneous.Note : null))
                .ForMember(dest => dest.DisReason, opt => opt.MapFrom(src => src.StudentMiscellaneous != null ? src.StudentMiscellaneous.DisReason : 0))
                .ForMember(dest => dest.DisNote, opt => opt.MapFrom(src => src.StudentMiscellaneous != null ? src.StudentMiscellaneous.DisNote : null))
                .ForMember(dest => dest.DisableAt, opt => opt.MapFrom(src => src.StudentMiscellaneous != null ? src.StudentMiscellaneous.DisableAt : null))
                
                // Class assignment information (with null checks)
                .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.ClassAssignments != null && src.ClassAssignments.Any() ? src.ClassAssignments.FirstOrDefault().ClassId : 0))
                .ForMember(dest => dest.SectionId, opt => opt.MapFrom(src => src.ClassAssignments != null && src.ClassAssignments.Any() ? src.ClassAssignments.FirstOrDefault().SectionId : 0))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.ClassAssignments != null && src.ClassAssignments.Any() ? src.ClassAssignments.FirstOrDefault().SessionId : 0))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.CategoryId : null))
                .ForMember(dest => dest.SchoolHouseId, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.SchoolHouseId : null))
                
                // Class and Section names from ClassAssignments (with null checks)
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassAssignments != null && src.ClassAssignments.Any() && src.ClassAssignments.FirstOrDefault().Class != null ? src.ClassAssignments.FirstOrDefault().Class.Name : null))
                .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.ClassAssignments != null && src.ClassAssignments.Any() && src.ClassAssignments.FirstOrDefault().Section != null ? src.ClassAssignments.FirstOrDefault().Section.Name : null))
                .ForMember(dest => dest.SessionName, opt => opt.MapFrom(src => src.ClassAssignments != null && src.ClassAssignments.Any() && src.ClassAssignments.FirstOrDefault().Session != null ? src.ClassAssignments.FirstOrDefault().Session.Sessions : null))
                
                // Computed names from navigation properties
                .ForMember(dest => dest.SchoolHouseName, opt => opt.MapFrom(src => src.StudentInfo != null && src.StudentInfo.SchoolHouse != null ? src.StudentInfo.SchoolHouse.Name : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.StudentInfo != null && src.StudentInfo.StudentCategory != null ? src.StudentInfo.StudentCategory.Name : null))
                
                // CNIC mapping (appears to be FatherCNIC duplicate)
                .ForMember(dest => dest.FatherCNIC, opt => opt.MapFrom(src => src.StudentParent != null ? src.StudentParent.FatherCNIC : null))
                
                // Navigation properties (with null checks)
                .ForMember(dest => dest.SchoolHouse, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.SchoolHouse : null))
                .ForMember(dest => dest.StudentCategory, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.StudentCategory : null))
                .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.Parent : null))
                .ForMember(dest => dest.StudentUser, opt => opt.MapFrom(src => src.StudentInfo != null ? src.StudentInfo.StudentUser : null))
                
                // ClassAssignments collection mapping (prevent circular reference)
                .ForMember(dest => dest.ClassAssignments, opt => opt.MapFrom(src => 
                    src.ClassAssignments != null ? src.ClassAssignments.Select(ca => new StudentClassAssignmentModel
                    {
                        StudentId = ca.StudentId,
                        ClassId = ca.ClassId,
                        SectionId = ca.SectionId,
                        SessionId = ca.SessionId,
                        AssignedDate = ca.AssignedDate,
                        Remarks = ca.Remarks ?? string.Empty,
                        Class = ca.Class != null ? new ClassModel { Id = ca.Class.Id, Name = ca.Class.Name } : null,
                        Section = ca.Section != null ? new SectionModel { Id = ca.Section.Id, Name = ca.Section.Name } : null,
                        Student = null, // Prevent circular reference
                        TenantId = ca.TenantId,
                        CreatedAt = ca.CreatedAt,
                        CreatedAtUtc = ca.CreatedAtUtc,
                        CreatedBy = ca.CreatedBy ?? string.Empty,
                        UpdatedAt = ca.UpdatedAt,
                        UpdatedBy = ca.UpdatedBy ?? string.Empty,
                        IsDeleted = ca.IsDeleted,
                        DeletedAt = ca.DeletedAt,
                        DeletedBy = ca.DeletedBy
                    }).ToList() : new List<StudentClassAssignmentModel>()))
                
                // Computed properties - ignore as they are read-only
                .ForMember(dest => dest.FullName, opt => opt.Ignore())
                
                // Siblings collection - ignore for now (would require additional query)
                .ForMember(dest => dest.Siblings, opt => opt.Ignore())
                
                // SyncStatus - default value, not from entity
                .ForMember(dest => dest.SyncStatus, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Class, ClassModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore()) // Navigation property from BaseClass
                //.ForMember(dest => dest.SelectedSectionIds, opt => opt.Ignore()) // UI-only property
                //.ForMember(dest => dest.SelectedSubjectIds, opt => opt.Ignore()) // UI-only property
                //.ForMember(dest => dest.ClassSections, opt => opt.Ignore()) // Prevent circular reference
                //.ForMember(dest => dest.SubjectClasses, opt => opt.Ignore()) // Prevent circular reference
                //.ForMember(dest => dest.ClassTeachers, opt => opt.Ignore()) // Prevent circular reference
                //.ForMember(dest => dest.StudentAssignments, opt => opt.Ignore()) // Prevent circular reference
                .ReverseMap();
            CreateMap<ClassSection, ClassSectionModel>()
                .ForMember(dest => dest.Class, opt => opt.MapFrom(src => src.Class))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => src.Section))
                .ForMember(dest => dest.ClassName, opt => opt.Ignore()) // Computed property
                .ForMember(dest => dest.SectionName, opt => opt.Ignore()) // Computed property
                .ReverseMap()
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.Section, opt => opt.Ignore());
            CreateMap<Section, SectionModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore()) // Navigation property from BaseClass
                .ReverseMap();

            CreateMap<Subject, SubjectModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<RefreshToken, RefreshTokenModel>().ReverseMap();
            CreateMap<SubjectClass, SubjectClassModel>().ReverseMap();
            // ✅ Department mapping - ignore Tenant in both directions
            CreateMap<Department, DepartmentModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<DepartmentModel, Department>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            // ✅ Staff mapping - ignore Tenant and Department navigation in both directions
            CreateMap<Staff, StaffModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<StaffModel, Staff>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore());
            CreateMap<StudentCategory, StudentCategoryModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<ClassTeacher, ClassTeacherModel>().ReverseMap();
            CreateMap<SchoolHouse, SchoolHouseModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<TeacherSubjects, TeacherSubjectsModel>().ReverseMap();
            CreateMap<Timetable, TimetableModel>().ReverseMap();
            CreateMap<Timetable, TimetableEntryModel>().ReverseMap();
            CreateMap<StudentAttendance, StudentAttendanceModel>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                    src.Student != null && src.Student.StudentInfo != null 
                    ? $"{src.Student.StudentInfo.FirstName} {src.Student.StudentInfo.LastName}".Trim()
                    : string.Empty))
                .ForMember(dest => dest.AdmissionNo, opt => opt.MapFrom(src => 
                    src.Student != null && src.Student.StudentInfo != null 
                    ? src.Student.StudentInfo.AdmissionNo 
                    : string.Empty))
                .ForMember(dest => dest.RollNo, opt => opt.MapFrom(src => 
                    src.Student != null && src.Student.StudentInfo != null 
                    ? src.Student.StudentInfo.RollNo 
                    : string.Empty))
                .ReverseMap();
            CreateMap<DisableReason, DisableReasonModel>().ReverseMap();
            
            // ✅ FeeType - Ignore Tenant navigation property
            CreateMap<FeeType, FeeTypeModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<FeeTypeModel, FeeType>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            
            // ✅ FeeGroup - Ignore Tenant navigation property
            CreateMap<FeeGroup, FeeGroupModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<FeeGroupModel, FeeGroup>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            
            // ✅ FeeGroupFeeType - Ignore navigation properties
            CreateMap<FeeGroupFeeType, FeeGroupFeeTypeModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<FeeGroupFeeTypeModel, FeeGroupFeeType>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.FeeGroup, opt => opt.Ignore())
                .ForMember(dest => dest.FeeType, opt => opt.Ignore())
                .ForMember(dest => dest.FeeDiscount, opt => opt.Ignore());
            
            // ✅ FeeDiscount - Ignore Tenant navigation property
            CreateMap<FeeDiscount, FeeDiscountModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<FeeDiscountModel, FeeDiscount>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<FeeTransaction, FeeTransactionModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<FeeTransactionModel, FeeTransaction>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<FeeAssignment, FeeAssignmentModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<FeeAssignmentModel, FeeAssignment>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<Permission, PermissionModel>().ReverseMap();

            // Exam Module Mappings
            CreateMap<Exam, ExamModel>().ReverseMap();
            CreateMap<ExamCreateRequest, Exam>().ReverseMap();
            CreateMap<ExamUpdateRequest, Exam>().ReverseMap();
            
            CreateMap<ExamSubject, ExamSubjectModel>().ReverseMap();
            CreateMap<ExamSubjectCreateRequest, ExamSubject>().ReverseMap();
            CreateMap<ExamSubjectUpdateRequest, ExamSubject>().ReverseMap();
            
            CreateMap<ExamMarks, ExamMarksModel>().ReverseMap();
            CreateMap<ExamMarksCreateRequest, ExamMarks>().ReverseMap();
            CreateMap<ExamMarksUpdateRequest, ExamMarks>().ReverseMap();
            
            CreateMap<GradeScale, GradeScaleModel>().ReverseMap();
            CreateMap<Grade, GradeModel>().ReverseMap();
            
            CreateMap<ExamResult, ExamResultModel>().ReverseMap();
            CreateMap<ExamTimetable, ExamTimetableModel>().ReverseMap();
            CreateMap<MarksheetTemplate, MarksheetTemplateModel>().ReverseMap();
            CreateMap<RolePermission, RolePermissionModel>().ReverseMap();
            CreateMap<Module, ModuleModel>()
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src =>
                    src.Children.Select(child => new ModuleModel
                    {
                        Id = child.Id,
                        Name = child.Name,
                        Icon = child.Icon,
                        Description = child.Description,
                        IsActive = child.IsActive,
                        IsDashboard = child.IsDashboard,
                        IsOpen = child.IsOpen,
                        OrderById = child.OrderById,
                        ParentId = child.ParentId,
                        Path = child.Path,
                        Type = child.Type,
                        Parent = null,
                        Children = null,
                        // map other fields as needed
                        Permissions = child.Permissions.Select(p => new PermissionModel { Id = p.Id, Name = p.Name }).ToList()
                    }).ToList()))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                    src.Permissions.Select(p => new PermissionModel { Id = p.Id, Name = p.Name }).ToList()))
                .ForMember(dest => dest.Parent, opt => opt.Ignore());
            // ✅ ExpenseHead mapping - ignore Tenant in both directions
            CreateMap<ExpenseHead, ExpenseHeadModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<ExpenseHeadModel, ExpenseHead>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            
            // ✅ Expense mapping - ignore Tenant and ExpenseHead navigation in both directions
            CreateMap<Expense, ExpenseModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<ExpenseModel, Expense>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.ExpenseHead, opt => opt.Ignore());
            
            // ✅ IncomeHead mapping - ignore Tenant in both directions
            CreateMap<IncomeHead, IncomeHeadModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<IncomeHeadModel, IncomeHead>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            
            // ✅ Income mapping - ignore Tenant and IncomeHead navigation in both directions
            CreateMap<Income, IncomeModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<IncomeModel, Income>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.IncomeHead, opt => opt.Ignore());
            CreateMap<Enquiry, EnquiryModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<EnquiryModel, Enquiry>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.PreAdmission, opt => opt.Ignore()); // Only ignore on write (Enquiry doesn't have reverse navigation)
            CreateMap<PreAdmission, PreAdmissionModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : null))
                .ForMember(dest => dest.EnquiryFullName, opt => opt.MapFrom(src => src.Enquiry != null ? src.Enquiry.FullName : null))
                .ForMember(dest => dest.FeeGroupName, opt => opt.MapFrom(src => src.FeeGroupFeeType != null && src.FeeGroupFeeType.FeeGroup != null ? src.FeeGroupFeeType.FeeGroup.Name : null));
            CreateMap<PreAdmissionModel, PreAdmission>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.Enquiry, opt => opt.Ignore());
            CreateMap<EntryTest, EntryTestModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<EntryTestModel, EntryTest>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.Application, opt => opt.Ignore());
            CreateMap<Admission, AdmissionModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.Name : null))
                .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.Section != null ? src.Section.Name : null))
                .ForMember(dest => dest.TestNumber, opt => opt.MapFrom(src => src.EntryTest != null ? src.EntryTest.TestNumber : null))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : null));
            CreateMap<AdmissionModel, Admission>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.Section, opt => opt.Ignore())
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.EntryTest, opt => opt.Ignore())
                .ForMember(dest => dest.Student, opt => opt.Ignore());

            CreateMap<FineWaiver, FineWaiverModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : ""))
                .ForMember(dest => dest.FeeTypeName, opt => opt.MapFrom(src => src.FeeAssignment != null && src.FeeAssignment.FeeGroupFeeType != null && src.FeeAssignment.FeeGroupFeeType.FeeType != null ? src.FeeAssignment.FeeGroupFeeType.FeeType.Name : ""))
                .ForMember(dest => dest.FeeGroupName, opt => opt.MapFrom(src => src.FeeAssignment != null && src.FeeAssignment.FeeGroupFeeType != null && src.FeeAssignment.FeeGroupFeeType.FeeGroup != null ? src.FeeAssignment.FeeGroupFeeType.FeeGroup.Name : ""))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.FeeAssignment != null ? src.FeeAssignment.Month : 0))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.FeeAssignment != null ? src.FeeAssignment.Year : 0))
                .ForMember(dest => dest.RemainingFineAmount, opt => opt.MapFrom(src => src.OriginalFineAmount - src.WaiverAmount));
            CreateMap<FineWaiverModel, FineWaiver>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<Session, SessionModel>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            CreateMap<SessionModel, Session>()
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());
            // automatic code generation area end

        }
    }
}
