using Domain.Entities;
using Common.DTO.Response;
using static Common.Utilities.Enums;

namespace Application.ServiceContracts
{
    public interface INotificationTemplateService : IBaseService<NotificationTemplateModel, NotificationTemplate, int>
    {
        Task<NotificationTemplateModel> GetNotificationTemplate(NotificationTemplates notificationTemplates, NotificationTypes notificationTypes);
    }
}
