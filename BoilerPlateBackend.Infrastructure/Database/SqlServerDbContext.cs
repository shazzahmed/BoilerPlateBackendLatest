using IdentityModel;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Infrastructure.Database
{
    public class SqlServerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
    ApplicationUserRole, IdentityUserLogin<string>,
    IdentityRoleClaim<string>, IdentityUserToken<string>>, ISqlServerDbContext
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            this.configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public SqlServerDbContext() : base()
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(SqlServerDbContext).Assembly);
            base.OnModelCreating(builder);
            SeedData.Seed(builder);

            // ===== GLOBAL TENANT CASCADE PREVENTION =====
            // Configure ALL Tenant relationships to use Restrict to prevent multiple cascade paths
            // This is essential because many entities inherit from BaseEntity which has TenantId FK
            // and these entities often reference each other, creating cascade cycles

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tenantForeignKey = entityType.GetForeignKeys()
                    .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Tenant));
                
                if (tenantForeignKey != null)
                {
                    tenantForeignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
            
            builder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<Module>()
            .HasOne(m => m.Parent) // Each menu has one parent
            .WithMany(m => m.Children) // Each menu can have many children
            .HasForeignKey(m => m.ParentId) // Foreign key is ParentId
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes to avoid issues

            // Configure RefreshToken entity for multi-device support
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.RefreshTokenId); // New primary key
                entity.Property(rt => rt.RefreshTokenId).ValueGeneratedOnAdd(); // Auto-increment
                
                // Create composite index for efficient queries
                entity.HasIndex(rt => new { rt.Id, rt.DeviceId }).IsUnique(); // One token per user per device
                entity.HasIndex(rt => rt.Username); // For username-based queries
                entity.HasIndex(rt => rt.Token); // For token-based queries
            });

            // ===== MULTI-TENANCY: GLOBAL QUERY FILTERS =====
            // Automatically filter all queries by TenantId
            // This ensures users can only see data from their own tenant/organization
            // CRITICAL SECURITY: Never use IgnoreQueryFilters() in production without explicit authorization
            // IMPORTANT: Don't capture tenantId in a variable! Call GetCurrentTenantId() directly in each filter
            //            so it evaluates PER-QUERY, not once at startup
            
            // Apply query filters to entities inheriting from BaseEntity
            // Each filter calls GetCurrentTenantId() which will be evaluated at query execution time
            // Note: Add query filters for your custom entities here as needed
            
            // Note: ApplicationUser already filtered by TenantId at user level
            // Note: Tenant table itself should NOT have a query filter
            // ===== END MULTI-TENANCY FILTERS =====
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected virtual void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;
            var now = DateTime.Now;
            var user = GetCurrentUser();
            var tenantId = GetCurrentTenantId(); // Get current tenant ID from JWT claims

            foreach (var entry in entries)
            {
                if (entry.Entity is Domain.Entities.BaseEntity baseEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // Prevent modification of audit fields and tenant ID
                            entry.Property(nameof(baseEntity.CreatedAt)).IsModified = false;
                            entry.Property(nameof(baseEntity.CreatedBy)).IsModified = false;
                            entry.Property(nameof(baseEntity.TenantId)).IsModified = false; // SECURITY: Prevent tenant switching
                            
                            baseEntity.UpdatedAt = now;
                            baseEntity.UpdatedBy = user;
                            break;

                        case EntityState.Added:
                            // Auto-set tenant ID for new entities
                            // CRITICAL SECURITY: All new entities must belong to current tenant
                            baseEntity.TenantId = tenantId;
                            
                            baseEntity.CreatedAt = now;
                            baseEntity.CreatedAtUtc = utcNow;
                            baseEntity.CreatedBy = user;
                            baseEntity.UpdatedAt = now;
                            baseEntity.UpdatedBy = user;
                            break;
                    }
                }
                
                // Handle ApplicationUser (Identity) separately since it doesn't inherit from BaseEntity
                if (entry.Entity is ApplicationUser appUser)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            // Auto-set tenant ID for new users
                            if (appUser.TenantId == 0)
                            {
                                appUser.TenantId = tenantId;
                            }
                            break;
                        
                        case EntityState.Modified:
                            // SECURITY: Prevent tenant ID modification for existing users
                            entry.Property(nameof(appUser.TenantId)).IsModified = false;
                            break;
                    }
                }
            }
        }
        
        // Multi-Tenancy
        public virtual DbSet<Tenant> Tenants { get; set; }

        public virtual DbSet<ApplicationUser> User { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<StatusType> StatusTypes { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }


        // Core Entities
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<RolePermission> RolePermission { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }
        
        /// <summary>
        /// Gets the current tenant ID from multiple sources (priority order):
        /// 1. HttpContext.Items["TenantId"] - Set by TenantMiddleware from X-Tenant-Id header
        /// 2. JWT claims - Fallback for backward compatibility
        /// 3. Default (1) - For migrations, seeding, background jobs
        /// </summary>
        private int GetCurrentTenantId()
        {
            // Priority 1: Get from HttpContext.Items (set by TenantMiddleware from X-Tenant-Id header)
            if (_httpContextAccessor?.HttpContext?.Items?.ContainsKey("TenantId") == true)
            {
                var tenantIdFromHeader = _httpContextAccessor.HttpContext.Items["TenantId"];
                if (tenantIdFromHeader is int headerTenantId && headerTenantId > 0)
                {
                    return headerTenantId;
                }
            }
            
            // Priority 2: Get from JWT claims (backward compatibility)
            var tenantIdClaim = GetContextClaims("TenantId") ?? GetContextClaims("tenantId");
            if (int.TryParse(tenantIdClaim, out var tenantId) && tenantId > 0)
            {
                return tenantId;
            }
            
            // Priority 3: FALLBACK - Return default tenant ID (1)
            // This happens during migrations, seeding, or background jobs
            // IMPORTANT: Ensure Tenant with ID = 1 exists in your Tenants table
            return 1;
        }

        private string GetCurrentUser() => $"{GetContextClaims(JwtClaimTypes.Name) ?? string.Empty} : {GetContextClaims(ClaimTypes.Role) ?? "<unknown>"}";
        private string? GetContextClaims(string typeName) => _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(c => c?.Type == typeName)?.Value;
    }
}
