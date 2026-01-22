using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Common.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;

namespace SMSBACKEND.Presentation.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Send SMS notification
        /// </summary>
        [HttpPost]
        [Route("SendSMS")]
        [ServiceFilter(typeof(ValidateModelState))]
        public async Task<IActionResult> SendSMS([FromBody] SendSMSRequest request)
        {
            try
            {
                var result = await _notificationService.SendSMSAsync(
                    request.PhoneNumber, 
                    request.Message, 
                    request.TemplateType);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] SendSMS error");
                return BadRequest(new BaseModel<bool> 
                { 
                    Success = false, 
                    Data = false, 
                    Message = $"Error sending SMS: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Send Email notification
        /// </summary>
        [HttpPost]
        [Route("SendEmail")]
        [ServiceFilter(typeof(ValidateModelState))]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                var result = await _notificationService.SendEmailAsync(
                    request.EmailAddress, 
                    request.Subject, 
                    request.Body, 
                    request.TemplateType, 
                    request.Attachments);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] SendEmail error");
                return BadRequest(new BaseModel<bool> 
                { 
                    Success = false, 
                    Data = false, 
                    Message = $"Error sending email: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Send payment confirmation notification
        /// </summary>
        [HttpPost]
        [Route("SendPaymentConfirmation")]
        [ServiceFilter(typeof(ValidateModelState))]
        public async Task<IActionResult> SendPaymentConfirmation([FromBody] PaymentConfirmationRequest request)
        {
            try
            {
                var result = await _notificationService.SendPaymentConfirmationAsync(
                    request.StudentId, 
                    request.PaymentAmount, 
                    request.PaymentMethod, 
                    request.SendSMS, 
                    request.SendEmail);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] SendPaymentConfirmation error");
                return BadRequest(new BaseModel<bool> 
                { 
                    Success = false, 
                    Data = false, 
                    Message = $"Error sending payment confirmation: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Send payment receipt notification
        /// </summary>
        [HttpPost]
        [Route("SendPaymentReceipt")]
        [ServiceFilter(typeof(ValidateModelState))]
        public async Task<IActionResult> SendPaymentReceipt([FromBody] PaymentReceiptRequest request)
        {
            try
            {
                var result = await _notificationService.SendPaymentReceiptAsync(
                    request.StudentId, 
                    request.ReceiptNumber, 
                    request.PaymentDetails, 
                    request.SendSMS, 
                    request.SendEmail);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] SendPaymentReceipt error");
                return BadRequest(new BaseModel<bool> 
                { 
                    Success = false, 
                    Data = false, 
                    Message = $"Error sending payment receipt: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Send overdue fee reminder
        /// </summary>
        [HttpPost]
        [Route("SendOverdueReminder")]
        [ServiceFilter(typeof(ValidateModelState))]
        public async Task<IActionResult> SendOverdueReminder([FromBody] OverdueReminderRequest request)
        {
            try
            {
                var result = await _notificationService.SendOverdueReminderAsync(
                    request.StudentId, 
                    request.OverdueAmount, 
                    request.OverdueDays, 
                    request.SendSMS, 
                    request.SendEmail);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] SendOverdueReminder error");
                return BadRequest(new BaseModel<bool> 
                { 
                    Success = false, 
                    Data = false, 
                    Message = $"Error sending overdue reminder: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Send fee due reminder
        /// </summary>
        [HttpPost]
        [Route("SendFeeDueReminder")]
        [ServiceFilter(typeof(ValidateModelState))]
        public async Task<IActionResult> SendFeeDueReminder([FromBody] FeeDueReminderRequest request)
        {
            try
            {
                var result = await _notificationService.SendFeeDueReminderAsync(
                    request.StudentId, 
                    request.DueAmount, 
                    request.DueDate, 
                    request.SendSMS, 
                    request.SendEmail);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] SendFeeDueReminder error");
                return BadRequest(new BaseModel<bool> 
                { 
                    Success = false, 
                    Data = false, 
                    Message = $"Error sending fee due reminder: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Get notification template
        /// </summary>
        [HttpGet]
        [Route("GetTemplate/{templateType}")]
        public async Task<IActionResult> GetTemplate(string templateType)
        {
            try
            {
                var result = await _notificationService.GetTemplateAsync(templateType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] GetTemplate error");
                return BadRequest(new BaseModel<NotificationTemplateDto> 
                { 
                    Success = false, 
                    Data = new NotificationTemplateDto(), 
                    Message = $"Error getting template: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Test notification service connectivity
        /// </summary>
        [HttpGet]
        [Route("TestConnectivity")]
        public async Task<IActionResult> TestConnectivity()
        {
            try
            {
                var result = await _notificationService.TestConnectivityAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] TestConnectivity error");
                return BadRequest(new BaseModel<bool> 
                { 
                    Success = false, 
                    Data = false, 
                    Message = $"Error testing connectivity: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Email payment receipt to student
        /// </summary>
        [HttpPost]
        [Route("EmailReceipt")]
        [Authorize]
        public async Task<IActionResult> EmailReceipt([FromBody] EmailReceiptRequest request)
        {
            try
            {
                _logger.LogInformation("📧 Emailing receipt {ReceiptNumber} to Student {StudentId}", 
                    request.ReceiptNumber, request.StudentId);

                var result = await _notificationService.SendPaymentReceiptAsync(
                    request.StudentId,
                    request.ReceiptNumber,
                    request.Amount ?? 0,
                    false, // No SMS
                    true); // Email only

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] EmailReceipt error");
                return BadRequest(new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Error emailing receipt: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }

        /// <summary>
        /// Test payment notification with sample data
        /// </summary>
        [HttpPost]
        [Route("TestPaymentNotification")]
        public async Task<IActionResult> TestPaymentNotification([FromBody] TestPaymentNotificationRequest request)
        {
            try
            {
                _logger.LogInformation("🧪 Testing payment notification for Student {StudentId}", request.StudentId);

                // Test SMS
                var smsResult = await _notificationService.SendSMSAsync(
                    request.PhoneNumber, 
                    $"Test payment notification for Student {request.StudentId}. Amount: {request.PaymentAmount:C}", 
                    "PaymentConfirmation");

                // Test Email
                var emailResult = await _notificationService.SendEmailAsync(
                    request.EmailAddress, 
                    "Test Payment Notification", 
                    $"This is a test payment notification for Student {request.StudentId}. Amount: {request.PaymentAmount:C}", 
                    "PaymentConfirmation");

                // Test Payment Confirmation
                var confirmationResult = await _notificationService.SendPaymentConfirmationAsync(
                    request.StudentId, 
                    request.PaymentAmount, 
                    request.PaymentMethod, 
                    true, 
                    true);

                var results = new
                {
                    SMS = smsResult,
                    Email = emailResult,
                    PaymentConfirmation = confirmationResult,
                    Summary = new
                    {
                        TotalTests = 3,
                        SuccessfulTests = (smsResult.Success ? 1 : 0) + (emailResult.Success ? 1 : 0) + (confirmationResult.Success ? 1 : 0),
                        Message = "Test completed - check logs for detailed results"
                    }
                };

                return Ok(new BaseModel<object>
                {
                    Success = true,
                    Data = results,
                    Message = "Notification test completed",
                    Total = 1,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotificationController] TestPaymentNotification error");
                return BadRequest(new BaseModel<object> 
                { 
                    Success = false, 
                    Data = null, 
                    Message = $"Error testing payment notification: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                });
            }
        }
    }

    // Request DTOs
    public class SendSMSRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TemplateType { get; set; } = "General";
    }

    public class SendEmailRequest
    {
        public string EmailAddress { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string TemplateType { get; set; } = "General";
        public List<EmailAttachment>? Attachments { get; set; }
    }

    public class PaymentConfirmationRequest
    {
        public int StudentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public bool SendSMS { get; set; } = true;
        public bool SendEmail { get; set; } = true;
    }

    public class PaymentReceiptRequest
    {
        public int StudentId { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public object PaymentDetails { get; set; } = new();
        public bool SendSMS { get; set; } = true;
        public bool SendEmail { get; set; } = true;
    }

    public class OverdueReminderRequest
    {
        public int StudentId { get; set; }
        public decimal OverdueAmount { get; set; }
        public int OverdueDays { get; set; }
        public bool SendSMS { get; set; } = true;
        public bool SendEmail { get; set; } = true;
    }

    public class FeeDueReminderRequest
    {
        public int StudentId { get; set; }
        public decimal DueAmount { get; set; }
        public DateTime DueDate { get; set; }
        public bool SendSMS { get; set; } = true;
        public bool SendEmail { get; set; } = true;
    }

    public class TestPaymentNotificationRequest
    {
        public int StudentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string PhoneNumber { get; set; } = "+1234567890";
        public string EmailAddress { get; set; } = "test@example.com";
    }

    public class EmailReceiptRequest
    {
        public int StudentId { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
    }
}
