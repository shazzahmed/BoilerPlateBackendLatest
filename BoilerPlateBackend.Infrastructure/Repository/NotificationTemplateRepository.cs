using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;

namespace Infrastructure.Repository
{
    public class NotificationTemplateRepository : BaseRepository<NotificationTemplate, int>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository(ISqlServerDbContext context) : base(context)
        {
        }
    }
}
