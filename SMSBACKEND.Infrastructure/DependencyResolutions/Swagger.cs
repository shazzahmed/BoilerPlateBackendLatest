using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using static Common.Utilities.Enums;
using Infrastructure.Filters;

namespace Infrastructure.DependencyResolutions
{
    public static class Swagger
    {
        public static void ConfigureService(IServiceCollection services, ApplicationType applicationType)
        {
            if (applicationType == ApplicationType.CoreApi)
            {
                services.AddSwaggerGen(c => {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "School MS API",
                        Description = "School MS Web API"
                    });
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });
                    c.EnableAnnotations();
                });
            }
            else if (applicationType == ApplicationType.Web)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "School Management System", Version = "v1" });
                    c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        BearerFormat = "JWT",
                        Description = "JWT Authorization header using the Bearer scheme."
                    });

                    c.OperationFilter<AuthResponsesOperationFilter>();
                });
            }
        }

        public static void ConfigureApp(IApplicationBuilder apps)
        {
            apps.UseSwagger();
            apps.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "School MS API V1");
            });
        }
    }
}
