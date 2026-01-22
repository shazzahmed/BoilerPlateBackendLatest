# 🔄 Backend Boilerplate Conversion Plan

## 📋 Executive Summary

This document outlines the plan to convert the School Management System (SMS) backend into a **general-purpose boilerplate** that can be used for any type of application. The plan categorizes all components into:

- ✅ **KEEP** - General-purpose functionality (authentication, cache, notifications, etc.)
- ❌ **REMOVE** - School-specific code
- ⚠️ **REVIEW** - Components that may need modification

---

## 🎯 Core Principles

1. **Keep all infrastructure** (authentication, caching, notifications, multi-tenancy)
2. **Remove all domain-specific entities** (Student, Exam, Fee, etc.)
3. **Preserve base patterns** (Repository, Service, Controller base classes)
4. **Maintain clean architecture** structure
5. **Keep all cross-cutting concerns** (logging, error handling, validation)

---

## ✅ SECTION 1: KEEP - General Purpose Infrastructure

### 1.1 Authentication & Authorization ✅

**Files to KEEP:**
- `SMSBACKEND.Domain/Entities/IdentityModel.cs` (ApplicationUser, ApplicationRole, ApplicationUserRole)
- `SMSBACKEND.Domain/Entities/Permission.cs`
- `SMSBACKEND.Domain/Entities/RolePermission.cs`
- `SMSBACKEND.Domain/Entities/Module.cs`
- `SMSBACKEND.Domain/Entities/RefreshToken.cs`
- `SMSBACKEND.Domain/Entities/UserSession.cs`
- `SMSBACKEND.Infrastructure/Security/` (entire folder)
- `SMSBACKEND.Application/Security/ISecurityService.cs`
- `SMSBACKEND.Application/AuthenticationHelper/IAuthenticationHelper.cs`
- `SMSBACKEND.Infrastructure/Helpers/AuthenticationHelper.cs`
- `SMSBACKEND.Infrastructure/Middleware/JwtHandler.cs`
- `SMSBACKEND.Infrastructure/Middleware/IJwtHandler.cs`

**Services to KEEP:**
- `IUserService` / `UserService`
- `IRefreshTokenService` / `RefreshTokenService`
- `IPermissionService` / `PermissionService`
- `IRolePermissionService` / `RolePermissionService`
- `IMenuService` / `MenuService`

**Controllers to KEEP:**
- `AccountController.cs` (Login, Register, RefreshToken)
- `AdminController.cs` (User management, Roles, Permissions)

**Reason:** Core authentication/authorization is needed in every application.

---

### 1.2 Multi-Tenancy ✅

**Files to KEEP:**
- `SMSBACKEND.Domain/Entities/Tenant.cs`
- `SMSBACKEND.Domain/Entities/BaseEntity.cs` (contains TenantId)
- `SMSBACKEND.Infrastructure/Middleware/TenantMiddleware.cs`
- `SMSBACKEND.Infrastructure/Middleware/TenantValidationMiddleware.cs`
- `SMSBACKEND.Infrastructure/Services/TenantService.cs`
- `SMSBACKEND.Application/ServiceContracts/IServices/ITenantService.cs`

**Reason:** Multi-tenancy is a common requirement for SaaS applications.

---

### 1.3 Caching Infrastructure ✅

**Files to KEEP:**
- `SMSBACKEND.Application/ServiceContracts/IServices/ICacheProvider.cs`
- `SMSBACKEND.Infrastructure/Services/Services/DistributedMemoryCacheProvider.cs`
- `SMSBACKEND.Infrastructure/Services/Services/DistributedHybridCacheProvider.cs`
- `SMSBACKEND.Common/Utilities/StaticClasses/CacheKeyBuilder.cs`

**Reason:** Caching is essential for performance in any application.

---

### 1.4 Notification System ✅

**Files to KEEP:**
- `SMSBACKEND.Domain/Entities/Notification.cs`
- `SMSBACKEND.Domain/Entities/NotificationTemplate.cs`
- `SMSBACKEND.Domain/Entities/NotificationType.cs`
- `SMSBACKEND.Application/ServiceContracts/IServices/INotificationService.cs`
- `SMSBACKEND.Application/ServiceContracts/IServices/INotificationTemplateService.cs`
- `SMSBACKEND.Application/ServiceContracts/IServices/INotificationTypeService.cs`
- `SMSBACKEND.Infrastructure/Services/Services/NotificationService.cs`
- `SMSBACKEND.Presentation/Controllers/NotificationController.cs`

