
using System.Security.Claims;
using System.Linq.Expressions;
using Common.DTO.Response;
using ApplicationUser = Domain.Entities.ApplicationUser;
using Common.DTO.Request;
using Common.DTO;

namespace Application.ServiceContracts
{
    public interface IUserService : IBaseService<UserModel, ApplicationUser, string>
    {
        //Task<BaseModel> CreateUser(RegisterUserModel model);
        Task<BaseModel> GetAllUsersByRole(string role);
        Task<List<string>> GetAdminUserRoles();
        Task<BaseModel> GetUsersByRoles(List<string> roles);
        Task<List<StatusCount>> GetUserCountByRoleAsync();
        Task<bool> IsEmailAlreadyExist(string email);
        Task<bool> IsStudentExist(string email, string ParentId);
        Task<bool> IsCNICAlreadyExist(string CNIC, string role);
        Task<bool> IsPhoneAlreadyExist(string phone);
        Task<ApplicationUserModel> GetUserByPhoneAndName(string phoneNumber, string firstName, string lastName);
        string GetRolesForUserById(string userId);
        string GetRolesForUserByEmail(string email);
        Task<(List<ModuleModel>, List<string> permissions)> GetUserModulesWithPermissionsAsync(string userId);

        #region User

        Task<LoginResponse> CreateUser(RegisterUserModel model);
        Task<AuthenticationResponse> CreateUser(object user, string password);
        Task<BaseModel> UpdateUserDetail(UserModel userInfo);
        Task<BaseModel> GetUser(string userName);
        Task<BaseModel> GetUserDetail(string userId);
        Task<BaseModel> GetUsers();
        Task<BaseModel> GetUsers(Expression<Func<object, bool>> where);
        Task<BaseModel> GetUsers(Expression<Func<object, bool>> where = null, Func<IQueryable<object>, IOrderedQueryable<object>> orderBy = null, params Expression<Func<object, object>>[] includeProperties);
        Task<BaseModel> ForgotPassword(string email);
        Task<AuthenticationResponse> ResetPassword(string email, string code, string password);
        Task<AuthenticationResponse> ChangePassword(string userName, string currentPassword, string newPassword);
        Task<BaseModel> SetPassword(string userId, string newPassword);
        Task<BaseModel> ChangeEmail(string userId, string email, string code);
        Task<BaseModel> RemovePhoneNumber(string userId);
        Task<BaseModel> ChangePhoneNumber(string userId, string phoneNumber, string code);
        Task<BaseModel> ConfirmEmail(string userId, string code);
        Task<BaseModel> BlockUser(string userId);

        #endregion

        #region Login
        Task<LoginResponse> Login(string Email, string password, bool persistCookie = false);
        Task<BaseModel> Logout();

        #endregion

        #region Token

        Task<AuthenticationResponse> AddUserClaim(string userId, string claimType, string claimValue);
        Task<BaseModel> GetUserClaim(string userId);
        Task<AuthenticationResponse> RemoveUserClaim(string userId, string claimType, string claimValue);
        Task<JsonWebToken> RefreshAccessToken(string token);
        Task RevokeRefreshToken(string token);
        #endregion

        #region Roles
        Task<AuthenticationResponse> CreateRole(string role);
        Task<AuthenticationResponse> AddUserRole(string userId, IEnumerable<string> roles);
        Task<AuthenticationResponse> SetUserRoles(string userId, IEnumerable<string> roles);
        Task<AuthenticationResponse> UpdateRole(string id, string role);
        Task<BaseModel> GetRole(string roleName);
        Task<BaseModel> GetRoles(bool isSystem = true);
        Task<BaseModel> GetUserRoles(string userId);
        Task<AuthenticationResponse> RemoveRole(string roleName);
        Task<AuthenticationResponse> RemoveUserRole(string userId, string roleName);
        Task<AuthenticationResponse> RemoveUserRoles(string userId, IEnumerable<string> roles);

        #endregion
    }
}
