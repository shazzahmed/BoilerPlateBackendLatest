using Common.DTO.Response;
using System.Threading.Tasks;

namespace Application.ServiceContracts
{
    public interface IEmailSender
    {
        Task<Task> SendEmailAsync(string email, string subject, string message);
        Task<Task> SendEmailByGmailAsync(SendEmailViewModel vm);
    }
}
