using Common.DTO.Response;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IMarksheetTemplateRepository : IBaseRepository<MarksheetTemplate, int>
    {
        Task<MarksheetTemplateModel> GetDefaultTemplateAsync();
        Task<bool> SetDefaultTemplateAsync(int templateId);
        Task<List<MarksheetTemplateModel>> GetActiveTemplatesAsync();
    }
}


