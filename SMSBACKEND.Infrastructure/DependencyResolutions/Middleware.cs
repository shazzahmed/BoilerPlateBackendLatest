using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using static Common.Utilities.Enums;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Infrastructure.Database;
using Infrastructure.Middleware;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;

namespace Infrastructure.DependencyResolutions
{
    public static class Middleware
    {
        [Obsolete]
        internal static void RegisterServices(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration, ApplicationType applicationType)
        {
            //services.AddScoped<CurrentUser>();
            // AddCors must be before AddMvc
            //services.AddCors(p => p.AddPolicy("corsapp", builder =>
            //{
            //    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            //}));
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(
            //        "CorsPolicy",
            //        builder =>
            //        {
            //            //builder.AllowAnyOrigin()
            //            builder.WithOrigins("http://localhost:5173")
            //               .AllowAnyMethod()
            //                .AllowAnyHeader()
            //                .AllowCredentials()
            //                .WithExposedHeaders("x-pagination");
            //        });
            //});
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200", "https://ilmsuite.pk", 
                            "http://dev.ilmsuite.pk", "https://dev.ilmsuite.pk")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials() // Required for SignalR
                            .WithExposedHeaders("x-pagination");
                    });
            });
            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            services.AddHttpLogging(options =>
            {
                options.RequestBodyLogLimit = 4096; // limit in bytes
                options.ResponseBodyLogLimit = 4096;

                // Explicitly log only these headers
                options.RequestHeaders.Add("User-Agent");
                options.ResponseHeaders.Add("Content-Type");

                options.LoggingFields =
                HttpLoggingFields.RequestMethod |
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.RequestQuery |
                HttpLoggingFields.RequestHeaders |
                HttpLoggingFields.ResponseStatusCode |
                HttpLoggingFields.Duration;
            });
            //services.AddControllers().AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //    //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});
            //.AddViewLocalization().AddDataAnnotationsLocalization();
            services.AddHealthChecks();
            if (applicationType == ApplicationType.Web)
            {
                services.AddControllersWithViews(config =>
                {
                    var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                    var defaultAuthPolicy = defaultAuthBuilder
                        .RequireAuthenticatedUser()
                        .Build();

                    // global authorization filter which is applied on whole application if you want to bypass it on some endpoint use [AllowAnonymous]
                    //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
                });
                services.AddRazorPages();
                services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(60);
                });
                services.AddDataProtection()
                .SetApplicationName("Business ERP Solution")
                .AddKeyManagementOptions(options =>
                {
                    options.NewKeyLifetime = new TimeSpan(180, 0, 0, 0);
                    options.AutoGenerateKeys = true;
                });
            }

        }

        public static void RegisterApps(IApplicationBuilder app, IWebHostEnvironment env, ApplicationType applicationType)
        {
            if (applicationType == ApplicationType.CoreApi)
            {
                // ✅ GLOBAL EXCEPTION HANDLER - Must be first in pipeline
                app.UseMiddleware<ErrorHandlerMiddleware>();
                
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                }
                app.UseHealthChecks("/healthcheck", new HealthCheckOptions
                {
                    Predicate = _ => true,
                });
                // Enable HTTP request/response logging
                app.UseHttpLogging();
                app.UseRouting();
                // global cors policy
                app.UseCors("CorsPolicy");
                //app.UseCors(x => x
                //    .AllowAnyOrigin()
                //    .AllowAnyMethod()
                //    .AllowAnyHeader());

                app.UseAuthentication();
                
                // MULTI-TENANCY: Extract tenant ID from X-Tenant-Id header (must be before validation)
                app.UseMiddleware<TenantMiddleware>();
                
                // MULTI-TENANCY: Validate tenant context after authentication
                app.UseMiddleware<TenantValidationMiddleware>();
                
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<Infrastructure.Hubs.DashboardHub>("/dashboardHub");
                });
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                }); 
            } else if(applicationType == ApplicationType.Web)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages();
                });
            }
        }
    }
}
