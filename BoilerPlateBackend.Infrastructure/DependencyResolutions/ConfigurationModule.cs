using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common.Helpers;
using static Common.Utilities.Enums;
using Common.Options;
using AppOptions = Common.Options.AppOptions;
using RedisOptions = Common.Options.RedisOptions;
using JwtOptions = Common.Options.JwtOptions;

namespace Infrastructure.DependencyResolutions
{
    public static class ConfigurationModule
    {

        public static IServiceCollection Configure(IServiceCollection services, IConfiguration configuration, ApplicationType applicationType)
        {
            services.Configure<AppOptions>(configuration.GetSection("AppOptions"));
            services.Configure<DefaultIdentityOptionsModel>(configuration.GetSection("DefaultIdentityOptionsModel"));
            services.Configure<ComponentOptions>(configuration.GetSection("Component"));
            services.Configure<InfrastructureOptions>(configuration.GetSection("Infrastructure"));
            services.Configure<SecurityOptions>(configuration.GetSection("Security"));
            services.Configure<GoogleOptions>(configuration.GetSection("Google"));
            services.Configure<OutlookOptions>(configuration.GetSection("Outlook"));
            services.Configure<FacebookOptions>(configuration.GetSection("Facebook"));
            services.Configure<TwitterOptions>(configuration.GetSection("Twitter"));
            services.Configure<ZohoOptions>(configuration.GetSection("Zoho"));
            services.Configure<RedisOptions>(configuration.GetSection("redis"));
            services.Configure<JwtOptions>(configuration.GetSection("jwt"));
            AppServicesHelper.Configuration = configuration;
            return services;
        }


        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env is null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            AppServicesHelper.Services = app.ApplicationServices;
        }
    }
}
