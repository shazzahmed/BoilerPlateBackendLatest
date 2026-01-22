using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using System;
using System.Collections.Generic;
using System.Text;
using Common.DTO.Response;using Common.DTO.Request;
using Application.ServiceContracts;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class NotificationTypeService : BaseService<NotificationTypeModel, NotificationType, int>, INotificationTypeService
    {
        private readonly INotificationTypeRepository notificationTypeRepository;

        public NotificationTypeService(
            IMapper mapper, 
            INotificationTypeRepository notificationTypeRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ,
            ILogger<NotificationTypeService> logger
            ) : base(mapper, notificationTypeRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            this.notificationTypeRepository = notificationTypeRepository;
        }
    }
}

