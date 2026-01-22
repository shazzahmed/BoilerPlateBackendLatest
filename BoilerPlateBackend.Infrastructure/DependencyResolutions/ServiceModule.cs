using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Infrastructure.Services;
using Infrastructure.Services.Services;

namespace Infrastructure.ServicesDependencyResolutions
{
    public static class ServiceModule
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Multi-Tenancy Services
            services.AddScoped<ITenantService, TenantService>();
            
            // SignalR for Real-time Updates
            services.AddSignalR(options =>
            {
                // ✅ SIGNALR: Enable detailed errors for debugging (disable in production)
                options.EnableDetailedErrors = true;
                
                // ✅ SIGNALR: Set keepalive and timeout intervals
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            });

            // Core Service Registrations
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<INotificationTemplateService, NotificationTemplateService>();
            services.AddTransient<INotificationTypeService, NotificationTypeService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddSingleton<ICacheProvider, DistributedHybridCacheProvider>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IRolePermissionService, RolePermissionService>();
        }
    }
}
