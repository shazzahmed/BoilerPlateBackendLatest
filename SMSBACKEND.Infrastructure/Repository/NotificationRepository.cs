using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class NotificationRepository : BaseRepository<Notification, int>, INotificationRepository
    {
        public NotificationRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
