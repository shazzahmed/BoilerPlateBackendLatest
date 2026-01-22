using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class NotificationTypeRepository : BaseRepository<NotificationType, int>, INotificationTypeRepository
    {
        public NotificationTypeRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
