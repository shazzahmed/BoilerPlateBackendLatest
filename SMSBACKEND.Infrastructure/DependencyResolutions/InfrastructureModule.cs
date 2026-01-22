using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Common.Utilities.Enums;

namespace Infrastructure.DependencyResolutions
{
    public static class InfrastructureModule
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration, ApplicationType applicationType)
        {
            ConfigurationModule.Configure(services, configuration, applicationType);
            RepositoryModule.ConfigureDbContext(services, configuration);
            Documentations.AddSwagger(services, applicationType);
            Utilities.RegisterLifetime(services);
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigurationModule.Configure(app, env);
            RepositoryModule.Configure(app, env);
            Documentations.RegisterApps(app);
            Utilities.LifetimeApps(app);
        }
    }
}
