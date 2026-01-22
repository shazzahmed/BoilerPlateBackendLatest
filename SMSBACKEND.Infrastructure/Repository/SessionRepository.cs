
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class SessionRepository : BaseRepository<Session, int>, ISessionRepository
    {
        public SessionRepository(ISqlServerDbContext context) : base(context)
        {
        }
        public async Task<int> GetActiveSessionId()
        {
            try
            {
                var activeSession = await DbContext.Session
                    .Where(s => s.IsActive && !s.IsDeleted)
                    .OrderByDescending(s => s.StartDate).FirstOrDefaultAsync();

                return activeSession?.Id ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting active session: {ex.Message}");
                return 0;
            }
        }
    }
}
