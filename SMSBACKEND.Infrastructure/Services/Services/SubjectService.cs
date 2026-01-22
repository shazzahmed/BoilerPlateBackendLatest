using AutoMapper;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Common.DTO.Request;
using Application.ServiceContracts;
using Common.DTO.Response;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Services
{
    public class SubjectService : BaseService<SubjectModel, Subject, int>, ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        
        public SubjectService(
            IMapper mapper, 
            ISubjectRepository subjectRepository, 
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<SubjectService> logger
            ) : base(mapper, subjectRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _subjectRepository = subjectRepository;
        }
        public async Task CreateSubject(SubjectModel model)
        {
            var result = await Add(new SubjectModel { Name = model.Name, Code = model.Code, Type = model.Type });
        }
        public async Task UpdateSubject(SubjectModel model)
        {
            model.IsDeleted = false;
            await Update(model);
        }
    }
}
