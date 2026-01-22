using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface INotificationTypeService : IBaseService<NotificationTypeModel, NotificationType, int>
    {
    }
}
