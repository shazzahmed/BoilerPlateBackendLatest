
namespace Application.ServiceContracts
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string subject, string content, string toEmail, string? fromEmail = null, string? fromName = null, string? attachment = null);
    }
}
