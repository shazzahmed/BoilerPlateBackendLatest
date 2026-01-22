using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface IMarksheetTemplateService : IBaseService<MarksheetTemplateModel, MarksheetTemplate, int>
    {
        Task<BaseModel> GetDefaultTemplateAsync();
        Task<BaseModel> SetDefaultTemplateAsync(int templateId);
        Task<BaseModel> GetActiveTemplatesAsync();
    }
}


