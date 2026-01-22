using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Common.Options;

namespace Infrastructure.Hubs
{
    /// <summary>
    /// SignalR Hub for Real-time Dashboard Updates
    /// Handles broadcasting of metrics, notifications, and events
    /// ✅ SECURITY: Uses [AllowAnonymous] + manual validation in OnConnectedAsync
    ///    This is required because [Authorize] blocks WebSocket connections before JWT token can be extracted
    /// </summary>
    [AllowAnonymous]
    public class DashboardHub : Hub
    {
        private readonly ILogger<DashboardHub> _logger;
        private readonly JwtOptions _jwtOptions;

        public DashboardHub(ILogger<DashboardHub> logger, IOptionsSnapshot<JwtOptions> jwtOptions)
        {
            _logger = logger;
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// Called when client connects
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            try
            {
                // ✅ MANUAL JWT VALIDATION: Extract and validate token from query string
                var tokenFromQuery = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
                
                _logger.LogInformation("🔌 SignalR connection attempt. ConnectionId: {ConnectionId}", Context.ConnectionId);
                _logger.LogInformation("   Token in query: {HasToken}", !string.IsNullOrEmpty(tokenFromQuery) ? "YES" : "NO");

                ClaimsPrincipal? principal = null;
                
                // If ASP.NET didn't authenticate (Context.User is null), manually validate the JWT token
                if (Context.User == null || Context.User.Identity?.IsAuthenticated != true)
                {
                    if (string.IsNullOrEmpty(tokenFromQuery))
                    {
                        _logger.LogWarning("❌ UNAUTHORIZED: No token provided. ConnectionId: {ConnectionId}", Context.ConnectionId);
                        Context.Abort();
                        return;
                    }

                    // ✅ MANUALLY VALIDATE JWT TOKEN using JwtOptions
                    try
                    {
                        if (string.IsNullOrEmpty(_jwtOptions.SecretKey))
                        {
                            _logger.LogError("❌ JWT SecretKey configuration is missing!");
                            Context.Abort();
                            return;
                        }

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
                        var validationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false, // ✅ Don't validate issuer for SignalR (token uses apiUrl as issuer)
                            ValidateAudience = false, // ✅ Don't validate audience for SignalR (token uses apiUrl as audience)
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuerSigningKey = true, // ✅ IMPORTANT: Validate signature
                            ValidateLifetime = true, // ✅ IMPORTANT: Validate expiration
                            ClockSkew = TimeSpan.Zero
                        };

                        principal = tokenHandler.ValidateToken(tokenFromQuery, validationParameters, out SecurityToken validatedToken);
                        _logger.LogInformation("✅ JWT Token manually validated for SignalR connection");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "❌ UNAUTHORIZED: Invalid JWT token. ConnectionId: {ConnectionId}", Context.ConnectionId);
                        Context.Abort();
                        return;
                    }
                }
                else
                {
                    // Use existing authenticated user
                    principal = Context.User;
                }

                // Extract claims from principal
                var userId = principal?.FindFirst(JwtClaimTypes.Id)?.Value ?? 
                             principal?.FindFirst("id")?.Value;
                var tenantId = principal?.FindFirst("TenantId")?.Value;
                var userName = principal?.FindFirst(ClaimTypes.Name)?.Value ?? 
                               principal?.FindFirst("name")?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("❌ UNAUTHORIZED: No user ID found in token claims. ConnectionId: {ConnectionId}", Context.ConnectionId);
                    Context.Abort();
                    return;
                }

                _logger.LogInformation("✅ SignalR AUTHENTICATED: User {UserId} ({UserName}), Tenant {TenantId}, ConnectionId {ConnectionId}", 
                    userId, userName ?? "Unknown", tenantId ?? "Unknown", Context.ConnectionId);
                
                // Add user to tenant-specific group
                if (!string.IsNullOrEmpty(tenantId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Tenant_{tenantId}");
                    _logger.LogInformation("👥 User {UserId} added to Tenant_{TenantId} group", userId, tenantId);
                }
                
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ EXCEPTION in OnConnectedAsync for ConnectionId {ConnectionId}", Context.ConnectionId);
                Context.Abort();
            }
        }

        /// <summary>
        /// Called when client disconnects
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = Context.User?.FindFirst(JwtClaimTypes.Id)?.Value ?? 
                             Context.User?.FindFirst("id")?.Value;
                var tenantId = Context.User?.FindFirst("TenantId")?.Value;
                
                _logger.LogInformation("SignalR Disconnected: User {UserId}, ConnectionId {ConnectionId}", 
                    userId, Context.ConnectionId);
                
                if (exception != null)
                {
                    _logger.LogError(exception, "SignalR disconnected with error for User {UserId}", userId);
                }
                
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in OnDisconnectedAsync for ConnectionId {ConnectionId}", Context.ConnectionId);
            }
        }

        /// <summary>
        /// Join Tenant Group (called from client)
        /// </summary>
        public async Task JoinTenantGroup(int tenantId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Tenant_{tenantId}");
            _logger.LogInformation("User joined tenant group: {TenantId}", tenantId);
        }

        /// <summary>
        /// Leave Tenant Group (called from client)
        /// </summary>
        public async Task LeaveTenantGroup(int tenantId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Tenant_{tenantId}");
            _logger.LogInformation("User left tenant group: {TenantId}", tenantId);
        }

        /// <summary>
        /// Broadcast Metric Update to all clients in tenant
        /// Called from backend when data changes (e.g., new student added)
        /// </summary>
        public async Task BroadcastMetricUpdate(int tenantId, string metricType, object data)
        {
            await Clients.Group($"Tenant_{tenantId}").SendAsync("ReceiveMetricUpdate", metricType, data);
            _logger.LogInformation("Metric update broadcasted: {MetricType} for Tenant {TenantId}", metricType, tenantId);
        }

        /// <summary>
        /// Send Notification to specific user
        /// </summary>
        public async Task SendNotificationToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", new { message, timestamp = DateTime.UtcNow });
            _logger.LogInformation("Notification sent to User {UserId}", userId);
        }

        /// <summary>
        /// Broadcast Notification to all tenant users
        /// </summary>
        public async Task BroadcastNotificationToTenant(int tenantId, string message)
        {
            await Clients.Group($"Tenant_{tenantId}").SendAsync("ReceiveNotification", new { message, timestamp = DateTime.UtcNow });
            _logger.LogInformation("Notification broadcasted to Tenant {TenantId}", tenantId);
        }

        /// <summary>
        /// Request Dashboard Refresh for all clients in tenant
        /// </summary>
        public async Task RequestDashboardRefresh(int tenantId)
        {
            await Clients.Group($"Tenant_{tenantId}").SendAsync("RefreshDashboard");
            _logger.LogInformation("Dashboard refresh requested for Tenant {TenantId}", tenantId);
        }
    }
}

