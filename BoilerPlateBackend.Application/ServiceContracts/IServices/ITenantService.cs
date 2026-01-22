using Domain.Entities;

namespace Application.ServiceContracts.IServices
{
    /// <summary>
    /// Service interface for managing tenant context and operations
    /// Provides methods to get current tenant information and validate tenant access
    /// </summary>
    public interface ITenantService
    {
        /// <summary>
        /// Gets the TenantId of the currently authenticated user from JWT claims
        /// Returns 0 if no tenant context is found
        /// </summary>
        /// <returns>Current tenant ID</returns>
        int GetCurrentTenantId();

        /// <summary>
        /// Gets the full Tenant entity for the currently authenticated user
        /// </summary>
        /// <returns>Tenant entity or null if not found</returns>
        Task<Tenant?> GetCurrentTenantAsync();

        /// <summary>
        /// Gets a tenant by its unique TenantId
        /// </summary>
        /// <param name="tenantId">The tenant ID to lookup</param>
        /// <returns>Tenant entity or null if not found</returns>
        Task<Tenant?> GetTenantByIdAsync(int tenantId);

        /// <summary>
        /// Gets a tenant by its unique TenantCode
        /// </summary>
        /// <param name="tenantCode">The tenant code to lookup (e.g., "ORG001")</param>
        /// <returns>Tenant entity or null if not found</returns>
        Task<Tenant?> GetTenantByCodeAsync(string tenantCode);

        /// <summary>
        /// Validates if the current user has access to the specified tenant
        /// Prevents cross-tenant data access
        /// </summary>
        /// <param name="tenantId">Tenant ID to validate access to</param>
        /// <returns>True if user belongs to the tenant, false otherwise</returns>
        Task<bool> ValidateTenantAccessAsync(int tenantId);

        /// <summary>
        /// Checks if the tenant's subscription is currently valid (not expired)
        /// </summary>
        /// <returns>True if subscription is active, false if expired</returns>
        Task<bool> IsSubscriptionValidAsync();


        /// <summary>
        /// Checks if a specific feature is enabled for the current tenant
        /// Based on subscription tier and enabled features
        /// </summary>
        /// <param name="featureName">Name of the feature to check (e.g., "Exams", "Library")</param>
        /// <returns>True if feature is enabled, false otherwise</returns>
        Task<bool> IsFeatureEnabledAsync(string featureName);

        /// <summary>
        /// Updates the cached counts for students and staff
        /// Should be called when students/staff are added or removed
        /// </summary>
        Task UpdateResourceCountsAsync();

        /// <summary>
        /// Gets all active tenants (for super admin use)
        /// </summary>
        /// <returns>List of all active tenants</returns>
        Task<IEnumerable<Tenant>> GetAllTenantsAsync();

        /// <summary>
        /// Creates a new tenant (for super admin use)
        /// </summary>
        /// <param name="tenant">Tenant entity to create</param>
        /// <returns>Created tenant with assigned ID</returns>
        Task<Tenant> CreateTenantAsync(Tenant tenant);

        /// <summary>
        /// Updates an existing tenant (for super admin use)
        /// </summary>
        /// <param name="tenant">Tenant entity with updates</param>
        /// <returns>Updated tenant</returns>
        Task<Tenant> UpdateTenantAsync(Tenant tenant);

        /// <summary>
        /// Soft deletes a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID to delete</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeleteTenantAsync(int tenantId);
    }
}


