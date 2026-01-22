using System.Text;
using Domain.Entities;
using Common.DTO;
using System.Security.Claims;
using Application.Interfaces;
using Common.Options;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static Common.Utilities.Enums;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Common.Helpers;
using Common.DTO.Request;
using IdentityModel;

namespace Infrastructure.Security
{
    public static class Securities
    {
        private static IServiceProvider? _serviceProvider;
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration, ApplicationType applicationType)
        {
            // Store the service provider for later use
            _serviceProvider = services.BuildServiceProvider();
           
            var defaultIdentityOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<DefaultIdentityOptionsModel>>();
            services.AddTransient<ISecurityService, SecurityAspnetIdentity>();
           
            if (applicationType == ApplicationType.CoreApi)
            {
                AddIdentity(services, applicationType, defaultIdentityOptions);
            }
            AddAuthentication(services, applicationType);
            AddAuthorization(services, configuration, applicationType);
        }
        private static void AddAuthentication(IServiceCollection services, ApplicationType applicationType)
        {
            var appOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<AppOptions>>();
            if (applicationType == ApplicationType.CoreApi)
            {
                var jwtOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<JwtOptions>>();
                var key = Encoding.ASCII.GetBytes(jwtOptions.Value.SecretKey);
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = appOptions?.Value.ApiUrl,
                        ValidAudience = appOptions?.Value.ApiUrl,
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var token = context.SecurityToken as JwtSecurityToken;
                            if (token != null && context.Principal?.Identity is ClaimsIdentity identity)
                            {
                                identity.AddClaim(new Claim("access_token", token.RawData));

                                // Add roles from token claims if available
                                if (token.Claims.Any(c => c.Type == ClaimTypes.Role))
                                {
                                    foreach (var roleClaim in token.Claims.Where(c => c.Type == ClaimTypes.Role))
                                    {
                                        identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                                    }
                                }
                                else
                                {
                                    // Fallback to database with timeout protection
                                    try
                                    {
                                        using (var scope = context.HttpContext.RequestServices.CreateScope())
                                        {
                                            var authHelper = scope.ServiceProvider.GetRequiredService<IAuthenticationHelper>();
                                            foreach (var role in authHelper.GetRoles(token))
                                            {
                                                identity.AddClaim(new Claim(ClaimTypes.Role, role));
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // Log the error but don't fail authentication
                                        Console.WriteLine($"Warning: Could not fetch roles from database: {ex.Message}");
                                    }
                                }

                                // Get user data with timeout protection
                                try
                                {
                                    var userService = context.HttpContext.RequestServices.GetRequiredService<ISecurityService>();
                                    var email = context.Principal?.Identity?.Name;

                                    if (!string.IsNullOrEmpty(email))
                                    {
                                        // Use CancellationToken with 5 second timeout
                                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                                        var userResult = await userService.GetUser(email);
                                        if (userResult?.Data != null)
                                        {
                                            var user = (ApplicationUserModel)userResult.Data;
                                            identity.AddClaim(new Claim(JwtClaimTypes.Id, user.Id));
                                            identity.AddClaim(new Claim(JwtClaimTypes.Name, user.UserName ?? ""));
                                        }
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    // Database call timed out - continue with token validation
                                    Console.WriteLine("Warning: Database call timed out during token validation");
                                }
                                catch (Exception ex)
                                {
                                    // Log the error but don't fail authentication
                                    Console.WriteLine($"Warning: Could not fetch user data: {ex.Message}");
                                }
                            }
                        },
                        OnMessageReceived = context =>
                        {
                            var authorization = context.Request.Headers["Authorization"].ToString();
                            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer"))
                            {
                                context.Token = authorization.Substring("Bearer ".Length).Trim();
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            // Don't write response body for CONNECT requests (SignalR WebSocket)
                            if (context.Request.Method == "CONNECT" || context.Request.Path.StartsWithSegments("/dashboardHub"))
                            {
                                return Task.CompletedTask;
                            }
                            
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                var path = context.Request.Path.Value;
                                // Allow refresh token requests
                                if (path.Contains("refresh-access-token") || path.Contains("Login") || path.Contains("stream") || path.Contains("ImportProgress"))
                                {
                                    return Task.CompletedTask; // Do not block refresh requests
                                }
                                context.Response.StatusCode = 401; // Unauthorized
                            }
                            context.Response.ContentType = "application/json";
                            var response = new
                            {
                                success = false,
                                message = "Un-Authorized Access"
                            };
                            return context.Response.WriteAsync(JsonSerializerHelper.Serialize(response));
                        },
                        OnChallenge = context =>
                        {
                            // Don't write response body for CONNECT requests (SignalR WebSocket)
                            if (context.Request.Method == "CONNECT" || context.Request.Path.StartsWithSegments("/dashboardHub"))
                            {
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }
                            
                            var response = new
                            {
                                success = false,
                                message = "Un-Authorized Access"
                            };
                            context.HandleResponse();
                            return context.Response.WriteAsync(JsonSerializerHelper.Serialize(response));
                        }
                    };
                });
            }
            else if (applicationType == ApplicationType.Web)
            {
                // Configure Web application authentication
            }
        }
        private static void AddAuthorization(IServiceCollection services, IConfiguration configuration, ApplicationType applicationType)
        {
            services.AddAuthorization(config =>
            {
                
               
            });
                
        }

        private static void AddIdentity(IServiceCollection services, ApplicationType applicationType, IOptionsSnapshot<DefaultIdentityOptionsModel> _DefaultIdentityOptions)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = _DefaultIdentityOptions.Value.PasswordRequireDigit;
                options.Password.RequiredLength = _DefaultIdentityOptions.Value.PasswordRequiredLength;
                options.Password.RequireNonAlphanumeric = _DefaultIdentityOptions.Value.PasswordRequireNonAlphanumeric;
                options.Password.RequireUppercase = _DefaultIdentityOptions.Value.PasswordRequireUppercase;
                options.Password.RequireLowercase = _DefaultIdentityOptions.Value.PasswordRequireLowercase;
                options.Password.RequiredUniqueChars = _DefaultIdentityOptions.Value.PasswordRequiredUniqueChars;

                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(_DefaultIdentityOptions.Value.LockoutDefaultLockoutTimeSpanInMinutes);
                //options.Lockout.MaxFailedAccessAttempts = _DefaultIdentityOptions.Value.LockoutMaxFailedAccessAttempts;
                //options.Lockout.AllowedForNewUsers = _DefaultIdentityOptions.Value.LockoutAllowedForNewUsers;

                options.User.RequireUniqueEmail = _DefaultIdentityOptions.Value.UserRequireUniqueEmail;

                options.SignIn.RequireConfirmedEmail = _DefaultIdentityOptions.Value.SignInRequireConfirmedEmail;
            })
            .AddEntityFrameworkStores<SqlServerDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/Home/Login";
            });
        }
    }
}
