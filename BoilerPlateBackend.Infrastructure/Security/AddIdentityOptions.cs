using Common.Options;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Security
{
    public static class AddIdentityOptions
    {
        public static void SetOptions(IServiceCollection services, IOptionsSnapshot<DefaultIdentityOptionsModel> _DefaultIdentityOptions)
        {
            try
            {
                services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = _DefaultIdentityOptions.Value.PasswordRequireDigit;
                    options.Password.RequiredLength = _DefaultIdentityOptions.Value.PasswordRequiredLength;
                    options.Password.RequireNonAlphanumeric = _DefaultIdentityOptions.Value.PasswordRequireNonAlphanumeric;
                    options.Password.RequireUppercase = _DefaultIdentityOptions.Value.PasswordRequireUppercase;
                    options.Password.RequireLowercase = _DefaultIdentityOptions.Value.PasswordRequireLowercase;
                    options.Password.RequiredUniqueChars = _DefaultIdentityOptions.Value.PasswordRequiredUniqueChars;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(_DefaultIdentityOptions.Value.LockoutDefaultLockoutTimeSpanInMinutes);
                    options.Lockout.MaxFailedAccessAttempts = _DefaultIdentityOptions.Value.LockoutMaxFailedAccessAttempts;
                    options.Lockout.AllowedForNewUsers = _DefaultIdentityOptions.Value.LockoutAllowedForNewUsers;

                    options.User.RequireUniqueEmail = _DefaultIdentityOptions.Value.UserRequireUniqueEmail;

                    options.SignIn.RequireConfirmedEmail = _DefaultIdentityOptions.Value.SignInRequireConfirmedEmail;
                }).AddEntityFrameworkStores<SqlServerDbContext>()
            .AddDefaultTokenProviders();
                services.ConfigureApplicationCookie(config =>
                {
                    //config.Cookie.Name = "Identity.Cookie";
                    //config.LoginPath = "/Account/Login";
                });
                services.Configure<CookieAuthenticationOptions>(options =>
                {
                    options.Cookie.HttpOnly = _DefaultIdentityOptions.Value.CookieHttpOnly;
                    options.Cookie.Expiration = TimeSpan.FromDays(_DefaultIdentityOptions.Value.CookieExpiration);
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(_DefaultIdentityOptions.Value.CookieExpireTimeSpan);
                    options.LoginPath = _DefaultIdentityOptions.Value.LoginPath;
                    options.LogoutPath = _DefaultIdentityOptions.Value.LogoutPath;
                    options.AccessDeniedPath = _DefaultIdentityOptions.Value.AccessDeniedPath;
                    options.SlidingExpiration = _DefaultIdentityOptions.Value.SlidingExpiration;
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
