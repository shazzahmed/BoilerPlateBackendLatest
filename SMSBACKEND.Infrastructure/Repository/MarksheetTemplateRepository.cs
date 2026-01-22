using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Common.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services.Communication;
using Application.ServiceContracts;

namespace Infrastructure.Repository
{
    public class MarksheetTemplateRepository : BaseRepository<MarksheetTemplate, int>, IMarksheetTemplateRepository
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;

        public MarksheetTemplateRepository(
            IMapper mapper,
            ISqlServerDbContext context,
            ICacheProvider cacheProvider,
            SseService sseService) : base(mapper, context)
        {
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }

        public async Task<MarksheetTemplateModel> GetDefaultTemplateAsync()
        {
            var template = await DbContext.Set<MarksheetTemplate>()
                .Where(t => t.IsDefault && t.IsActive && !t.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return mapper.Map<MarksheetTemplateModel>(template);
        }

        public async Task<bool> SetDefaultTemplateAsync(int templateId)
        {
            // Remove default from all
            var allTemplates = await DbContext.Set<MarksheetTemplate>()
                .Where(t => !t.IsDeleted)
                .ToListAsync();

            foreach (var template in allTemplates)
            {
                template.IsDefault = (template.Id == templateId);
            }

            DbContext.Set<MarksheetTemplate>().UpdateRange(allTemplates);
            await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<MarksheetTemplateModel>> GetActiveTemplatesAsync()
        {
            var templates = await DbContext.Set<MarksheetTemplate>()
                .Where(t => t.IsActive && !t.IsDeleted)
                .OrderByDescending(t => t.IsDefault)
                .ThenBy(t => t.TemplateName)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<MarksheetTemplateModel>>(templates);
        }
    }
}


