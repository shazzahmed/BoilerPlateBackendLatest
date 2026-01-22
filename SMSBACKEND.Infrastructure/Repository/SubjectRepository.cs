
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Response;
using Application.ServiceContracts;
using Infrastructure.Services.Communication;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class SubjectRepository : BaseRepository<Subject, int>, ISubjectRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;
        public SubjectRepository(IMapper mapper, ISqlServerDbContext context, ICacheProvider cacheProvider, SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }
    }
}
