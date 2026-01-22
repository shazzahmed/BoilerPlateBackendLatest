using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Common.DTO.Response;

namespace Infrastructure.Middleware
{
    /// <summary>
    /// Global exception handler middleware
    /// Catches all unhandled exceptions and returns consistent BaseModel responses
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ErrorHandlerMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlerMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(context, exception);
            }
        }

        private async Task HandleErrorAsync(HttpContext context, Exception exception)
        {
            // ✅ Log the exception with full details
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

            // ✅ Return BaseModel format for consistency
            var errorResponse = BaseModel.Failed(
                message: _env.IsDevelopment() 
                    ? $"Error: {exception.Message}"  // Development: show details
                    : "An error occurred while processing your request",  // Production: generic message
                data: _env.IsDevelopment() ? exception.StackTrace : null  // Development: include stack trace
            );

            var payload = JsonConvert.SerializeObject(errorResponse);
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsync(payload);
        }
    }
}
