using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface INotificationService
    {
        /// <summary>
        /// Send SMS notification
        /// </summary>
        /// <param name="phoneNumber">Recipient phone number</param>
        /// <param name="message">SMS message content</param>
        /// <param name="templateType">Type of notification template</param>
        /// <returns>Success status and message</returns>
        Task<BaseModel<bool>> SendSMSAsync(string phoneNumber, string message, string templateType = "General");

        /// <summary>
        /// Send Email notification
        /// </summary>
        /// <param name="emailAddress">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="templateType">Type of notification template</param>
        /// <param name="attachments">Optional email attachments</param>
        /// <returns>Success status and message</returns>
        Task<BaseModel<bool>> SendEmailAsync(string emailAddress, string subject, string body, string templateType = "General", List<EmailAttachment>? attachments = null);


        /// <summary>
        /// Get notification templates
        /// </summary>
        /// <param name="templateType">Type of template</param>
        /// <returns>Template content</returns>
        Task<BaseModel<NotificationTemplateDto>> GetTemplateAsync(string templateType);

        /// <summary>
        /// Send email using database template with placeholder replacement
        /// </summary>
        Task<BaseModel<bool>> SendTemplateEmailAsync(
            string emailAddress,
            Common.Utilities.Enums.NotificationTemplates templateId,
            Dictionary<string, string> placeholders);


        /// <summary>
        /// Test notification service connectivity
        /// </summary>
        /// <returns>Service status</returns>
        Task<BaseModel<bool>> TestConnectivityAsync();
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
    }

    /// <summary>
    /// DTO for notification templates (different from Domain.Entities.NotificationTemplate)
    /// </summary>
    public class NotificationTemplateDto
    {
        public string TemplateType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string SMSMessage { get; set; } = string.Empty;
        public Dictionary<string, string> Placeholders { get; set; } = new();
    }
}