using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Common.Utilities;
using Common.Options;
using static Common.Utilities.Enums;
using StatusType = Domain.Entities.StatusType;
using Infrastructure.Services.Services;
using Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Infrastructure.DependencyResolutions;
using Application.ServiceContracts;

namespace Infrastructure.Database
{
    public static class DataSeeder
    {
        public static async Task Seed(this IApplicationBuilder app)
        {
            try
            {
                using (var scope = app?.ApplicationServices?.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope?.ServiceProvider.GetRequiredService<SqlServerDbContext>();

                    #region  Users
                    var status = context?.Statuses.FirstOrDefault(s => s.Name.Equals(nameof(UserStatus.Active)));
                    if (status == null)
                    {
                        throw new Exception($"{nameof(status)} is not found, seeds are malfunctioned.");
                    }

                    var _userManager = scope?.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = new ApplicationUser
                    {
                        Email = "shazzahmed27@gmail.com",
                        FirstName = "Shazz",
                        LastName = "Ahmed",
                        UserName = "shazzahmed27@gmail.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "shazzahmed27@gmail.com",
                        NormalizedUserName = "shazzahmed27@gmail.com",
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        PhoneNumber = "03002291364",
                        PhoneNumberConfirmed = true,
                        CNIC = "4220178884487",
                        StatusId = status.Id
                    };
                    if (!context.Users.Any(u => u.Email == user.Email))
                    {

                        var result = _userManager?.CreateAsync(user, "SuperAdmin1@345").Result;
                        try
                        {
                            var ro = _userManager?.AddToRoleAsync(user, nameof(UserRoles.SuperAdmin)).Result;
                            context.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }

                    var superAdmin = new ApplicationUser
                    {
                        Email = "superadmin@gmail.com",
                        FirstName = "Shazz",
                        LastName = "Ahmed",
                        UserName = "superadmin@gmail.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "superadmin@gmail.com",
                        NormalizedUserName = "superadmin@gmail.com",
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        PhoneNumber = "03002291364",
                        PhoneNumberConfirmed = true,
                        StatusId = status.Id
                    };
                    if (!context.Users.Any(u => u.Email == superAdmin.Email))
                    {
                        var result = _userManager?.CreateAsync(superAdmin, "SuperAdmin1@345").Result;

                        var ro = _userManager?.AddToRoleAsync(superAdmin, nameof(UserRoles.SuperAdmin)).Result;
                        context.SaveChanges();
                    }

                    var admin = new ApplicationUser
                    {
                        Email = "admin@test.com",
                        UserName = "admin@test.com",
                        FirstName = "Admin",
                        LastName = "",
                        EmailConfirmed = true,
                        NormalizedEmail = "admin@test.com",
                        NormalizedUserName = "admin@test.com",
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                        PhoneNumber = "03306888661",
                        PhoneNumberConfirmed = true,
                        StatusId = status.Id
                    };
                    if (!context.Users.Any(u => u.Email == admin.Email))
                    {
                        var result = _userManager?.CreateAsync(admin, "Admin1@123").Result;

                        var ro = _userManager?.AddToRoleAsync(admin, nameof(UserRoles.Admin)).Result;
                        context.SaveChanges();
                    }

                    #endregion Users
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static void SeedingData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<SqlServerDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                }
                catch (Exception ex)
                {
                    //var logger = services.GetRequiredService<ILogger<RepositoryModule>>();
                    //logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}
