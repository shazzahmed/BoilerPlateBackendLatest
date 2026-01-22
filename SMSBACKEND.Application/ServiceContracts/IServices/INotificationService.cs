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
        /// Send payment confirmation notification
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="paymentAmount">Amount paid</param>
        /// <param name="paymentMethod">Payment method used</param>
        /// <param name="sendSMS">Whether to send SMS</param>
        /// <param name="sendEmail">Whether to send Email</param>
        /// <returns>Success status and message</returns>
        Task<BaseModel<bool>> SendPaymentConfirmationAsync(int studentId, decimal paymentAmount, string paymentMethod, bool sendSMS = true, bool sendEmail = true);

        /// <summary>
        /// Send payment receipt notification
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="receiptNumber">Receipt number</param>
        /// <param name="paymentDetails">Payment details</param>
        /// <param name="sendSMS">Whether to send SMS</param>
        /// <param name="sendEmail">Whether to send Email</param>
        /// <returns>Success status and message</returns>
        Task<BaseModel<bool>> SendPaymentReceiptAsync(int studentId, string receiptNumber, object paymentDetails, bool sendSMS = true, bool sendEmail = true);

        /// <summary>
        /// Send overdue fee reminder
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="overdueAmount">Overdue amount</param>
        /// <param name="overdueDays">Number of days overdue</param>
        /// <param name="sendSMS">Whether to send SMS</param>
        /// <param name="sendEmail">Whether to send Email</param>
        /// <returns>Success status and message</returns>
        Task<BaseModel<bool>> SendOverdueReminderAsync(int studentId, decimal overdueAmount, int overdueDays, bool sendSMS = true, bool sendEmail = true);

        /// <summary>
        /// Send fee due reminder
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="dueAmount">Amount due</param>
        /// <param name="dueDate">Due date</param>
        /// <param name="sendSMS">Whether to send SMS</param>
        /// <param name="sendEmail">Whether to send Email</param>
        /// <returns>Success status and message</returns>
        Task<BaseModel<bool>> SendFeeDueReminderAsync(int studentId, decimal dueAmount, DateTime dueDate, bool sendSMS = true, bool sendEmail = true);

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
        /// Send Fine Waiver status email (Approved or Rejected)
        /// </summary>
        Task<BaseModel<bool>> SendFineWaiverStatusEmailAsync(
            int studentId,
            bool isApproved,
            decimal waiverAmount,
            string approvedBy,
            string approvalNote);

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