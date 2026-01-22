
using Domain.Entities;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.ServiceContracts
{
    public interface IStaffService : IBaseService<StaffModel, Staff, int>
    {
        Task<BaseModel> GetAllStaff(StaffRequest request);
        Task<BaseModel> CreateStaff(StaffCreateReq model);
        Task<BaseModel> UpdateStaff(StaffCreateReq model);
        Task<BaseModel> DeleteStaff(int id);
    }
}
