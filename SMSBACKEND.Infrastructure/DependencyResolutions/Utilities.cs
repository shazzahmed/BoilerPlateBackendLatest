using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Common.Helpers;
using Application.Interfaces;
using Common.Options;
using Application.Mappings;
using Infrastructure.Middleware;
using Infrastructure.Helpers;
using Microsoft.Extensions.Caching.Hybrid;
using Infrastructure.Services.Communication;

namespace Infrastructure.DependencyResolutions
{
    public static class Utilities
    {
        public static void RegisterLifetime(this IServiceCollection services)
        {
            var redisOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<RedisOptions>>();

            services.AddAutoMapper(typeof(ModelMapper)); // Use Utilities instead of Startup
            services.AddSingleton<IJwtHandler, JwtHandler>();
            services.AddHttpClient(); // Basic registration for IHttpClientFactory
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<SseService>();
            //services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            //services.AddStackExchangeRedisCache(action =>
            //{
            //    action.Configuration = redisOptions?.Value.ConnectionString;
            //});
            services.AddHybridCache(options =>
            {
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(60),
                    LocalCacheExpiration = TimeSpan.FromMinutes(60)
                };
            });
        }

        public static void LifetimeApps(this IApplicationBuilder app)
        {

        }
    }
}