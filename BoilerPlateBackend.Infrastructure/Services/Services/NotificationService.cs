using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Common.DTO.Response;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Infrastructure.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly SqlServerDbContext _context;
        private readonly Dictionary<string, NotificationTemplateDto> _templates;
        private readonly IEmailService _emailService;
        private readonly bool _isTestMode;
        private readonly string _testEmail;

        public NotificationService(
            ILogger<NotificationService> logger,
            IConfiguration configuration,
            SqlServerDbContext context,
            IEmailService emailService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
            _templates = InitializeTemplates();
            
            // Load test mode configuration
            _isTestMode = configuration.GetValue<bool>("Component:Communication:TestMode", false);
            _testEmail = configuration.GetValue<string>("Component:Communication:TestEmail", string.Empty);
            
            if (_isTestMode && !string.IsNullOrEmpty(_testEmail))
            {
                _logger.LogWarning("⚠️ EMAIL TEST MODE ENABLED - All emails will be sent to: {TestEmail}", _testEmail);
            }
        }

        public async Task<BaseModel<bool>> SendSMSAsync(string phoneNumber, string message, string templateType = "General")
        {
            try
            {
                _logger.LogInformation("📱 Sending SMS to {PhoneNumber} with template {TemplateType}", phoneNumber, templateType);

                // TODO: Integrate with actual SMS gateway (Twilio, AWS SNS, etc.)
                // For now, this is a placeholder implementation
                
                // Validate phone number format
                if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length < 10)
                {
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Invalid phone number format",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Simulate SMS sending delay
                await Task.Delay(100);

                // Log the SMS content (in production, this would be sent via SMS gateway)
                _logger.LogInformation("📱 SMS Content: {Message}", message);

                // TODO: Replace with actual SMS gateway call
                // Example: await _smsGateway.SendAsync(phoneNumber, message);

                return new BaseModel<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "SMS sent successfully (placeholder)",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending SMS to {PhoneNumber}", phoneNumber);
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send SMS: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel<bool>> SendEmailAsync(string emailAddress, string subject, string body, string templateType = "General", List<EmailAttachment>? attachments = null)
        {
            try
            {
                var originalEmail = emailAddress;
                
                // Redirect to test email if in test mode
                if (_isTestMode && !string.IsNullOrEmpty(_testEmail))
                {
                    _logger.LogWarning("🧪 TEST MODE: Redirecting email from {OriginalEmail} to {TestEmail}", 
                        emailAddress, _testEmail);
                    emailAddress = _testEmail;
                    
                    // Add original recipient to subject for tracking
                    subject = $"[TEST - Original: {originalEmail}] {subject}";
                }
                
                _logger.LogInformation("📧 Sending Email to {EmailAddress} with template {TemplateType}", emailAddress, templateType);

                // Validate email format
                if (string.IsNullOrWhiteSpace(emailAddress) || !emailAddress.Contains("@"))
                {
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Invalid email address format",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Log to notification queue
                var notification = new Domain.Entities.Notification
                {
                    NotificationTypeId = Common.Utilities.Enums.NotificationTypes.Email,
                    Recipient = emailAddress,
                    Subject = subject,
                    Message = body,
                    StatusId = 1, // Pending
                    Attempts = 0,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "System",
                    IsDeleted = false
                };

                await _context.Set<Domain.Entities.Notification>().AddAsync(notification);
                await _context.SaveChangesAsync();

                // Send using existing IEmailService (your infrastructure!)
                var success = await _emailService.SendEmail(
                    subject,
                    body,
                    emailAddress,
                    null, // fromEmail - will use default from config
                    null, // fromName - will use default from config
                    null  // attachment - handled separately
                );

                // Update notification status
                notification.StatusId = success ? 2 : 3; // 2=Success, 3=Failed
                notification.ProcessedAt = DateTime.Now;
                notification.Attempts++;
                notification.Result = success ? "Sent successfully" : "Failed to send";
                await _context.SaveChangesAsync();

                _logger.LogInformation(success 
                    ? "✅ Email sent successfully to {EmailAddress}" 
                    : "❌ Email failed to {EmailAddress}", emailAddress);

                return new BaseModel<bool>
                {
                    Success = success,
                    Data = success,
                    Message = success ? "Email sent successfully" : "Email sending failed",
                    Total = 1,
                    LastId = notification.Id,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending Email to {EmailAddress}", emailAddress);
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send email: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        /// <summary>
        /// Send email using database template with placeholder replacement
        /// </summary>
        public async Task<BaseModel<bool>> SendTemplateEmailAsync(
            string emailAddress,
            Common.Utilities.Enums.NotificationTemplates templateId,
            Dictionary<string, string> placeholders)
        {
            try
            {
                _logger.LogInformation("📧 Sending template email {TemplateId} to {EmailAddress}", templateId, emailAddress);

                // Get template from database
                var template = await _context.Set<Domain.Entities.NotificationTemplate>()
                    .FirstOrDefaultAsync(t => t.Id == templateId);

                if (template == null)
                {
                    _logger.LogWarning("❌ Template {TemplateId} not found", templateId);
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = $"Template {templateId} not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Replace placeholders in subject and body
                var subject = template.Subject;
                var body = template.MessageBody;

                foreach (var placeholder in placeholders)
                {
                    subject = subject.Replace($"{{{placeholder.Key}}}", placeholder.Value);
                    body = body.Replace($"{{{placeholder.Key}}}", placeholder.Value);
                }

                // Send using SendEmailAsync
                return await SendEmailAsync(emailAddress, subject, body, template.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending template email {TemplateId}", templateId);
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send template email: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public Task<BaseModel<NotificationTemplateDto>> GetTemplateAsync(string templateType)
        {
            try
            {
                var template = GetTemplate(templateType);
                return Task.FromResult(new BaseModel<NotificationTemplateDto>
                {
                    Success = true,
                    Data = template,
                    Message = "Template retrieved successfully",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting template {TemplateType}", templateType);
                return Task.FromResult(new BaseModel<NotificationTemplateDto>
                {
                    Success = false,
                    Data = new NotificationTemplateDto(),
                    Message = $"Failed to get template: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        public async Task<BaseModel<bool>> TestConnectivityAsync()
        {
            try
            {
                _logger.LogInformation("🔍 Testing notification service connectivity");

                // Test SMS connectivity (placeholder)
                var smsTest = await SendSMSAsync("+1234567890", "Test SMS", "Test");
                
                // Test Email connectivity using real service
                var emailTest = await SendEmailAsync("test@example.com", "Test Email", "<html><body>Test Body</body></html>", "Test");

                var success = smsTest.Success && emailTest.Success;

                return new BaseModel<bool>
                {
                    Success = success,
                    Data = success,
                    Message = success ? "All notification services are operational" : "Some notification services are not available",
                    Total = success ? 1 : 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error testing notification service connectivity");
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Connectivity test failed: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        private Dictionary<string, NotificationTemplateDto> InitializeTemplates()
        {
            return new Dictionary<string, NotificationTemplateDto>
            {
                // Add your custom notification templates here
                // Example:
                // ["Welcome"] = new NotificationTemplateDto
                // {
                //     TemplateType = "Welcome",
                //     Subject = "Welcome to {OrganizationName}",
                //     Body = "Welcome message body...",
                //     SMSMessage = "Welcome SMS message"
                // }
            };
        }

        private NotificationTemplateDto GetTemplate(string templateType)
        {
            return _templates.TryGetValue(templateType, out var template) 
                ? template 
                : new NotificationTemplateDto { TemplateType = templateType };
        }

        private string FormatTemplate(string template, Dictionary<string, string> placeholders)
        {
            var result = template;
            foreach (var placeholder in placeholders)
            {
                result = result.Replace($"{{{placeholder.Key}}}", placeholder.Value);
            }
            return result;
        }
    }
}