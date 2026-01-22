
namespace Application.ServiceContracts
{
    public interface ISmsService
    {
        Task<bool> SendSms();
    }
}
