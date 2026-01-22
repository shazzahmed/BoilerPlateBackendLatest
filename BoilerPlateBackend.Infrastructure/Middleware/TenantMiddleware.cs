using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Infrastructure.Middleware
{
    /// <summary>
    /// Middleware to extract tenant ID from request headers
    /// and store it in HttpContext for use throughout the request pipeline
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip if response has already started (e.g., 401 from expired token)
            if (!context.Response.HasStarted)
            {
                // Try to get tenant ID from custom header (sent by frontend)
                if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
                {
                    if (int.TryParse(tenantIdHeader, out var tenantId) && tenantId > 0)
                    {
                        // Store tenant ID in HttpContext.Items for use in DbContext
                        context.Items["TenantId"] = tenantId;
                        
                        // Optional: Log for debugging
                        // Console.WriteLine($"🔑 Tenant ID from header: {tenantId}");
                    }
                }
            }

            // Continue processing request (if not already processed)
            if (!context.Response.HasStarted)
            {
                await _next(context);
            }
        }
    }
}

