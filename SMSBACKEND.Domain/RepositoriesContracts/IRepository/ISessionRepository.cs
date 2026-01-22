
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface ISessionRepository : IBaseRepository<Session, int>
    {
        Task<int> GetActiveSessionId();
    }
}
