using Application.ServiceContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Options;
using Infrastructure.Services.Communication;

namespace Infrastructure.Services.Communication
{
    public static class Communications
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ICommunicationService, CommunicationService>();
            services.AddTransient<IEmailSender, EmailSender>();
            var componentOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<ComponentOptions>>();
            switch (componentOptions?.Value?.Communication?.EmailService)
            {
                case "Google":
                    //services.AddTransient<IEmailService, EmailServiceGoogle>();
                    services.AddTransient<IEmailService, EmailServicePlesk>();
                    break;
                case "Outlook":
                    services.AddTransient<IEmailService, EmailServiceOutlook>();
                    break;
                case "Zoho":
                    services.AddTransient<IEmailService, EmailServiceZoho>();
                    break;
                default:
                    break;
            }
            services.AddTransient<ISmsService, SmsServiceTest>();
        }
    }
}
