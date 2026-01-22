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
                    TenantCode = "TENANT001",
                    Name = "Default Organization",
                    ShortName = "DO",
                    ContactEmail = "admin@organization.com",
                    ContactPhone = "03001234567",
                    Address = "Default Address",
                    City = "Default City",
                    State = "Default State",
                    Country = "Default Country",
                    IsActive = true,
                    IsSubscriptionValid = true,
                    SubscriptionTier = "Premium",
                    SubscriptionStartDate = new DateTime(2025, 01, 01),
                    SubscriptionEndDate = new DateTime(2026, 01, 01),
                    Currency = "USD",
                    CurrencySymbol = "$",
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
                     Subject = "MultiServe Account Activation",
                     MessageBody = "Hi #Name </br></br>"
                                        + "Thank you for registering in MultiServe. "
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
                     Subject = "MultiServe Reset Password",
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
                     Subject = "MultiServe Account Activation",
                     MessageBody = "Hi #Name </br></br>"
                                 + "Thank you for adding a local account in MultiServe. "
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
                     Subject = "MultiServe Email Change",
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
                     Subject = "MultiServe Code",
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
                     Subject = "MultiServe Account Restricted",
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
                                    IsSystem = 
                                               role.ToString() == UserRoles.User.ToString() ||
                                               role.ToString() == UserRoles.SuperAdmin.ToString() ? false : true,
                                    NormalizedName = role.ToString().ToUpper(),
                                    ConcurrencyStamp = Guid.Empty.ToString() // Set a fixed ConcurrencyStamp
                                }).ToArray();
            modelBuilder.Entity<ApplicationRole>().HasData(roles);

            // General Purpose Modules
            var modules = new List<Module>
            {
                // Main Modules
                new Module { Id = 1, Icon = "fa fa-dashboard", Name = "Dashboard", Path = "/dashboard", Type = MenuType.Admin, OrderById = 1 },
                new Module { Id = 2, Icon = "fe fe-settings", Name = "Security", Path = "", Type = MenuType.Admin, OrderById = 2 },

                // Security SubMenu
                new Module { Id = 101, Icon = "fa fa-user-circle-o", Name = "Role", Path = "/Security/Roles", Type = MenuType.SubMenu, ParentId = 2, OrderById = 1 },
                
                // Security Features
                new Module { Id = 201, Icon = "fa fa-key", Name = "Change Password", Path = "/Security/ChangePassword", Type = MenuType.Feature, ParentId = 2, OrderById = 1 },
                new Module { Id = 202, Icon = "fe fe-user", Name = "Profile", Path = "/Security/Profile", Type = MenuType.Feature, ParentId = 2, OrderById = 2 }
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
