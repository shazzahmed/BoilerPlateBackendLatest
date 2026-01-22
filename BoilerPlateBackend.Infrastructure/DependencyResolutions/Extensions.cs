
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Security;
using static Common.Utilities.Enums;
using Infrastructure.Services.Communication;

namespace Infrastructure.DependencyResolutions
{
    public static class Extensions
    {
        [Obsolete]
        public static void Configure(this IServiceCollection services, IConfiguration configuration, ApplicationType applicationType)
        {
            InfrastructureModule.Configure(services, configuration, applicationType);
            Middleware.RegisterServices(services, configuration, applicationType);
            Securities.RegisterServices(services, configuration, applicationType);
            Communications.RegisterServices(services);
        }


        public static void RegisterApps(this IApplicationBuilder app, IWebHostEnvironment env, ApplicationType applicationType)
        {
            InfrastructureModule.Configure(app, env);
            Middleware.RegisterApps(app, env, applicationType);
        }
    }
}
