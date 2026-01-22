using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DTO.Request;
using Domain.Entities;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface IFineWaiverService : IBaseService<FineWaiverModel, FineWaiver, int>
    {
        Task<BaseModel> RequestFineWaiver(FineWaiverRequest request);
        Task<BaseModel> ApproveFineWaiver(FineWaiverApprovalRequest request);
        Task<BaseModel> GetPendingWaivers();
        Task<BaseModel> GetWaiversByStudent(int studentId);
        Task<BaseModel> GetWaiversByStatus(string status);
    }
}
