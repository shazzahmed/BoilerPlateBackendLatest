using System.Security.Claims;
using Application.ServiceContracts.IServices;
using Domain.Entities;
using Domain.RepositoriesContracts;
using IdentityModel;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ApplicationUser = Domain.Entities.ApplicationUser;

namespace Infrastructure.Services
{
    /// <summary>
    /// Service for managing tenant context and multi-tenancy operations
    /// Extracts tenant information from JWT claims and provides tenant validation
    /// </summary>
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISqlServerDbContext _context;

        public TenantService(
            IHttpContextAccessor httpContextAccessor,
            ISqlServerDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        /// <summary>
        /// Gets the TenantId from the current user's JWT claims
        /// </summary>
        public int GetCurrentTenantId()
        {
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.Claims
                ?.FirstOrDefault(c => c.Type == "TenantId" || c.Type == "tenantId")?.Value;

            if (int.TryParse(tenantIdClaim, out var tenantId))
            {
                return tenantId;
            }

            // Log warning: No tenant context found
            Console.WriteLine("WARNING: No TenantId found in claims. Returning 0.");
            return 0;
        }

        /// <summary>
        /// Gets the full Tenant entity for the current user
        /// </summary>
        public async Task<Tenant?> GetCurrentTenantAsync()
        {
            var tenantId = GetCurrentTenantId();
            if (tenantId == 0)
            {
                return null;
            }

            return await _context.Set<Tenant>()
                .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.IsActive && !t.IsDeleted);
        }

        /// <summary>
        /// Gets a tenant by ID
        /// </summary>
        public async Task<Tenant?> GetTenantByIdAsync(int tenantId)
        {
            return await _context.Set<Tenant>()
                .FirstOrDefaultAsync(t => t.TenantId == tenantId && !t.IsDeleted);
        }

        /// <summary>
        /// Gets a tenant by its unique code
        /// </summary>
        public async Task<Tenant?> GetTenantByCodeAsync(string tenantCode)
        {
            return await _context.Set<Tenant>()
                .FirstOrDefaultAsync(t => t.TenantCode == tenantCode && !t.IsDeleted);
        }

        /// <summary>
        /// Validates if the current user has access to the specified tenant
        /// Critical security method to prevent cross-tenant data access
        /// </summary>
        public async Task<bool> ValidateTenantAccessAsync(int tenantId)
        {
            var currentTenantId = GetCurrentTenantId();

            // If no tenant context, deny access
            if (currentTenantId == 0)
            {
                Console.WriteLine($"SECURITY WARNING: Attempted access to tenant {tenantId} without tenant context");
                return false;
            }

            // Check if user's tenant matches requested tenant
            var hasAccess = currentTenantId == tenantId;

            if (!hasAccess)
            {
                Console.WriteLine($"SECURITY WARNING: User from tenant {currentTenantId} attempted to access tenant {tenantId}");
            }

            return hasAccess;
        }

        /// <summary>
        /// Checks if the current tenant's subscription is valid
        /// </summary>
        public async Task<bool> IsSubscriptionValidAsync()
        {
            var tenant = await GetCurrentTenantAsync();

            if (tenant == null)
            {
                return false;
            }

            // Check if subscription is marked as valid
            if (!tenant.IsSubscriptionValid)
            {
                return false;
            }

            // Check if subscription has expired
            if (tenant.SubscriptionEndDate.HasValue && tenant.SubscriptionEndDate.Value < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if tenant has reached their user limit
        /// </summary>
        public async Task<bool> HasReachedUserLimitAsync()
        {
            var tenant = await GetCurrentTenantAsync();

            if (tenant == null)
            {
                return true; // Block if tenant not found
            }

            return tenant.CurrentUserCount >= tenant.MaxUsers;
        }

        /// <summary>
        /// Checks if a specific feature is enabled for the current tenant
        /// </summary>
        public async Task<bool> IsFeatureEnabledAsync(string featureName)
        {
            var tenant = await GetCurrentTenantAsync();

            if (tenant == null || string.IsNullOrWhiteSpace(tenant.EnabledFeatures))
            {
                return false;
            }

            // Features are stored as comma-separated values
            var enabledFeatures = tenant.EnabledFeatures.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim())
                .ToList();

            return enabledFeatures.Contains(featureName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Updates the cached count for users
        /// Call this after adding/removing users
        /// </summary>
        public async Task UpdateResourceCountsAsync()
        {
            var tenantId = GetCurrentTenantId();
            if (tenantId == 0)
            {
                return;
            }

            var tenant = await GetTenantByIdAsync(tenantId);
            if (tenant == null)
            {
                return;
            }

            // Count active users
            var userCount = await _context.Set<ApplicationUser>()
                .Where(u => u.TenantId == tenantId && u.IsActive)
                .CountAsync();

            // Update tenant
            tenant.CurrentUserCount = userCount;
            tenant.UpdatedAt = DateTime.UtcNow;

            _context.Set<Tenant>().Update(tenant);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all active tenants (for super admin)
        /// </summary>
        public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
        {
            // Note: This bypasses tenant filtering - only for super admins
            return await _context.Set<Tenant>()
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new tenant (for super admin)
        /// </summary>
        public async Task<Tenant> CreateTenantAsync(Tenant tenant)
        {
            tenant.CreatedAt = DateTime.UtcNow;
            tenant.UpdatedAt = DateTime.UtcNow;
            tenant.IsActive = true;
            tenant.IsDeleted = false;

            _context.Set<Tenant>().Add(tenant);
            await _context.SaveChangesAsync();

            return tenant;
        }

        /// <summary>
        /// Updates an existing tenant (for super admin)
        /// </summary>
        public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
        {
            tenant.UpdatedAt = DateTime.UtcNow;

            _context.Set<Tenant>().Update(tenant);
            await _context.SaveChangesAsync();

            return tenant;
        }

        /// <summary>
        /// Soft deletes a tenant
        /// </summary>
        public async Task<bool> DeleteTenantAsync(int tenantId)
        {
            var tenant = await GetTenantByIdAsync(tenantId);
            if (tenant == null)
            {
                return false;
            }

            tenant.IsDeleted = true;
            tenant.IsActive = false;
            tenant.DeletedAt = DateTime.UtcNow;

            _context.Set<Tenant>().Update(tenant);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}


