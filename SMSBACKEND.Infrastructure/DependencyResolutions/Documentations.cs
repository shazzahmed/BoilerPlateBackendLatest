using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Common.Options;
using static Common.Utilities.Enums;

namespace Infrastructure.DependencyResolutions
{
    public static class Documentations
    {
        private static IOptionsSnapshot<InfrastructureOptions> infrastructureOptions = null;

        public static void AddSwagger(IServiceCollection services, ApplicationType applicationType)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            infrastructureOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<InfrastructureOptions>>();
#pragma warning restore CS8601 // Possible null reference assignment.
            switch (infrastructureOptions?.Value.Documentation)
            {
                case "Swagger":
                    Swagger.ConfigureService(services,applicationType);
                    break;
                default:
                    break;
            }
        }

        public static void RegisterApps(IApplicationBuilder apps)
        {
            switch (infrastructureOptions.Value.Documentation)
            {
                case "Swagger":
                    Swagger.ConfigureApp(apps);
                    break;
                default:
                    break;
            }
        }
    }
}
