using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using static Common.Utilities.Enums;
using StatusType = Domain.Entities.StatusType;
using Common.Utilities.Constants;
using Common.Utilities.StaticClasses;
using System;

namespace Infrastructure.Database
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Seed for Tenant (MUST BE FIRST - All other entities reference it)
            modelBuilder.Entity<Tenant>().HasData(
                new Tenant 
                { 
                    TenantId = 1,
                    TenantCode = "SCHOOL001",
                    SchoolName = "ILMSuite School",
                    ShortName = "ISS",
                    ContactEmail = "admin@school.com",
                    ContactPhone = "03001234567",
                    Address = "Default Address",
                    City = "Default City",
                    State = "Default State",
                    Country = "Pakistan",
                    IsActive = true,
                    IsSubscriptionValid = true,
                    SubscriptionTier = "Premium",
                    SubscriptionStartDate = new DateTime(2025, 01, 01),
                    SubscriptionEndDate = new DateTime(2026, 01, 01),
                    MaxStudents = 1000,
                    MaxStaff = 100,
                    CurrentStudentCount = 0,
                    CurrentStaffCount = 0,
                    StorageLimitGB = 10,
                    CurrentStorageUsedGB = 0,
                    Currency = "PKR",
                    CurrencySymbol = "Rs",
                    InstitutionType = "School",
                    EducationLevel = "All",
                    AcademicYearStartMonth = 4,
                    AcademicYearEndMonth = 3,
                    LastStudentSequence = 0, // Initialize admission number sequence
                    AdmissionNoFormat = "{SEQ}", // Default format: simple numeric sequence
                    AllowParentIdInCnicField = true, // Allow ParentId for backward compatibility
                    CreatedAt = new DateTime(2025, 01, 01),
                    UpdatedAt = new DateTime(2025, 01, 01),
                    CreatedBy = "System",
                    UpdatedBy = "System",
                    IsDeleted = false
                }
            );

            // Seed for StatusType
            modelBuilder.Entity<StatusType>().HasData(
                new StatusType { Id = (StatusTypes)(int)StatusTypes.UserStatus, Name = nameof(StatusTypes.UserStatus) }
                );

            // Seed for Status
            modelBuilder.Entity<Status>().HasData(
                    // User statuses
                    new Status() { Id = 1, Name = nameof(UserStatus.Preactive), Code = UserStatusCode.Preactive, TypeId = StatusTypes.UserStatus },
                    new Status() { Id = 2, Name = nameof(UserStatus.Active), Code = UserStatusCode.Active, TypeId = StatusTypes.UserStatus },
                    new Status() { Id = 3, Name = nameof(UserStatus.Inactive), Code = UserStatusCode.Inactive, TypeId = StatusTypes.UserStatus },
                    new Status() { Id = 4, Name = nameof(UserStatus.Canceled), Code = UserStatusCode.Canceled, TypeId = StatusTypes.UserStatus },
                    new Status() { Id = 5, Name = nameof(UserStatus.Frozen), Code = UserStatusCode.Frozen, TypeId = StatusTypes.UserStatus },
                    new Status() { Id = 6, Name = nameof(UserStatus.Blocked), Code = UserStatusCode.Blocked, TypeId = StatusTypes.UserStatus }

                    // Notification statuses
                    );

            // Seed for NotificationType
            modelBuilder.Entity<NotificationType>().HasData(
                new NotificationType() { Id = NotificationTypes.Email, Name = NotificationName.Email },
                new NotificationType() { Id = NotificationTypes.Sms, Name = NotificationName.Sms },
                new NotificationType() { Id = NotificationTypes.Site, Name = NotificationName.Site },
                new NotificationType() { Id = NotificationTypes.Push, Name = NotificationName.Push }
            );

            // Seed for NotificationTemplate
            modelBuilder.Entity<NotificationTemplate>().HasData(
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.EmailUserRegisteration,
                     NotificationTypeId = NotificationTypes.Email,
                     Description = "Email confirmation when user is registered.",
                     Subject = "ItSolution Account Activation",
                     MessageBody = "Hi #Name </br></br>"
                                        + "Thank you for registering in ItSolution. "
                                        + "Click <a href=\"#Link\">here</a> to activate your account."
                     // Todo
                     //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                     //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tu Actividad en dale! para ver los detalles de la transacción. ")
                     //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.SmsUserRegisteration,
                     NotificationTypeId = NotificationTypes.Sms,
                     Description = "Confirmation sms when user is registered.",
                     Subject = string.Empty,
                     MessageBody = "Todo"
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.EmailForgotPassword,
                     NotificationTypeId = NotificationTypes.Email,
                     Description = "Email, when user click on forget password.",
                     Subject = "ItSolution Reset Password",
                     MessageBody = "Hi #Name </br></br>"
                                 + "Please click <a href=\"#Link\">here</a> to reset your password."
                     //Todo
                     //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                     //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tuActividaen dale! para ver los detalles de la transacción. ")
                     //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.SmsForgotPassword,
                     NotificationTypeId = NotificationTypes.Sms,
                     Description = "Code sent in sms, for forget password.",
                     Subject = string.Empty,
                     MessageBody = "Todo"
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.EmailSetPassword,
                     NotificationTypeId = NotificationTypes.Email,
                     Description = "Confirmation email when an external user set password.",
                     Subject = "ItSolution Account Activation",
                     MessageBody = "Hi #Name </br></br>"
                                 + "Thank you for adding a local account in ItSolution. "
                                 + "Click <a href=\"#Link\">here</a> to activate your account"
                     // Todo
                     //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                     //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tuActividaen dale! para ver los detalles de la transacción. ")
                     //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.SmsSetPassword,
                     NotificationTypeId = NotificationTypes.Sms,
                     Description = "Confirmation sms when an external user set password.",
                     Subject = string.Empty,
                     MessageBody = "Todo"
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.EmailChangePassword,
                     NotificationTypeId = NotificationTypes.Email,
                     Description = "Confirmation email when user change the password.",
                     Subject = "ItSolution Email Change",
                     MessageBody = "Hi #Name </br></br>"
                                 + "Click <a href=\"#Link\">here</a> to confirm your email to change it."
                                 + "</br></br> Thank you so much."
                     //Todo:
                     //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                     //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tuActividaen dale! para ver los detalles de la transacción. ")
                     //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.SmsChangePassword,
                     NotificationTypeId = NotificationTypes.Sms,
                     Description = "Confirmation sms when user change the password.",
                     Subject = string.Empty,
                     MessageBody = "Todo"
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.EmailTwoFactorToken,
                     NotificationTypeId = NotificationTypes.Email,
                     Description = "Code in email for tow factor authentication.",
                     Subject = "ItSolution Code",
                     MessageBody = "Hi #Name </br></br>"
                                 + "#Token is your code."
                                 + "</br></br> Thank you so much."
                     //Todo:
                     //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                     //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tuActividaen dale! para ver los detalles de la transacción. ")
                     //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.SmsTwoFactorToken,
                     NotificationTypeId = NotificationTypes.Sms,
                     Description = "Code in sms for tow factor authentication.",
                     Subject = string.Empty,
                     MessageBody = "Todo"
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.EmailUserStatusChange,
                     NotificationTypeId = NotificationTypes.Email,
                     Description = "Email to user whan user is block or status change to any status in application.",
                     Subject = "ItSolution Account Restricted",
                     MessageBody = "Todo"
                     //MessageBody = Resources.email_template.Replace("#Greeting", "Hola #Name")
                     //                                      .Replace("#Content", "¿Cómo estás? #Sender te hizo una transferencia. Revisa tuActividaen dale! para ver los detalles de la transacción. ")
                     //                                      .Replace("#Closing", $"Nos vemos en {Resources.system_name}.")
                 },
                 new NotificationTemplate()
                 {
                     Id = NotificationTemplates.SmsUserStatusChange,
                     NotificationTypeId = NotificationTypes.Email,
                     Description = "Sms to user whan user is block or status change to any status in application.",
                     Subject = string.Empty,
                     MessageBody = "Todo"
                 }
            );

            // Convert Enum values to ApplicationRole seed data
            var roles = Enum.GetValues(typeof(UserRoles))
                                .Cast<UserRoles>().Select(role => new ApplicationRole
                                {  //Id = Guid.NewGuid().ToString(), 
                                    Id = ((int)role).ToString(),
                                    Name = role.ToString(),
                                    IsSystem = role.ToString() == UserRoles.Parent.ToString() ||
                                               role.ToString() == UserRoles.Student.ToString() ||
                                               role.ToString() == UserRoles.User.ToString() ||
                                               role.ToString() == UserRoles.SuperAdmin.ToString() ? false : true,
                                    NormalizedName = role.ToString().ToUpper(),
                                    ConcurrencyStamp = Guid.Empty.ToString() // Set a fixed ConcurrencyStamp
                                }).ToArray();
            modelBuilder.Entity<ApplicationRole>().HasData(roles);

            modelBuilder.Entity<Section>().HasData(
                new Section() { Id = 1, Name = "A", TenantId = 1, CreatedAt = new DateTime(2025, 01, 01), CreatedAtUtc = new DateTime(2025, 01, 01), UpdatedAt = new DateTime(2025, 01, 01), CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Section() { Id = 2, Name = "B", TenantId = 1, CreatedAt = new DateTime(2025, 01, 01), CreatedAtUtc = new DateTime(2025, 01, 01), UpdatedAt = new DateTime(2025, 01, 01), CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Section() { Id = 3, Name = "C", TenantId = 1, CreatedAt = new DateTime(2025, 01, 01), CreatedAtUtc = new DateTime(2025, 01, 01), UpdatedAt = new DateTime(2025, 01, 01), CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
                );
            var seedDate = new DateTime(2025, 01, 01);
            modelBuilder.Entity<Class>().HasData(
                new Class() { Id = 1, Name = "Class 1", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 2, Name = "Class 2", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 3, Name = "Class 3", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 4, Name = "Class 4", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 5, Name = "Class 5", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 6, Name = "Class 6", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 7, Name = "Class 7", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 8, Name = "Class 8", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 9, Name = "Class 9", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 10, Name = "Class 10", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 11, Name = "Play Group", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 12, Name = "Nursery", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Class() { Id = 13, Name = "Kinder Garten", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
                );
            modelBuilder.Entity<ClassSection>().HasData(
                new ClassSection() { Id = 1, ClassId = 1, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 2, ClassId = 1, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 3, ClassId = 2, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 4, ClassId = 2, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 5, ClassId = 3, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 6, ClassId = 3, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 7, ClassId = 4, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 8, ClassId = 4, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 9, ClassId = 5, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 10, ClassId = 5, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 11, ClassId = 6, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 12, ClassId = 6, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 13, ClassId = 7, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 14, ClassId = 7, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 15, ClassId = 8, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 16, ClassId = 8, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 17, ClassId = 9, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 18, ClassId = 9, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 19, ClassId = 10, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 20, ClassId = 10, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 21, ClassId = 11, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 22, ClassId = 11, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 23, ClassId = 12, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 24, ClassId = 12, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 25, ClassId = 13, SectionId = 1, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new ClassSection() { Id = 26, ClassId = 13, SectionId = 2, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
            );
            modelBuilder.Entity<Subject>().HasData(
                new Subject() { Id = 1, Name = "English", Code = "001", Type = SubjectTypes.Theory, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Subject() { Id = 2, Name = "Math", Code = "002", Type = SubjectTypes.Theory, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Subject() { Id = 3, Name = "Physics", Code = "003", Type = SubjectTypes.Theory, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Subject() { Id = 4, Name = "Science", Code = "004", Type = SubjectTypes.Theory, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Subject() { Id = 5, Name = "Urdu", Code = "005", Type = SubjectTypes.Theory, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Subject() { Id = 6, Name = "Social Studies", Code = "006", Type = SubjectTypes.Theory, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Subject() { Id = 7, Name = "Computer", Code = "007", Type = SubjectTypes.Theory, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Subject() { Id = 8, Name = "Physics Practical", Code = "003P", Type = SubjectTypes.Practical, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Subject() { Id = 9, Name = "Computer Practical", Code = "007P", Type = SubjectTypes.Practical, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
                );
            modelBuilder.Entity<StudentCategory>().HasData(
                new StudentCategory() { Id = 1, Name = "General", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new StudentCategory() { Id = 2, Name = "Special", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new StudentCategory() { Id = 3, Name = "Physically Challenged", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
                );
            modelBuilder.Entity<SchoolHouse>().HasData(
               new SchoolHouse() { Id = 1, Name = "Red", Color = "#FF0000", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new SchoolHouse() { Id = 2, Name = "Yellow", Color = "#FFFF00", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new SchoolHouse() { Id = 3, Name = "Green", Color = "#00FF00", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new SchoolHouse() { Id = 4, Name = "Blue", Color = "#0000FF", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
               );
            modelBuilder.Entity<Department>().HasData(
               new Department() { Id = 1, Name = "Admin", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new Department() { Id = 2, Name = "Academic", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new Department() { Id = 3, Name = "Finance", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new Department() { Id = 4, Name = "Exam", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new Department() { Id = 5, Name = "Sports", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new Department() { Id = 6, Name = "Library", TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
               );
            modelBuilder.Entity<FeeGroup>().HasData(
                new FeeGroup() { Id = 1, Name = "System", IsSystem = true, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 2, Name = "Tuition Fee", IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = "System", IsDeleted = true, IsActive = false, SyncStatus = "synced" },
                new FeeGroup() { Id = 3, Name = "Admission", IsSystem = true, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 4, Name = "Registration", IsSystem = true, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 5, Name = "Library", IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 6, Name = "Lab", IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 7, Name = "Annual", IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 8, Name = "Exam", IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 9, Name = "Sports", IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 10, Name = "I Installment", IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeGroup() { Id = 11, Name = "II Installment", IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
            );
            modelBuilder.Entity<FeeDiscount>().HasData(
               new FeeDiscount() { Id = 1, Name = "Sibling Discount", Code = "sibling-disc", DiscountType = DiscountType.Percentage, DiscountPercentage = 5, DiscountAmount = 0, ExpiryDate = new DateTime(2025, 12, 31), NoOfUseCount = 10, IsSystem = true, IsRecurring = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new FeeDiscount() { Id = 2, Name = "Early Bird Discount Percentage", Code = "early-bird-per", DiscountType = DiscountType.Percentage, DiscountPercentage = 40, DiscountAmount = 0, ExpiryDate = new DateTime(2025, 12, 31), NoOfUseCount = 10, IsSystem = true, IsRecurring = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new FeeDiscount() { Id = 3, Name = "Early Bird Discount Amount", Code = "early-bird-amount", DiscountType = DiscountType.Fixed, DiscountPercentage = 0, DiscountAmount = 5000, ExpiryDate = new DateTime(2025, 12, 31), NoOfUseCount = 10, IsSystem = true, IsRecurring = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
               new FeeDiscount() { Id = 4, Name = "Class Topper Discount", Code = "cls-top-disc", DiscountType = DiscountType.Percentage, DiscountPercentage = 5, DiscountAmount = 0, ExpiryDate = new DateTime(2025, 12, 31), NoOfUseCount = 10, IsSystem = true, IsRecurring = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
               );
            modelBuilder.Entity<FeeType>().HasData(
                new FeeType() { Id = 1, Name = "Monthly Tuition Fee", Code = "monthly-tuition-fee", FeeFrequency = FeeFrequency.Monthly, IsSystem = true, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 2, Name = "Admission Fees", Code = "admission-fee", FeeFrequency = FeeFrequency.OneTime, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 3, Name = "Library Fee", Code = "library-fee", FeeFrequency = FeeFrequency.Annually, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 4, Name = "Lab Charges", Code = "lab-charges", FeeFrequency = FeeFrequency.Annually, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 5, Name = "Annual Charges", Code = "annual-charges", FeeFrequency = FeeFrequency.Annually, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 6, Name = "Registration Fee", Code = "registration-charges", FeeFrequency = FeeFrequency.OneTime, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 7, Name = "Security Fee", Code = "security-fee", FeeFrequency = FeeFrequency.OneTime, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 8, Name = "Stationary Charges", Code = "stationary-charges", FeeFrequency = FeeFrequency.Annually, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 9, Name = "Exam Fee", Code = "exam-fee", FeeFrequency = FeeFrequency.Annually, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 10, Name = "Sports Fees", Code = "sports-fee", FeeFrequency = FeeFrequency.Annually, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 11, Name = "1st Installment Fees", Code = "1-installment-fees", FeeFrequency = FeeFrequency.Quaterly, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 12, Name = "2nd Installment Fees", Code = "2-installment-fees", FeeFrequency = FeeFrequency.Quaterly, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 13, Name = "3rd Installment Fees", Code = "3-installment-fees", FeeFrequency = FeeFrequency.Quaterly, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 14, Name = "4th Installment Fees", Code = "4-installment-fees", FeeFrequency = FeeFrequency.Quaterly, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },

                new FeeType() { Id = 15, Name = "5th Installment Fees", Code = "5-installment-fees", FeeFrequency = FeeFrequency.HalfYearly, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new FeeType() { Id = 16, Name = "6th Installment Fees", Code = "6-installment-fees", FeeFrequency = FeeFrequency.HalfYearly, IsSystem = false, TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" }
            );
            
            modelBuilder.Entity<Session>().HasData(
                new Session() { Id = 1, Sessions = "2020-2021", StartDate = new DateTime(2020, 8, 1), EndDate = new DateTime(2021, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" },
                new Session() { Id = 2, Sessions = "2021-2022", StartDate = new DateTime(2021, 8, 1), EndDate = new DateTime(2022, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" },
                new Session() { Id = 3, Sessions = "2022-2023", StartDate = new DateTime(2022, 8, 1), EndDate = new DateTime(2023, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" },
                new Session() { Id = 4, Sessions = "2023-2024", StartDate = new DateTime(2023, 8, 1), EndDate = new DateTime(2024, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" },
                new Session() { Id = 5, Sessions = "2024-2025", StartDate = new DateTime(2024, 8, 1), EndDate = new DateTime(2025, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" },
                new Session() { Id = 6, Sessions = "2025-2026", StartDate = new DateTime(2025, 8, 1), EndDate = new DateTime(2026, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = true, SyncStatus = "synced" },
                new Session() { Id = 7, Sessions = "2026-2027", StartDate = new DateTime(2026, 8, 1), EndDate = new DateTime(2027, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" },
                new Session() { Id = 8, Sessions = "2027-2028", StartDate = new DateTime(2027, 8, 1), EndDate = new DateTime(2028, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" },
                new Session() { Id = 9, Sessions = "2028-2029", StartDate = new DateTime(2028, 8, 1), EndDate = new DateTime(2029, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" },
                new Session() { Id = 10, Sessions = "2029-2030", StartDate = new DateTime(2029, 8, 1), EndDate = new DateTime(2030, 7, 31), TenantId = 1, CreatedAt = seedDate, CreatedAtUtc = seedDate, UpdatedAt = seedDate, CreatedBy = "System", UpdatedBy = "System", DeletedBy = null, IsDeleted = false, IsActive = false, SyncStatus = "synced" }
            );
            var modules = new List<Module>
            {
                // Main Modules
                new Module { Id = 1, Icon = "fa fa-mortar-board", Name = "Academics", Path = "", Type = MenuType.School, OrderById = 2 },
                new Module { Id = 2, Icon = "fa fa-user-plus", Name = "Student Information", Path = "", Type = MenuType.School, OrderById = 3 },
                new Module { Id = 3, Icon = "fa fa-dashboard", Name = "Dashboard", Path = "/dashboard", Type = MenuType.School, OrderById = 1 },
                new Module { Id = 4, Icon = "fa fa-ioxhost ftlayer", Name = "Front Office", Path = "", Type = MenuType.Admin, OrderById = 4 },
                new Module { Id = 5, Icon = "fa fa-money", Name = "Fees Collection", Path = "", Type = MenuType.Admin, OrderById = 5 },
                new Module { Id = 6, Icon = "fa fa-sitemap", Name = "Human Resource", Path = "", Type = MenuType.Admin, OrderById = 6 },
                new Module { Id = 7, Icon = "fa fa-calendar-check-o", Name = "Attendance", Path = "", Type = MenuType.School, OrderById = 7 },
                new Module { Id = 8, Icon = "fa fa-usd", Name = "Income", Path = "", Type = MenuType.Admin, OrderById = 8 },
                new Module { Id = 9, Icon = "fa fa-credit-card", Name = "Expenses", Path = "", Type = MenuType.Admin, OrderById = 9 },

                new Module { Id = 10, Icon = "fa fa-map-o ftlayer", Name = "Examinations", Path = "Exams", Type = MenuType.School, OrderById = 10 },
                new Module { Id = 11, Icon = "fe fe-settings", Name = "Security", Path = "", Type = MenuType.Admin, OrderById = 11 },
                // SubMenu Modules

                // Academics SubMenu
                new Module { Id = 101, Icon = "fa fa-table", Name = "Sections", Path = "/Academics/Sections", Type = MenuType.SubMenu, ParentId = 1, OrderById = 1 },
                new Module { Id = 102, Icon = "fa fa-book", Name = "Classes", Path = "/Academics/Classes", Type = MenuType.SubMenu, ParentId = 1, OrderById = 2 },
                new Module { Id = 103, Icon = "fa fa-clipboard", Name = "Subjects", Path = "/Academics/Subjects", Type = MenuType.SubMenu, ParentId = 1, OrderById = 3 },
                new Module { Id = 104, Icon = "fa fa-clipboard", Name = "Teacher", Path = "/Academics/Teachers", Type = MenuType.SubMenu, ParentId = 1, OrderById = 4 },
                new Module { Id = 105, Icon = "fa fa-clipboard", Name = "Timetable", Path = "/Academics/Timetable", Type = MenuType.SubMenu, ParentId = 1, OrderById = 5 },

                // Student Information SubMenu
                new Module { Id = 121, Icon = "fa fa-users", Name = "Students", Path = "/StudentsInfo/Students", Type = MenuType.SubMenu, ParentId = 2, OrderById = 1 },
                new Module { Id = 122, Icon = "fa fa-users", Name = "Student Categories", Path = "/StudentsInfo/Category", Type = MenuType.SubMenu, ParentId = 2, OrderById = 2 },
                new Module { Id = 123, Icon = "fa fa-users", Name = "School House", Path = "/StudentsInfo/SchoolHouse", Type = MenuType.SubMenu, ParentId = 2, OrderById = 3 },
                new Module { Id = 124, Icon = "fa fa-users", Name = "Disable Reason", Path = "/StudentsInfo/DisableReason", Type = MenuType.SubMenu, ParentId = 2, OrderById = 4 },

                // Fees Collection SubMenu
                new Module { Id = 141, Icon = "fa fa-calendar-check-o", Name = "Fees Type", Path = "/Fees/FeesType", Type = MenuType.SubMenu, ParentId = 5, OrderById = 1 },
                new Module { Id = 142, Icon = "fa fa-object-group", Name = "Fees Group", Path = "/Fees/FeesGroup", Type = MenuType.SubMenu, ParentId = 5, OrderById = 2 },
                new Module { Id = 143, Icon = "fa fa-object-group", Name = "Fees Master", Path = "/Fees/FeesMaster", Type = MenuType.SubMenu, ParentId = 5, OrderById = 3 },
                new Module { Id = 144, Icon = "fa fa-tag", Name = "Fees Discount", Path = "/Fees/FeesDiscount", Type = MenuType.SubMenu, ParentId = 5, OrderById = 4 },
                new Module { Id = 145, Icon = "fa fa-book", Name = "Fee Assignment", Path = "/Fees/FeeAssignment", Type = MenuType.SubMenu, ParentId = 5, OrderById = 5 },
                new Module { Id = 146, Icon = "fa fa-book", Name = "Fee Transaction", Path = "/Fees/FeeTransaction", Type = MenuType.SubMenu, ParentId = 5, OrderById = 6 },
                new Module { Id = 147, Icon = "fa fa-book", Name = "Fine Waiver", Path = "/Fees/FineWaiver", Type = MenuType.SubMenu, ParentId = 5, OrderById = 7 },
                new Module { Id = 148, Icon = "fa fa-book", Name = "Payment Analytics", Path = "/Fees/PaymentAnalytics", Type = MenuType.SubMenu, ParentId = 5, OrderById = 8 },

                // Academics Human Resource SubMenu
                new Module { Id = 161, Icon = "fa fa-user-circle-o", Name = "Staff", Path = "/HumanResource/Staff", Type = MenuType.SubMenu, ParentId = 6, OrderById = 1 },
                new Module { Id = 162, Icon = "fa fa-building", Name = "Departments", Path = "/HumanResource/Departments", Type = MenuType.SubMenu, ParentId = 6, OrderById = 3 },

                // Attendance SubMenu
                new Module { Id = 181, Icon = "fa fa-calendar-check-o", Name = "Attendance", Path = "/Attendance/StudentAttendance", Type = MenuType.SubMenu, ParentId = 7, OrderById = 1 },
                new Module { Id = 182, Icon = "fa fa-calendar-check-o", Name = "Mark Attendance", Path = "/Attendance/MarkStudentAttendance", Type = MenuType.SubMenu, ParentId = 7, OrderById = 2 },
                new Module { Id = 183, Icon = "fa fa-calendar-check-o", Name = "Attendance Report", Path = "/Attendance/Reports", Type = MenuType.SubMenu, ParentId = 7, OrderById = 3 },
                
                // Income SubMenu
                new Module { Id = 191, Icon = "fa fa-usd", Name = "All Income", Path = "/Income/AllIncome", Type = MenuType.SubMenu, ParentId = 8, OrderById = 1 },
                new Module { Id = 192, Icon = "fa fa-table", Name = "Income Head", Path = "/Income/IncomeHead", Type = MenuType.SubMenu, ParentId = 8, OrderById = 2 },
                
                // Expenses SubMenu
                new Module { Id = 201, Icon = "fa fa-credit-card", Name = "All Expenses", Path = "/Expenses/AllExpense", Type = MenuType.SubMenu, ParentId = 9, OrderById = 1 },
                new Module { Id = 202, Icon = "fa fa-table", Name = "Expense Head", Path = "/Expenses/ExpenseHead", Type = MenuType.SubMenu, ParentId = 9, OrderById = 2 },

                // Security SubMenu
                new Module { Id = 211, Icon = "fa fa-user-circle-o", Name = "Role", Path = "/Security/Roles", Type = MenuType.SubMenu, ParentId = 11, OrderById = 1 },


                // Front Office SubMenu
                new Module { Id = 231, Icon = "fa fa-info-circle", Name = "Front Office Dashboard", Path = "/FrontOffice/Dashboard", Type = MenuType.SubMenu, ParentId = 4, OrderById = 1 },
                new Module { Id = 232, Icon = "fa fa-info-circle", Name = "Enquiry", Path = "/FrontOffice/Enquiry", Type = MenuType.SubMenu, ParentId = 4, OrderById = 2 },
                new Module { Id = 233, Icon = "fa fa-info-circle", Name = "Application", Path = "/FrontOffice/Application", Type = MenuType.SubMenu, ParentId = 4, OrderById = 3 },
                new Module { Id = 234, Icon = "fa fa-info-circle", Name = "Entry Test", Path = "/FrontOffice/EntryTest", Type = MenuType.SubMenu, ParentId = 4, OrderById = 4 },
                new Module { Id = 235, Icon = "fa fa-info-circle", Name = "Admission", Path = "/FrontOffice/Admission", Type = MenuType.SubMenu, ParentId = 4, OrderById = 5 },

                // Examinations SubMenu
                new Module { Id = 251, Icon = "fa fa-info-circle", Name = "Exam Management", Path = "/Exams/Management", Type = MenuType.SubMenu, ParentId = 10, OrderById = 1 },
                new Module { Id = 252, Icon = "fa fa-info-circle", Name = "Exam Marks", Path = "/Exams/Marks", Type = MenuType.SubMenu, ParentId = 10, OrderById = 2 },
                new Module { Id = 253, Icon = "fa fa-info-circle", Name = "Exam Results", Path = "/Exams/Results", Type = MenuType.SubMenu, ParentId = 10, OrderById = 3 },
                new Module { Id = 254, Icon = "fa fa-info-circle", Name = "Grade Configuration", Path = "/Exams/GradeConfiguration", Type = MenuType.SubMenu, ParentId = 10, OrderById = 4 },


                //new Module { Id = 41, Icon = "fa fa-gear", Name = "Settings", Path = "/setting", Type = MenuType.Admin, OrderById = 41 },
                //new Module { Id = 40, Icon = "fa fa-calendar-check-o", Name = "Attendance", Path = "attendance", Type = MenuType.Admin, OrderById = 40 },
                //new Module { Id = 39, Icon = "fa fa-truck", Name = "Transport", Path = "/transports", Type = MenuType.Admin, OrderById = 39 },
                //new Module { Id = 38, Icon = "fa fa-bed", Name = "Hostel", Path = "/hostel", Type = MenuType.Admin, OrderById = 34 },
                //new Module { Id = 37, Icon = "fa fa-list-ul", Name = "Taskboard", Path = "/taskboard", Type = MenuType.Admin, OrderById = 8 },
                //new Module { Id = 24, Icon = "fa fa-credit-card", Name = "Payments", Path = "/payments", Type = MenuType.Admin, OrderById = 24 },
                //new Module { Id = 33, Icon = "fa fa-flag", Name = "Leave", Path = "/leave", Type = MenuType.Admin, OrderById = 33 },


                 // Student Information Feature
                new Module { Id = 501, Icon = "", Name = "Import Student", Path = "", Type = MenuType.Feature, ParentId = 2, OrderById = 1 },
                new Module { Id = 502, Icon = "", Name = "Student Timeline", Path = "", Type = MenuType.Feature, ParentId = 2, OrderById = 2 },
                new Module { Id = 503, Icon = "fa fa-key", Name = "Change Password", Path = "/Security/ChangePassword", Type = MenuType.Feature, ParentId = 11, OrderById = 3 },
                new Module { Id = 504, Icon = "fe fe-user", Name = "Profile", Path = "/Security/Profile", Type = MenuType.Feature, ParentId = 11, OrderById = 4 }
            };
            modelBuilder.Entity<Module>().HasData(modules);

            int permissionId = 1;
            var allPermissions = new List<Permission>();

            foreach (var module in modules.Where(x=> x.Name == "Security" || x.Name == "Role"))
            {
                foreach (var permissionName in AppPermissions.All)
                {
                    allPermissions.Add(new Permission
                    {
                        Id = permissionId++,
                        Name = $"{permissionName}_{module.Name}",
                        Description = $"{permissionName} permission for module {module.Name} and {module.Id}",
                        ModuleId = module.Id
                    });
                }
            }

            modelBuilder.Entity<Permission>().HasData(allPermissions);

            // 3. Seed RolePermissions
            var rolePermissions = allPermissions.Select(p => new RolePermission
            {
                RoleId = ((int)UserRoles.SuperAdmin).ToString(), // Admin RoleId
                PermissionId = p.Id
            }).ToList();

            modelBuilder.Entity<RolePermission>().HasData(rolePermissions);

            //new Module { Id = 13, Icon = "fa fa-graduation-cap", Name = "Courses", Path = "/courses", Type = MenuType.School, OrderById = 13 },
            //new Module { Id = 14, Icon = "fa fa-book", Name = "Library", Path = "/library", Type = MenuType.School, OrderById = 14 },
            //new Module { Id = 15, Icon = "fa fa-bullhorn", Name = "Holiday", Path = "/holiday", Type = MenuType.School, OrderById = 15 },
            //new Module { Id = 16, Icon = "fa fa-calendar", Name = "Calendar", Path = "/events", Type = MenuType.School, OrderById = 16 },
            //new Module { Id = 17, Icon = "fa fa-comments-o", Name = "Chat App", Path = "/chat", Type = MenuType.School, OrderById = 17 },
            //new Module { Id = 18, Icon = "fa fa-address-book", Name = "Contact", Path = "/contact", Type = MenuType.School, OrderById = 18 },
            //new Module { Id = 19, Icon = "fa fa-folder", Name = "File Manager", Path = "/fileManager", Type = MenuType.School, OrderById = 19 },
            //new Module { Id = 20, Icon = "fa fa-map", Name = "Our Centres", Path = "/ourCentre", Type = MenuType.School, OrderById = 20 },
            //new Module { Id = 21, Icon = "fa fa-camera-retro", Name = "Gallery", Path = "/gallery", Type = MenuType.School, OrderById = 21 }




            //new Module { Id = 22, Icon = "fa fa-flask", Name = "Science", Path = "/courses/science", Type = MenuType.SubMenu, OrderById = 22, ParentId = 1 },
            //new Module { Id = 23, Icon = "fa fa-calculator", Name = "Mathematics", Path = "/courses/math", Type = MenuType.SubMenu, OrderById = 23, ParentId = 1 }


            //new Module { Id = 22, Icon = "fa fa-nav-link icon", Name = "Search...", Path = "pageSearch",Type=MenuType.LeftMenu, LinkclassName = "fe fe-search" },
            //new Module { Id = 23, Icon = "fa fa-nav-link icon app_inbox", Name = "Inbox", Path = "email",Type=MenuType.LeftMenu, LinkclassName = "fe fe-inbox" },
            //new Module { Id = 24, Icon = "fa fa-nav-link icon app_file xs-hide", Name = "File Manager", Path="filemanager",Type = MenuType.LeftMenu, LinkclassName = "fe fe-folder" },
            //new Module { Id = 25, Icon = "fa fa-nav-link icon xs-hide", Name = "Social Media", Path = "socialMedia",Type=MenuType.LeftMenu, LinkclassName = "fe fe-share-2" },
            //new Module { Id = 26, Icon = "fa fa-nav-link icon settingbar", Name = "Settings", Path = "Settings",Type=MenuType.LeftMenu, LinkclassName = "fe fe-settings" },
            //new Module { Id = 27, Icon = "fa fa-nav-link icon right_tab", Name = "Right Tab", Path = "righttab",Type=MenuType.LeftMenu, LinkclassName = "fe fe-align-right" },
            //new Module { Id = 28, Icon = "fa fa-nav-link icon settingbar", Name = "power", Path = "login", Type = MenuType.LeftMenu, LinkclassName = "fe fe-power" }

        }

        //public static void SeedingData(WebApplication app)
        //{
        //    using (var scope = app.Services.CreateScope())
        //    {
        //        var services = scope.ServiceProvider;
        //        try
        //        {
        //            var context = services.GetRequiredService<SqlServerDbContext>();
        //            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        //            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        //            var functional = services.GetRequiredService<IFunctional>();

        //            DbInitializer.Initialize(context, functional).Wait();
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionLogging.LogError(ex);
        //            var logger = services.GetRequiredService<ILogger<Program>>();
        //            logger.LogError(ex, "An error occurred while seeding the database.");
        //        }
        //    }
        //}
    }
}