**Communication Services to KEEP:**
- `SMSBACKEND.Infrastructure/Services/Communication/` (entire folder)
  - `EmailService.cs` (interfaces)
  - `EmailSender.cs`
  - `EmailServiceGoogle.cs`
  - `EmailServiceOutlook.cs`
  - `EmailServicePlesk.cs`
  - `EmailServiceZoho.cs`
  - `SmsServiceTest.cs`
  - `SseService.cs` (Server-Sent Events)

**Reason:** Email/SMS notifications are needed in most applications.

---

### 1.5 Base Patterns & Infrastructure ✅

**Base Classes to KEEP:**
- `SMSBACKEND.Domain/RepositoriesContracts/IRepository/IBaseRepository.cs`
- `SMSBACKEND.Infrastructure/Repository/BaseRepository.cs`
- `SMSBACKEND.Application/ServiceContracts/IServices/IBaseService.cs`
- `SMSBACKEND.Infrastructure/Services/Services/BaseService.cs`
- `SMSBACKEND.Presentation/Controllers/BaseController.cs`

**Unit of Work:**
- `SMSBACKEND.Domain/RepositoriesContracts/IUnitOfWork.cs`
- `SMSBACKEND.Infrastructure/Repository/UnitOfWork.cs`

**Reason:** These are the foundation of the architecture pattern.

---

### 1.6 Database Infrastructure ✅

**Files to KEEP:**
- `SMSBACKEND.Infrastructure/Database/SqlServerDbContext.cs`
- `SMSBACKEND.Infrastructure/Database/ISqlServerDbContext.cs`
- `SMSBACKEND.Infrastructure/Database/DbMigrator.cs`
- `SMSBACKEND.Infrastructure/DependencyResolutions/RepositoryModule.cs` (base registrations)
- `SMSBACKEND.Infrastructure/DependencyResolutions/ServiceModule.cs` (base registrations)

**Reason:** Core database access layer.

---

### 1.7 Middleware & Error Handling ✅

**Files to KEEP:**
- `SMSBACKEND.Infrastructure/Middleware/ErrorHandlerMiddleware.cs`
- `SMSBACKEND.Infrastructure/DependencyResolutions/Middleware.cs`
- `SMSBACKEND.Presentation/Filters/ValidateModelState.cs` (if exists)

**Reason:** Essential for error handling and request pipeline.

---

### 1.8 Common Utilities & Helpers ✅

**Files to KEEP:**
- `SMSBACKEND.Common/Helpers/` (entire folder)
  - `AppServicesHelper.cs`
  - `AuditTrailHelper.cs`
  - `ChecksHelper.cs`
  - `DateTimeHelper.cs`
  - `EmailGenerateHelper.cs`
  - `JsonSerializerHelper.cs`
  - `SeedTimestamps.cs`
- `SMSBACKEND.Common/Extensions/` (entire folder)
- `SMSBACKEND.Common/Utilities/` (entire folder)
  - `StaticClasses/CacheKeyBuilder.cs`
  - `Enums/Enums.cs` (keep general enums, remove school-specific)
  - `Constants/` (review - keep general, remove school-specific)

**Reason:** Reusable utility functions.

---

### 1.9 Status Management ✅

**Files to KEEP:**
- `SMSBACKEND.Domain/Entities/Status.cs`
- `SMSBACKEND.Domain/Entities/StatusType.cs`

**Reason:** Generic status management is useful in any application.

---

### 1.10 AutoMapper Configuration ✅

**Files to KEEP:**
- `SMSBACKEND.Application/Mappings/ModelMapper.cs` (but remove school-specific mappings)

**Reason:** DTO mapping infrastructure.

---

### 1.11 Dependency Injection Modules ✅

