
using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface ISessionService : IBaseService<SessionModel, Session, int>
    {
        Task<int> GetActiveSessionId();
    }
}
