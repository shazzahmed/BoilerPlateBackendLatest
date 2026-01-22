using Domain.Entities;
using Common.DTO.Request;
using Common.DTO.Response;

namespace Application.ServiceContracts
{
    public interface IPreAdmissionService : IBaseService<PreAdmissionModel, PreAdmission, int>
    {
        /// <summary>
        /// Get provisional fees for an application
        /// </summary>
        Task<BaseModel> GetProvisionalFees(int applicationId);
        
        /// <summary>
        /// Process provisional fee payment
        /// </summary>
        Task<BaseModel> ProcessProvisionalFeePayment(ProvisionalFeePaymentRequest request);
    }
}