**Files to KEEP:**
- `SMSBACKEND.Infrastructure/DependencyResolutions/ConfigurationModule.cs`
- `SMSBACKEND.Infrastructure/DependencyResolutions/Documentations.cs`
- `SMSBACKEND.Infrastructure/DependencyResolutions/Extensions.cs`
- `SMSBACKEND.Infrastructure/DependencyResolutions/InfrastructureModule.cs`
- `SMSBACKEND.Infrastructure/DependencyResolutions/Swagger.cs`
- `SMSBACKEND.Infrastructure/DependencyResolutions/Utilities.cs`

**Reason:** DI configuration infrastructure.

---

### 1.12 Presentation Layer Infrastructure ✅

**Files to KEEP:**
- `SMSBACKEND.Presentation/Program.cs`
- `SMSBACKEND.Presentation/appsettings.json`
- `SMSBACKEND.Presentation/Properties/launchSettings.json`
- `SMSBACKEND.Infrastructure/Filters/AuthResponsesOperationFilter.cs`

**Reason:** Application startup and configuration.

---

## ❌ SECTION 2: REMOVE - School-Specific Code

### 2.1 School Domain Entities ❌

**Files to REMOVE:**
- `SMSBACKEND.Domain/Entities/School/INUSE/` (entire folder - 45 entities)
  - `Admission.cs`
  - `Class.cs`
  - `ClassSection.cs`
  - `ClassTeacher.cs`
  - `Department.cs`
  - `DisableReason.cs`
  - `Enquiry.cs`
  - `EntryTest.cs`
  - `Exam.cs`
  - `ExamMarks.cs`
  - `ExamResult.cs`
  - `ExamSubject.cs`
  - `ExamTimetable.cs`
  - `Expense.cs`
  - `ExpenseHead.cs`
  - `FeeAssignment.cs`
  - `FeeDiscount.cs`
  - `FeeGroup.cs`
  - `FeeGroupFeeType.cs`
  - `FeeTransaction.cs`
  - `FeeType.cs`
  - `FineWaiver.cs`
  - `Grade.cs`
  - `GradeScale.cs`
  - `Income.cs`
  - `IncomeHead.cs`
  - `MarksheetTemplate.cs`
  - `PreAdmission.cs`
  - `SchoolHouse.cs`
  - `Section.cs`
  - `Session.cs`
  - `Staff.cs`
  - `Student.cs`
  - `StudentAddress.cs`
  - `StudentAssignment.cs`
  - `StudentAttendance.cs`
  - `StudentCategory.cs`
  - `StudentFinancial.cs`
  - `StudentInfo.cs`
  - `StudentMiscellaneous.cs`
  - `StudentParent.cs`
  - `Subject.cs`
  - `SubjectClass.cs`
  - `TeacherSubjects.cs`
  - `Timetable.cs`

**Note:** Also check `SMSBACKEND.Domain/Entities/School/Pending/` folder for additional entities.

---

### 2.2 School-Specific Repositories ❌

**Files to REMOVE:**
- `SMSBACKEND.Domain/RepositoriesContracts/IRepository/`
  - `IAdmissionRepository.cs`
  - `IClassRepository.cs`
  - `IDepartmentRepository.cs`
  - `IDisableReasonRepository.cs`
  - `IEnquiryRepository.cs`
  - `IEntryTestRepository.cs`
  - `IExamRepository.cs`
  - `IExamMarksRepository.cs`
  - `IExamResultRepository.cs`
  - `IExamSubjectRepository.cs`
  - `IExamTimetableRepository.cs`
  - `IExpenseRepository.cs`
  - `IExpenseHeadRepository.cs`
  - `IFeeAssignmentRepository.cs`
  - `IFeeDiscountRepository.cs`
  - `IFeeGroupRepository.cs`
  - `IFeeGroupFeeTypeRepository.cs`
  - `IFeeTransactionRepository.cs`
  - `IFeeTypeRepository.cs`
  - `IFineWaiverRepository.cs`
  - `IGradeRepository.cs`
  - `IGradeScaleRepository.cs`
  - `IIncomeRepository.cs`
  - `IIncomeHeadRepository.cs`
  - `IMarksheetTemplateRepository.cs`
  - `IPreAdmissionRepository.cs`
  - `ISchoolHouseRepository.cs`
  - `ISectionRepository.cs`
  - `ISessionRepository.cs`
  - `IStaffRepository.cs`
  - `IStudentRepository.cs`
  - `IStudentAttendanceRepository.cs`
  - `IStudentCategoryRepository.cs`
  - `ISubjectRepository.cs`
  - `ITimetableRepository.cs`

