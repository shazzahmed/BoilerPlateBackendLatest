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

        public async Task<BaseModel<bool>> SendPaymentConfirmationAsync(int studentId, decimal paymentAmount, string paymentMethod, bool sendSMS = true, bool sendEmail = true)
        {
            try
            {
                _logger.LogInformation("💰 Sending payment confirmation for Student {StudentId}, Amount: {Amount}, Method: {Method}", 
                    studentId, paymentAmount, paymentMethod);

                // Get student details (using Find with includes)
                var student = await _context.Set<Student>()
                    .Include(s => s.StudentInfo)
                    .Include(s => s.StudentParent)
                    .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
                    
                if (student == null)
                {
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Student not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                var results = new List<bool>();

                // Send SMS if requested (using StudentInfo.MobileNo or StudentParent.GuardianPhone)
                var phoneNumber = student.StudentInfo?.MobileNo ?? student.StudentParent?.GuardianPhone;
                if (sendSMS && !string.IsNullOrWhiteSpace(phoneNumber))
                {
                    var smsTemplate = GetTemplate("PaymentConfirmation");
                    var smsMessage = FormatTemplate(smsTemplate.SMSMessage, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName ?? "Student" },
                        { "Amount", paymentAmount.ToString("C") },
                        { "PaymentMethod", paymentMethod },
                        { "Date", DateTime.Now.ToString("dd/MM/yyyy") }
                    });

                    var smsResult = await SendSMSAsync(phoneNumber, smsMessage, "PaymentConfirmation");
                    results.Add(smsResult.Success);
                }

                // Send Email if requested (using StudentInfo.Email or StudentParent.GuardianEmail)
                var email = student.StudentInfo?.Email ?? student.StudentParent?.GuardianEmail;
                if (sendEmail && !string.IsNullOrWhiteSpace(email))
                {
                    var emailTemplate = GetTemplate("PaymentConfirmation");
                    var subject = FormatTemplate(emailTemplate.Subject, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName ?? "Student" },
                        { "Amount", $"₨{paymentAmount:N2}" }  // ✅ Use Rupees symbol
                    });
                    var body = FormatTemplate(emailTemplate.Body, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName ?? "Student" },
                        { "Amount", $"₨{paymentAmount:N2}" },  // ✅ Use Rupees symbol
                        { "PaymentMethod", paymentMethod },
                        { "Date", DateTime.Now.ToString("dd/MM/yyyy HH:mm") },
                        { "ReceiptNumber", $"RCP-{DateTime.Now:yyyyMMdd}-{studentId:D4}" }
                    });

                    var emailResult = await SendEmailAsync(email, subject, body, "PaymentConfirmation");
                    results.Add(emailResult.Success);
                }

                var successCount = results.Count(r => r);
                var totalCount = results.Count;

                return new BaseModel<bool>
                {
                    Success = successCount > 0,
                    Data = successCount == totalCount,
                    Message = $"Payment confirmation sent: {successCount}/{totalCount} notifications delivered",
                    Total = successCount,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending payment confirmation for Student {StudentId}", studentId);
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send payment confirmation: {ex.Message}",
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

        public async Task<BaseModel<bool>> SendPaymentReceiptAsync(int studentId, string receiptNumber, object paymentDetails, bool sendSMS = true, bool sendEmail = true)
        {
            try
            {
                _logger.LogInformation("🧾 Sending payment receipt for Student {StudentId}, Receipt: {ReceiptNumber}", 
                    studentId, receiptNumber);

                // Get student details (using Find with includes)
                var student = await _context.Set<Student>()
                    .Include(s => s.StudentInfo)
                    .Include(s => s.StudentParent)
                    .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
                    
                if (student == null)
                {
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Student not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                var results = new List<bool>();

                // Send SMS if requested (using StudentInfo.MobileNo or StudentParent.GuardianPhone)
                var phoneNumber = student.StudentInfo?.MobileNo ?? student.StudentParent?.GuardianPhone;
                if (sendSMS && !string.IsNullOrWhiteSpace(phoneNumber))
                {
                    var smsTemplate = GetTemplate("PaymentReceipt");
                    var smsMessage = FormatTemplate(smsTemplate.SMSMessage, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName ?? "Student" },
                        { "ReceiptNumber", receiptNumber },
                        { "Date", DateTime.Now.ToString("dd/MM/yyyy") }
                    });

                    var smsResult = await SendSMSAsync(phoneNumber, smsMessage, "PaymentReceipt");
                    results.Add(smsResult.Success);
                }

                // Send Email if requested (using StudentInfo.Email or StudentParent.GuardianEmail)
                var email = student.StudentInfo?.Email ?? student.StudentParent?.GuardianEmail;
                if (sendEmail && !string.IsNullOrWhiteSpace(email))
                {
                    // Use database template
                    var placeholders = new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName ?? "Student" },
                        { "ReceiptNumber", receiptNumber },
                        { "PaymentDate", DateTime.Now.ToString("dd MMM yyyy") },
                        { "PaymentMethod", "N/A" }, // TODO: Pass from paymentDetails
                        { "Amount", paymentDetails?.ToString() ?? "0" }
                    };

                    var emailResult = await SendTemplateEmailAsync(
                        email, 
                        Common.Utilities.Enums.NotificationTemplates.EmailPaymentReceipt, 
                        placeholders);
                    results.Add(emailResult.Success);
                }

                var successCount = results.Count(r => r);
                var totalCount = results.Count;

                return new BaseModel<bool>
                {
                    Success = successCount > 0,
                    Data = successCount == totalCount,
                    Message = $"Payment receipt sent: {successCount}/{totalCount} notifications delivered",
                    Total = successCount,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending payment receipt for Student {StudentId}", studentId);
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send payment receipt: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel<bool>> SendOverdueReminderAsync(int studentId, decimal overdueAmount, int overdueDays, bool sendSMS = true, bool sendEmail = true)
        {
            try
            {
                _logger.LogInformation("⚠️ Sending overdue reminder for Student {StudentId}, Amount: {Amount}, Days: {Days}", 
                    studentId, overdueAmount, overdueDays);

                // Get student details (using Find with includes)
                var student = await _context.Set<Student>()
                    .Include(s => s.StudentInfo)
                    .Include(s => s.StudentParent)
                    .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
                    
                if (student == null)
                {
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Student not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                var results = new List<bool>();

                // Send SMS if requested (using StudentInfo.MobileNo or StudentParent.GuardianPhone)
                var phoneNumber = student.StudentInfo?.MobileNo ?? student.StudentParent?.GuardianPhone;
                if (sendSMS && !string.IsNullOrWhiteSpace(phoneNumber))
                {
                    var smsTemplate = GetTemplate("OverdueReminder");
                    var smsMessage = FormatTemplate(smsTemplate.SMSMessage, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName ?? "Student" },
                        { "Amount", overdueAmount.ToString("C") },
                        { "Days", overdueDays.ToString() }
                    });

                    var smsResult = await SendSMSAsync(phoneNumber, smsMessage, "OverdueReminder");
                    results.Add(smsResult.Success);
                }

                // Send Email if requested (using StudentInfo.Email or StudentParent.GuardianEmail)
                var email = student.StudentInfo?.Email ?? student.StudentParent?.GuardianEmail;
                if (sendEmail && !string.IsNullOrWhiteSpace(email))
                {
                    var emailTemplate = GetTemplate("OverdueReminder");
                    var subject = FormatTemplate(emailTemplate.Subject, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName },
                        { "Days", overdueDays.ToString() }
                    });
                    var body = FormatTemplate(emailTemplate.Body, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName },
                        { "Amount", overdueAmount.ToString("C") },
                        { "Days", overdueDays.ToString() },
                        { "DueDate", DateTime.Now.AddDays(-overdueDays).ToString("dd/MM/yyyy") }
                    });

                    var emailResult = await SendEmailAsync(student.StudentInfo.Email, subject, body, "OverdueReminder");
                    results.Add(emailResult.Success);
                }

                var successCount = results.Count(r => r);
                var totalCount = results.Count;

                return new BaseModel<bool>
                {
                    Success = successCount > 0,
                    Data = successCount == totalCount,
                    Message = $"Overdue reminder sent: {successCount}/{totalCount} notifications delivered",
                    Total = successCount,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending overdue reminder for Student {StudentId}", studentId);
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send overdue reminder: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
        }

        public async Task<BaseModel<bool>> SendFeeDueReminderAsync(int studentId, decimal dueAmount, DateTime dueDate, bool sendSMS = true, bool sendEmail = true)
        {
            try
            {
                _logger.LogInformation("📅 Sending fee due reminder for Student {StudentId}, Amount: {Amount}, Due: {DueDate}", 
                    studentId, dueAmount, dueDate);

                // Get student details (using Find with includes)
                var student = await _context.Set<Student>()
                    .Include(s => s.StudentInfo)
                    .Include(s => s.StudentParent)
                    .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
                    
                if (student == null)
                {
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Student not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                var results = new List<bool>();

                // Send SMS if requested (using StudentInfo.MobileNo or StudentParent.GuardianPhone)
                var phoneNumber = student.StudentInfo?.MobileNo ?? student.StudentParent?.GuardianPhone;
                if (sendSMS && !string.IsNullOrWhiteSpace(phoneNumber))
                {
                    var smsTemplate = GetTemplate("FeeDueReminder");
                    var smsMessage = FormatTemplate(smsTemplate.SMSMessage, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName ?? "Student" },
                        { "Amount", dueAmount.ToString("C") },
                        { "DueDate", dueDate.ToString("dd/MM/yyyy") }
                    });

                    var smsResult = await SendSMSAsync(phoneNumber, smsMessage, "FeeDueReminder");
                    results.Add(smsResult.Success);
                }

                // Send Email if requested (using StudentInfo.Email or StudentParent.GuardianEmail)
                var email = student.StudentInfo?.Email ?? student.StudentParent?.GuardianEmail;
                if (sendEmail && !string.IsNullOrWhiteSpace(email))
                {
                    var emailTemplate = GetTemplate("FeeDueReminder");
                    var subject = FormatTemplate(emailTemplate.Subject, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName },
                        { "DueDate", dueDate.ToString("dd/MM/yyyy") }
                    });
                    var body = FormatTemplate(emailTemplate.Body, new Dictionary<string, string>
                    {
                        { "StudentName", student.FullName },
                        { "Amount", dueAmount.ToString("C") },
                        { "DueDate", dueDate.ToString("dd/MM/yyyy") },
                        { "DaysRemaining", ((dueDate - DateTime.Now).Days).ToString() }
                    });

                    var emailResult = await SendEmailAsync(student.StudentInfo.Email, subject, body, "FeeDueReminder");
                    results.Add(emailResult.Success);
                }

                var successCount = results.Count(r => r);
                var totalCount = results.Count;

                return new BaseModel<bool>
                {
                    Success = successCount > 0,
                    Data = successCount == totalCount,
                    Message = $"Fee due reminder sent: {successCount}/{totalCount} notifications delivered",
                    Total = successCount,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending fee due reminder for Student {StudentId}", studentId);
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send fee due reminder: {ex.Message}",
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

        /// <summary>
        /// Send Fine Waiver status email (Approved or Rejected)
        /// </summary>
        public async Task<BaseModel<bool>> SendFineWaiverStatusEmailAsync(
            int studentId,
            bool isApproved,
            decimal waiverAmount,
            string approvedBy,
            string approvalNote)
        {
            try
            {
                _logger.LogInformation("📧 Sending fine waiver {Status} email for Student {StudentId}", 
                    isApproved ? "approval" : "rejection", studentId);

                // Get student details
                var student = await _context.Set<Student>()
                    .Include(s => s.StudentInfo)
                    .Include(s => s.StudentParent)
                    .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);

                if (student == null)
                {
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Student not found",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                var email = student.StudentInfo?.Email ?? student.StudentParent?.GuardianEmail;
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("⚠️ No email address found for Student {StudentId}", studentId);
                    return new BaseModel<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "No email address found for student",
                        Total = 0,
                        LastId = 0,
                        CorrelationId = Guid.NewGuid().ToString()
                    };
                }

                // Use database template based on approval status
                var templateId = isApproved 
                    ? Common.Utilities.Enums.NotificationTemplates.EmailFineWaiverApproved 
                    : Common.Utilities.Enums.NotificationTemplates.EmailFineWaiverRejected;

                var placeholders = new Dictionary<string, string>
                {
                    { "StudentName", student.FullName ?? "Student" },
                    { "WaiverAmount", waiverAmount.ToString("N0") },
                    { "ApprovedBy", approvedBy ?? "Admin" },
                    { "ApprovalDate", DateTime.Now.ToString("dd MMM yyyy") },
                    { "ApprovalNote", approvalNote ?? "No additional notes" }
                };

                return await SendTemplateEmailAsync(email, templateId, placeholders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error sending fine waiver email");
                return new BaseModel<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Failed to send fine waiver email: {ex.Message}",
                    Total = 0,
                    LastId = 0,
                    CorrelationId = Guid.NewGuid().ToString()
                };
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
                ["PaymentConfirmation"] = new NotificationTemplateDto
                {
                    TemplateType = "PaymentConfirmation",
                    Subject = "Payment Confirmation - {StudentName}",
                    Body = @"
Dear {StudentName},

Your payment of {Amount} has been received successfully.

Payment Details:
- Amount: {Amount}
- Payment Method: {PaymentMethod}
- Date: {Date}
- Receipt Number: {ReceiptNumber}

Thank you for your payment.

Best regards,
School Management System
                    ",
                    SMSMessage = "Payment of {Amount} received successfully for {StudentName} on {Date}. Receipt: {ReceiptNumber}"
                },
                ["PaymentReceipt"] = new NotificationTemplateDto
                {
                    TemplateType = "PaymentReceipt",
                    Subject = "Payment Receipt - {StudentName}",
                    Body = @"
Dear {StudentName},

Please find attached your payment receipt.

Receipt Details:
- Receipt Number: {ReceiptNumber}
- Date: {Date}
- Payment Details: {PaymentDetails}

Thank you for your payment.

Best regards,
School Management System
                    ",
                    SMSMessage = "Receipt {ReceiptNumber} generated for {StudentName} on {Date}. Check your email for details."
                },
                ["OverdueReminder"] = new NotificationTemplateDto
                {
                    TemplateType = "OverdueReminder",
                    Subject = "Overdue Fee Reminder - {StudentName}",
                    Body = @"
Dear {StudentName},

This is a reminder that you have an overdue fee payment.

Overdue Details:
- Amount: {Amount}
- Days Overdue: {Days}
- Original Due Date: {DueDate}

Please make the payment as soon as possible to avoid any additional charges.

Best regards,
School Management System
                    ",
                    SMSMessage = "Overdue fee reminder: {Amount} is {Days} days overdue for {StudentName}. Please pay immediately."
                },
                ["FeeDueReminder"] = new NotificationTemplateDto
                {
                    TemplateType = "FeeDueReminder",
                    Subject = "Fee Due Reminder - {StudentName}",
                    Body = @"
Dear {StudentName},

This is a reminder that you have a fee payment due soon.

Fee Details:
- Amount: {Amount}
- Due Date: {DueDate}
- Days Remaining: {DaysRemaining}

Please ensure payment is made before the due date.

Best regards,
School Management System
                    ",
                    SMSMessage = "Fee reminder: {Amount} is due on {DueDate} for {StudentName}. Please pay on time."
                }
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