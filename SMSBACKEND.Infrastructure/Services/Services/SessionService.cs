
using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;

namespace Infrastructure.Services.Services
{

    public class SessionService : BaseService<SessionModel, Session, int>, ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        
        public SessionService(
            IMapper mapper, 
            ISessionRepository sessionRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider
            ) : base(mapper, sessionRepository, unitOfWork, sseService, cacheProvider)
        {
            _sessionRepository = sessionRepository;
        }
        public async Task<int> GetActiveSessionId()
        {
            return await _sessionRepository.GetActiveSessionId();
        }
    }
}
