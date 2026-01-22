using AutoMapper;
using Common.Utilities;
using Domain.Entities;
using Domain.RepositoriesContracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.DTO.Response;
using Common.DTO.Request;
using static Common.Utilities.Enums;
using Application.ServiceContracts;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using NotificationTemplate = Domain.Entities.NotificationTemplate;

namespace Infrastructure.Services.Services
{
    public class NotificationTemplateService : BaseService<NotificationTemplateModel, NotificationTemplate, int>, INotificationTemplateService
    {
        private readonly INotificationTemplateRepository _notificationTemplateRepository;

        public NotificationTemplateService(
            IMapper mapper, 
            INotificationTemplateRepository notificationTemplateRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<NotificationTemplateService> logger
            ) : base(mapper, notificationTemplateRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _notificationTemplateRepository = notificationTemplateRepository;
        }

        public async Task<NotificationTemplateModel> GetNotificationTemplate(NotificationTemplates notificationTemplates, NotificationTypes notificationTypes)
        {
            var template = await _notificationTemplateRepository.FirstOrDefaultAsync(x => x.Id == notificationTemplates && x.NotificationTypeId == notificationTypes);
            return mapper.Map<NotificationTemplate, NotificationTemplateModel>(template);
        }
    }
}

