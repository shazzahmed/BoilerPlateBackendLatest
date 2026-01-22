using System.Collections.Generic;
using System.Threading.Tasks;
using Common.DTO.Request;
using Domain.Entities;

namespace Domain.RepositoriesContracts
{
    public interface IFineWaiverRepository : IBaseRepository<FineWaiver, int>
    {
        Task<FineWaiver> RequestWaiverAsync(FineWaiverRequest request);
        Task<FineWaiver> ApproveWaiverAsync(FineWaiverApprovalRequest request);
        Task<List<FineWaiver>> GetPendingWaiversAsync();
        Task<List<FineWaiver>> GetWaiversByStudentAsync(int studentId);
        Task<List<FineWaiver>> GetWaiversByStatusAsync(string status);
    }
}
