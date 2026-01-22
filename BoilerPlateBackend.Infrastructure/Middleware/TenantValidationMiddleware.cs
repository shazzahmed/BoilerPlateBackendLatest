using Application.ServiceContracts.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Middleware
{
    /// <summary>
    /// Middleware to validate tenant context and subscription status on every API request
    /// Ensures users have valid tenant access and active subscriptions
    /// </summary>
    public class TenantValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantValidationMiddleware> _logger;

        // Endpoints that should bypass tenant validation
        private readonly HashSet<string> _excludedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "/api/account/login",
            "/api/account/register",
            "/api/account/forgotpassword",
            "/api/account/resetpassword",
            "/api/account/refreshaccesstoken",
            "/health",
            "/swagger",
            "/api/tenant" // Super admin tenant management
        };

        public TenantValidationMiddleware(
            RequestDelegate next,
            ILogger<TenantValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            // Skip validation for excluded paths
            if (ShouldBypassValidation(context))
            {
                await _next(context);
                return;
            }

            // Skip validation if response has already started (e.g., 401 from expired token)
            if (context.Response.HasStarted)
            {
                return;
            }

            // Skip validation for unauthenticated requests (let AuthZ handle it)
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }

            // Get current tenant ID from JWT claims
            var tenantId = tenantService.GetCurrentTenantId();

            // SECURITY CHECK 1: Validate tenant context exists
            if (tenantId == 0)
            {
                _logger.LogWarning("Request from user {User} without tenant context", 
                    context.User.Identity?.Name ?? "Unknown");
                
                await WriteErrorResponse(context, 
                    HttpStatusCode.Forbidden, 
                    "TENANT_REQUIRED",
                    "No tenant context found. Please contact support.");
                return;
            }

            try
            {
                // SECURITY CHECK 2: Validate tenant exists and is active
                var tenant = await tenantService.GetCurrentTenantAsync();
                
                if (tenant == null)
                {
                    _logger.LogError("Tenant {TenantId} not found in database", tenantId);
                    
                    await WriteErrorResponse(context, 
                        HttpStatusCode.Forbidden, 
                        "TENANT_NOT_FOUND",
                        "Tenant not found. Please contact support.");
                    return;
                }

                if (!tenant.IsActive)
                {
                    _logger.LogWarning("Access attempt to inactive tenant {TenantId} by user {User}", 
                        tenantId, context.User.Identity?.Name ?? "Unknown");
                    
                    await WriteErrorResponse(context, 
                        HttpStatusCode.Forbidden, 
                        "TENANT_INACTIVE",
                        "Your organization account is inactive. Please contact support.");
                    return;
                }

                // BUSINESS CHECK 3: Validate subscription status
                var isSubscriptionValid = await tenantService.IsSubscriptionValidAsync();
                
                if (!isSubscriptionValid)
                {
                    _logger.LogWarning("Access attempt with expired subscription for tenant {TenantId}", tenantId);
                    
                    await WriteErrorResponse(context, 
                        HttpStatusCode.PaymentRequired, // 402
                        "SUBSCRIPTION_EXPIRED",
                        "Your subscription has expired. Please renew to continue using the system.");
                    return;
                }

                // Add tenant information to HttpContext for easy access in controllers
                context.Items["TenantId"] = tenantId;
                context.Items["TenantCode"] = tenant.TenantCode;
                context.Items["TenantName"] = tenant.Name;

                // All validations passed, proceed with request
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating tenant {TenantId}", tenantId);
                
                await WriteErrorResponse(context, 
                    HttpStatusCode.InternalServerError, 
                    "TENANT_VALIDATION_ERROR",
                    "An error occurred validating your account. Please try again.");
            }
        }

        /// <summary>
        /// Determines if the current request should bypass tenant validation
        /// </summary>
        private bool ShouldBypassValidation(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Check excluded paths
            foreach (var excludedPath in _excludedPaths)
            {
                if (path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // Allow OPTIONS requests (CORS preflight)
            if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Writes a standardized error response
        /// </summary>
        private async Task WriteErrorResponse(
            HttpContext context, 
            HttpStatusCode statusCode, 
            string errorCode, 
            string message)
        {
            // Don't try to write if response has already started
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Cannot write error response - response has already started");
                return;
            }

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Status = "Failed",
                Message = message,
                ErrorCode = errorCode,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}


