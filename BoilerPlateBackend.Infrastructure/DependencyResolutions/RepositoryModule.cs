using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Common.Options;
using Infrastructure.Repository;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Hangfire;

namespace Infrastructure.DependencyResolutions
{
    public static class RepositoryModule
    {
        public static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SqlServerDbContext>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    msSqlServerOptions => msSqlServerOptions.MigrationsAssembly("Infrastructure")
                    .EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)
                    .CommandTimeout(30)) // 30 seconds command timeout
                .EnableSensitiveDataLogging() // Enable sensitive data logging
                .EnableDetailedErrors(), // Optional: Enable detailed errors for more debugging information
                ServiceLifetime.Scoped);
            services.AddHangfire(config => config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"))
            .UseRecommendedSerializerSettings());
            services.AddHangfireServer();
            services.BuildServiceProvider().GetService<UserManager<ApplicationUser>>();

            services.AddScoped<ISqlServerDbContext, SqlServerDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IUnitOfWork>(unitOfWork => new UnitOfWork(unitOfWork.GetService<ISqlServerDbContext>()));

            // Core Repository Registrations
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddTransient<INotificationTypeRepository, NotificationTypeRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IRolePermissionRepository, RolePermissionRepository>();

        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            DbMigrator.Migrate(app);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            DataSeeder.SeedingData(app);
            DataSeeder.Seed(app);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