**Implementation Files to REMOVE:**
- `SMSBACKEND.Infrastructure/Repository/` (all school-specific repositories)
  - Keep only: `BaseRepository.cs`, `UnitOfWork.cs`

---

### 2.3 School-Specific Services ❌

**Files to REMOVE:**
- `SMSBACKEND.Application/ServiceContracts/IServices/`
  - `IAdmissionService.cs`
  - `IClassService.cs`
  - `IDepartmentService.cs`
  - `IDisableReasonService.cs`
  - `IEnquiryService.cs`
  - `IEntryTestService.cs`
  - `IExamService.cs`
  - `IExamMarksService.cs`
  - `IExamResultService.cs`
  - `IExamSubjectService.cs`
  - `IExamTimetableService.cs`
  - `IExpenseService.cs`
  - `IExpenseHeadService.cs`
  - `IFeeAssignmentService.cs`
  - `IFeeDiscountService.cs`
  - `IFeeGroupService.cs`
  - `IFeeGroupFeeTypeService.cs`
  - `IFeePaymentService.cs`
  - `IFeeTransactionService.cs`
  - `IFeeTypeService.cs`
  - `IFineWaiverService.cs`
  - `IGradeService.cs`
  - `IGradeScaleService.cs`
  - `IIncomeService.cs`
  - `IIncomeHeadService.cs`
  - `IMarksheetTemplateService.cs`
  - `IPreAdmissionService.cs`
  - `ISchoolHouseService.cs`
  - `ISectionService.cs`
  - `ISessionService.cs`
  - `IStaffService.cs`
  - `IStudentService.cs`
  - `IStudentAttendanceService.cs`
  - `IStudentCategoryService.cs`
  - `ISubjectService.cs`
  - `ITeacherService.cs`
  - `ITimetableService.cs`
  - `IDashboardService.cs` (if school-specific)
  - `IPaymentAnalyticsService.cs`

**Implementation Files to REMOVE:**
- `SMSBACKEND.Infrastructure/Services/Services/` (all school-specific services)
  - Keep only: `BaseService.cs`, `NotificationService.cs`, `TenantService.cs`

---

### 2.4 School-Specific Controllers ❌

**Files to REMOVE:**
- `SMSBACKEND.Presentation/Controllers/`
  - `AcademicsController.cs`
  - `AdmissionController.cs`
  - `DashboardController.cs` (if school-specific)
  - `EnquiryController.cs`
  - `EntryTestController.cs`
  - `ExamController.cs`
  - `ExamMarksController.cs`
  - `ExamResultController.cs`
  - `FeesController.cs`
  - `FineWaiverController.cs`
  - `PaymentAnalyticsController.cs`
  - `PreAdmissionController.cs`
  - `StaffController.cs`
  - `StudentAttendanceController.cs`
  - `StudentController.cs`
  - `TeacherController.cs`

**Keep:**
- `BaseController.cs`
- `AccountController.cs`
- `AdminController.cs`
- `NotificationController.cs`

---

### 2.5 School-Specific DTOs ❌

**Files to REMOVE:**
- `SMSBACKEND.Common/DTO/Request/` (all school-specific request DTOs)
- `SMSBACKEND.Common/DTO/Response/` (all school-specific response DTOs)

**Keep:**
- `BaseModel.cs`
- `BaseRequest.cs` (if exists)
- General DTOs (User, Role, Permission, Tenant, Notification, etc.)

---

### 2.6 School-Specific Seed Data ❌

**Files to REMOVE:**
- `SMSBACKEND.Infrastructure/Database/DataSeeder.cs` (remove school-specific seed data)
- `SMSBACKEND.Infrastructure/Database/SeedData.cs` (remove school-specific seed data)
- `SMSBACKEND.Infrastructure/Migrations/` (review - may need to keep initial migration structure)

