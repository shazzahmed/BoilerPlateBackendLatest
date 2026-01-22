using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Database
{
    public interface ISqlServerDbContext : IDisposable
    {
        DatabaseFacade Database { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        //int SaveChanges(CancellationToken cancellationToken = default(CancellationToken));


        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
       
        DbSet<ApplicationUser> User { get; set; }
        DbSet<Status> Statuses { get; set; }
        DbSet<StatusType> StatusTypes { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<Notification> Notifications { get; set; }
        DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        DbSet<NotificationType> NotificationTypes { get; set; }
        // automatic code generation area

        DbSet<Module> Modules { get; set; }
        DbSet<Permission> Permission { get; set; }
        DbSet<RolePermission> RolePermission { get; set; }
        DbSet<Tenant> Tenants { get; set; }

        // automatic code generation area end
    }
}

