using Application.ServiceContracts;
using Application.ServiceContracts.IServices;
using Common.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;

namespace BoilerPlateBackend.Presentation.Controllers
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

}