**Action:** Keep seed data structure but remove school-specific entities.

---

### 2.7 School-Specific Constants & Enums ❌

**Files to REVIEW:**
- `SMSBACKEND.Common/Utilities/Constants/StudentAuthorizationConstants.cs` ❌ REMOVE
- `SMSBACKEND.Common/Utilities/Enums/Enums.cs` ⚠️ REVIEW - Remove school-specific enums

**Action:** Keep general enums (Gender, NotificationTypes, StatusTypes, etc.), remove school-specific ones.

---

### 2.8 School-Specific AutoMapper Mappings ❌

**Action:** In `ModelMapper.cs`, remove all mappings for school entities, keep only:
- User, Role, Permission, Tenant, Notification mappings

---

### 2.9 School-Specific Dependency Registrations ❌

**Files to MODIFY:**
- `SMSBACKEND.Infrastructure/DependencyResolutions/RepositoryModule.cs`
  - Remove all school-specific repository registrations
- `SMSBACKEND.Infrastructure/DependencyResolutions/ServiceModule.cs`
  - Remove all school-specific service registrations

---

### 2.10 School-Specific DbContext Configurations ❌

**Files to MODIFY:**
- `SMSBACKEND.Infrastructure/Database/SqlServerDbContext.cs`
  - Remove all `DbSet<>` for school entities
  - Remove entity configurations for school entities
  - Keep: User, Role, Tenant, Notification, Permission, Module, Status, RefreshToken, UserSession

---

## ⚠️ SECTION 3: REVIEW - May Need Modification

### 3.1 Tenant Entity ⚠️

**File:** `SMSBACKEND.Domain/Entities/Tenant.cs`

**Current Properties (School-Specific):**
- `SchoolName` → Rename to `OrganizationName` or `Name`
- `InstitutionType` → Keep or make generic
- `EducationLevel` → Remove (school-specific)
- `AcademicYearStartMonth` → Remove (school-specific)
- `AcademicYearEndMonth` → Remove (school-specific)
- `LastStudentSequence` → Remove (school-specific)
- `AdmissionNoFormat` → Remove (school-specific)
- `AllowParentIdInCnicField` → Remove (school-specific)
- `MaxStudents` → Rename to `MaxUsers` or `MaxEntities`
- `CurrentStudentCount` → Rename to `CurrentUserCount` or remove
- `MaxStaff` → Remove or make generic

**Action:** Generalize tenant properties to be application-agnostic.

---

### 3.2 ApplicationUser Entity ⚠️

**File:** `SMSBACKEND.Domain/Entities/IdentityModel.cs`

**Current Properties (School-Specific):**
- `StudentId` → Remove (school-specific)
- `StaffId` → Remove (school-specific)
- `Student` navigation → Remove
- `Staff` navigation → Remove

**Action:** Keep user entity but remove school-specific relationships.

---

### 3.3 BaseEntity ⚠️

**File:** `SMSBACKEND.Domain/Entities/BaseEntity.cs`

**Current Properties:**
- `SyncStatus` → Review if needed for general use
- `TenantId` → ✅ Keep (multi-tenancy)

**Action:** Review if `SyncStatus` is needed or remove it.

---

### 3.4 Notification Service ⚠️

**File:** `SMSBACKEND.Infrastructure/Services/Services/NotificationService.cs`

**Current Methods (School-Specific):**
- `SendPaymentConfirmationAsync` → Remove (school-specific)
- `SendPaymentReceiptAsync` → Remove (school-specific)

**Action:** Keep generic notification methods, remove school-specific ones.

---

### 3.5 Dashboard Service ⚠️

**File:** `SMSBACKEND.Application/ServiceContracts/IServices/IDashboardService.cs`

**Action:** Review if dashboard is generic or school-specific. If school-specific, remove.

---

### 3.6 Enums ⚠️

**File:** `SMSBACKEND.Common/Utilities/Enums/Enums.cs`

**Review Each Enum:**
- Keep: `Gender`, `NotificationTypes`, `StatusTypes`, `MenuType`, `ApplicationType`
- Remove: School-specific enums (if any)

---

### 3.7 Constants ⚠️

