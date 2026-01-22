using System.Security.Claims;
using System.Linq.Expressions;
using Domain.Entities;
using Common.DTO;
using Common.DTO.Response;
using Common.DTO.Request;

namespace Application.Interfaces
{
    public interface ISecurityService
    {
        Task<LoginResponse> CreateUser(RegisterUserModel model);

        //Reasoning: Since this project does't have knowlege about IdentityUser,IdentityUser<Tkey> hence object datatype is selected
        Task<AuthenticationResponse> CreateUser(object user, string password);
        Task<BaseModel> UpdateUserDetail(UserModel userInfo);

        Task<BaseModel> ConfirmEmail(string userId, string code);

        Task<LoginResponse> Login(string Email, string password, bool persistCookie = false);
        Task<BaseModel> GetUser(string userName);
        Task<BaseModel> GetUserDetail(string userId);

        Task<BaseModel> GetUsers();

        Task<BaseModel> GetUsers(Expression<Func<object, bool>> where);

        Task<BaseModel> GetUsers(Expression<Func<object, bool>> where = null, Func<IQueryable<object>, IOrderedQueryable<object>> orderBy = null, params Expression<Func<object, object>>[] includeProperties);

        Task<BaseModel> BlockUser(string userId);

        Task<AuthenticationResponse> AddUserClaim(string userId, string claimType, string claimValue);

        Task<BaseModel> GetUserClaim(string userId);

        Task<AuthenticationResponse> RemoveUserClaim(string userId, string claimType, string claimValue);

        Task<AuthenticationResponse> CreateRole(string role);

        Task<AuthenticationResponse> UpdateRole(string id, string role);

        Task<BaseModel> GetRole(string roleName);

        Task<BaseModel> GetRoles(bool isSystem = true);

        Task<AuthenticationResponse> RemoveRole(string roleName);

        Task<AuthenticationResponse> AddUserRole(string userId, IEnumerable<string> roles);

        Task<AuthenticationResponse> SetUserRoles(string userId, IEnumerable<string> roles);

        Task<BaseModel> GetUserRoles(string userId);

        Task<AuthenticationResponse> RemoveUserRole(string userId, string roleName);

        Task<AuthenticationResponse> RemoveUserRoles(string userId, IEnumerable<string> roles);

        Task<BaseModel> ForgotPassword(string email);

        Task<AuthenticationResponse> ResetPassword(string email, string code, string password);

        Task<AuthenticationResponse> ChangePassword(string userName, string currentPassword, string newPassword);

        Task<BaseModel> SetPassword(string userId, string newPassword);

        Task<BaseModel> ChangeEmail(string userId, string email, string code);
        Task<BaseModel> ChangePhoneNumber(string userId, string phoneNumber, string code);

        Task<BaseModel> RemovePhoneNumber(string userId);
        Task<JsonWebToken> RefreshAccessToken(string token);
        Task RevokeRefreshToken(string token);

        Task<BaseModel> Logout();
    }
}
