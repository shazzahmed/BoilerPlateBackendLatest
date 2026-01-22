using Domain.Entities;
using Common.DTO.Response;

namespace Domain.RepositoriesContracts
{
    public interface IUserRepository : IBaseRepository<ApplicationUser, string>
    {
        Task<List<string>> GetAdminUserRoles();
        string GetRolesForUserById(string userId);
        string GetRolesForUserByEmail(string email);
        Task<bool> IsStudentExists(string email, string ParentId);
        Task<bool> IsEmailAlreadyExists(string email);
        Task<bool> IsCNICAlreadyExist(string CNIC, string role);
        Task<bool> IsPhoneAlreadyExists(string phone);
        Task<List<ApplicationUser>> GetUsersByRoles(List<string> userRoles);
        Task<bool> IsPasswordChangeRequired(string userId);
    }
}