**Files:** `SMSBACKEND.Common/Utilities/Constants/`

**Action:** Review all constants, keep generic ones, remove school-specific.

---

### 3.8 Migrations ⚠️

**Files:** `SMSBACKEND.Infrastructure/Migrations/`

**Action:** 
- Option 1: Delete all migrations and create fresh initial migration
- Option 2: Keep migration structure but remove school entity configurations

**Recommendation:** Option 1 - Clean slate for boilerplate.

---

## 📊 Summary Statistics

### Files to KEEP: ~150-200 files
- Core infrastructure
- Base patterns
- Authentication/Authorization
- Caching
- Notifications
- Multi-tenancy
- Common utilities

### Files to REMOVE: ~200-250 files
- 45+ school entities
- 40+ school repositories
- 40+ school services
- 15+ school controllers
- 200+ school DTOs
- School-specific seed data
- School-specific constants/enums

### Files to REVIEW/MODIFY: ~10-15 files
- Tenant entity (generalize)
- ApplicationUser (remove school links)
- Notification service (remove school methods)
- DbContext (remove school DbSets)
- DI modules (remove school registrations)
- AutoMapper (remove school mappings)
- Enums/Constants (clean up)

---

## 🎯 Conversion Steps (Recommended Order)

1. **Phase 1: Backup & Preparation**
   - Create a new branch
   - Document current state

2. **Phase 2: Remove School Entities**
   - Delete all entities in `School/INUSE/`
   - Update `BaseEntity` if needed

3. **Phase 3: Remove School Repositories**
   - Delete repository interfaces
   - Delete repository implementations
   - Update `RepositoryModule.cs`

4. **Phase 4: Remove School Services**
   - Delete service interfaces
   - Delete service implementations
   - Update `ServiceModule.cs`

5. **Phase 5: Remove School Controllers**
   - Delete school-specific controllers
   - Keep base controllers

6. **Phase 6: Remove School DTOs**
   - Delete school-specific request/response DTOs
   - Keep base DTOs

7. **Phase 7: Clean Up DbContext**
   - Remove school `DbSet<>` properties
   - Remove entity configurations

8. **Phase 8: Generalize Tenant & User**
   - Update `Tenant.cs` to be generic
   - Update `ApplicationUser.cs` to remove school links

9. **Phase 9: Clean Up AutoMapper**
   - Remove school entity mappings

10. **Phase 10: Clean Up Enums/Constants**
    - Remove school-specific enums
    - Remove school-specific constants

11. **Phase 11: Update Migrations**
    - Delete existing migrations
    - Create fresh initial migration

12. **Phase 12: Testing**
    - Test authentication
    - Test multi-tenancy
    - Test notifications
    - Test caching
    - Verify base CRUD operations work

---

## ✅ Final Checklist

After conversion, the boilerplate should have:

- ✅ Authentication & Authorization (JWT, Roles, Permissions)
- ✅ Multi-Tenancy Support
- ✅ Caching Infrastructure
- ✅ Notification System (Email/SMS)
- ✅ Base Repository Pattern
- ✅ Base Service Pattern
- ✅ Base Controller Pattern
- ✅ Unit of Work Pattern
- ✅ Error Handling Middleware
- ✅ Audit Trail (via BaseEntity)
- ✅ Soft Delete Support
- ✅ AutoMapper Configuration
- ✅ Dependency Injection Setup
- ✅ Swagger/API Documentation
- ✅ Database Migrations Support
- ✅ Common Utilities & Helpers
- ✅ Generic Status Management

---

## 📝 Notes

1. **Tenant Entity**: Consider renaming to `Organization` or keeping as `Tenant` but making properties generic.

2. **BaseEntity**: The `SyncStatus` field might be useful for offline-first apps, but consider making it optional or removing if not needed.

3. **CodeGenerator**: Review if the code generator is useful for the boilerplate or if it's school-specific.

4. **Documentation**: Update all documentation to reflect the boilerplate nature.

5. **README**: Create a new README explaining how to use the boilerplate.

---

**Document Version:** 1.0  
**Created:** 2025-01-XX  
**Status:** 📋 Planning Phase - Awaiting Review
