using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Common.DTO.Response;
using Common.DTO.Request;
using static Common.Utilities.Enums;
using ApplicationUser = Domain.Entities.ApplicationUser;
using ApplicationRole = Domain.Entities.ApplicationRole;
using System.Linq.Expressions;
using System.Security.Claims;
using Application.ServiceContracts;
using Application.Interfaces;
using Domain.RepositoriesContracts;
using Common.DTO;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services.Communication;
using Microsoft.Extensions.Logging;
using IdentityModel;
using Domain.Entities;

namespace Infrastructure.Services.Services
{
    public class UserService : BaseService<UserModel, ApplicationUser, string>, IUserService
    {
        private readonly IUserRepository _userRepository;
        protected readonly RoleManager<ApplicationRole> _roleManager;
        protected readonly UserManager<ApplicationUser> _applicationUserManager;
        private readonly ISecurityService _securityService;
        private readonly INotificationTemplateService _notificationTemplateService;
        private readonly ICommunicationService _communicationService;
        private readonly ICacheProvider _cacheProvider;
        public UserService(
            IMapper mapper,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> applicationUserManager,
            ISecurityService securityService,
            INotificationTemplateService notificationTemplateService,
            ICommunicationService communicationService,
            SseService sseService,
            ICacheProvider cacheProvider,
            ILogger<UserService> logger
            ) : base(mapper, userRepository, unitOfWork, sseService, cacheProvider, logger)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _applicationUserManager = applicationUserManager;

            _securityService = securityService;
            _notificationTemplateService = notificationTemplateService;
            _communicationService = communicationService;
            _cacheProvider = cacheProvider;
        }

        public async Task<BaseModel> GetAllUsersByRole(string role)
        {
            var result = await _applicationUserManager.GetUsersInRoleAsync(role);
            return new BaseModel { Success = true, Data = result };
        }
        public async Task<List<string>> GetAdminUserRoles()
        {
            return await _userRepository.GetAdminUserRoles();
        }
        public async Task<BaseModel> GetUsersByRoles(List<string> roles)
        {
            var result = await _userRepository.GetUsersByRoles(roles);
            return new BaseModel { Success = true, Data = result };
        }
        public async Task<List<StatusCount>> GetUserCountByRoleAsync()
        {
            var roles = _roleManager.Roles.Where(x=> x.IsSystem).ToList();
            var result = new List<StatusCount>();

            foreach (var role in roles)
            {
                var usersInRole = await _applicationUserManager.GetUsersInRoleAsync(role.Name);
                result.Add(new StatusCount
                {
                    Status = role.Name,
                    Count = usersInRole.Count
                });
            }

            return result;
        }
        public async Task<bool> IsEmailAlreadyExist(string email)
        {
            return await _userRepository.IsEmailAlreadyExists(email);
        }
        public async Task<bool> IsStudentExist(string email, string ParentId)
        {
            return await _userRepository.IsStudentExists(email, ParentId);
        }
        public async Task<bool> IsCNICAlreadyExist(string CNIC, string role)
        {
            return await _userRepository.IsCNICAlreadyExist(CNIC, role);
        }
        public async Task<bool> IsPhoneAlreadyExist(string phone)
        {
            return await _userRepository.IsPhoneAlreadyExists(phone);
        }

        public async Task<ApplicationUserModel> GetUserByPhoneAndName(string phoneNumber, string firstName, string lastName)
        {
            var user = await _applicationUserManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && 
                                         u.FirstName == firstName && 
                                         u.LastName == lastName &&
                                         u.IsActive);
            
            if (user == null) return null;
            
            return mapper.Map<ApplicationUserModel>(user);
        }
        public string GetRolesForUserById(string userId)
        {
            return _userRepository.GetRolesForUserById(userId);
        }
        public string GetRolesForUserByEmail(string email)
        {
            return _userRepository.GetRolesForUserByEmail(email);
        }
        public async Task<(List<ModuleModel>, List<string> permissions)> GetUserModulesWithPermissionsAsync(string userId)
        {
            var cacheKey = $"user:modules:{userId}";
            return await _cacheProvider.GetOrCreateCacheAsync(
            cacheKey,
            typeof(RolePermission).Name,
            async () =>
            {
                var user = await _applicationUserManager.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .ThenInclude(p => p.Module)
                    .ThenInclude(c => c.Children)
                    .ThenInclude(cp => cp.Permissions)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return new();
                var permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList();
                var modules = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .GroupBy(rp => rp.Permission.Module)
                    .Select(g => new ModuleModel
                    {
                        Id = g.Key.Id,
                        Name = g.Key.Name,
                        Path = g.Key.Path,
                        Icon = g.Key.Icon,
                        IsOpen = g.Key.IsOpen,
                        IsDashboard = g.Key.IsDashboard,
                        IsActive = g.Key.IsActive,
                        ParentId = g.Key.ParentId,
                        OrderById = g.Key.OrderById,
                        Description = g.Key.Description,
                        Type = g.Key.Type,
                        Permissions = g
                        .Select(x => new PermissionModel
                        {
                            Id = x.Permission.Id,
                            Name = x.Permission.Name.Split('_').First()
                        })
                        .DistinctBy(p => p.Name) // optional: remove duplicates
                        .ToList(),
                        Children = g.Key.Children
                        .Select(child => new ModuleModel
                        {
                            Id = child.Id,
                            Name = child.Name,
                            Path = child.Path,
                            Icon = child.Icon,
                            IsOpen = child.IsOpen,
                            IsDashboard = child.IsDashboard,
                            IsActive = child.IsActive,
                            ParentId = child.ParentId,
                            OrderById = child.OrderById,
                            Description = child.Description,
                            Type = child.Type,
                            Permissions = user.UserRoles
                                .SelectMany(ur => ur.Role.RolePermissions)
                                .Where(rp => rp.Permission.ModuleId == child.Id)
                                .Select(rp => new PermissionModel
                                {
                                    Id = rp.Permission.Id,
                                    Name = rp.Permission.Name.Split('_').First()
                                })
                                .DistinctBy(p => p.Name)
                                .ToList(),
                        }).OrderBy(x => x.OrderById)
                        .Where(child => child.Permissions.Any())
                        .ToList()
                    })
                    .ToList();
                return (modules, permissions);
            },
            CancellationToken.None
            );
            
        }


        #region User

        public async Task<LoginResponse> CreateUser(RegisterUserModel model)
        {
            var result = await _securityService.CreateUser(model);
            if (result.Status == LoginStatus.Succeded.ToString())
            {
                //// ToDo: Check how it works in SingleSignOn
                
            }
            return result;
            //return new BaseModel { Success = true, Message = result.Message, Data = result.Data };

        }
        public async Task<AuthenticationResponse> CreateUser(object user, string password)
        {
            return await _securityService.CreateUser(user, password);
        }


        public async Task<BaseModel> UpdateUserDetail(UserModel userInfo)
        {
            return await _securityService.UpdateUserDetail(userInfo);
        }

        public async Task<BaseModel> GetUser(string userName)
        {
            return await _securityService.GetUser(userName);
        }

        public async Task<BaseModel> GetUserDetail(string userId)
        {
            return await _securityService.GetUserDetail(userId);
        }

        public async Task<BaseModel> GetUsers()
        {
            return await _securityService.GetUsers();
        }

        public async Task<BaseModel> GetUsers(Expression<Func<object, bool>> where)
        {
            return await _securityService.GetUsers(null, null);
        }

        public async Task<BaseModel> GetUsers(Expression<Func<object, bool>> where = null, Func<IQueryable<object>, IOrderedQueryable<object>> orderBy = null, params Expression<Func<object, object>>[] includeProperties)
        {
            return await _securityService.GetUsers(null, null);
        }

        public async Task<BaseModel> ForgotPassword(string email)
        {
            var response = await _securityService.ForgotPassword(email);
            Common.DTO.Response.ForgotPasswordModel forgotPasswordModel = (Common.DTO.Response.ForgotPasswordModel)response.Data;
            if (forgotPasswordModel != null)
            {
                var template = await _notificationTemplateService.GetNotificationTemplate(NotificationTemplates.EmailForgotPassword, NotificationTypes.Email);
                var emailMessage = template.MessageBody.Replace("#Name", $"{forgotPasswordModel.FirstName} {forgotPasswordModel.LastName}")
                                                       .Replace("#Link", $"{forgotPasswordModel.Link}");
                var sent = await _communicationService.SendEmail(template.Subject, emailMessage, forgotPasswordModel.Email);
            }
            return response;

        }

        public async Task<AuthenticationResponse> ResetPassword(string email, string code, string password)
        {
            return await _securityService.ResetPassword(email, code, password);
        }

        public async Task<AuthenticationResponse> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            return await _securityService.ChangePassword(userName, currentPassword, newPassword);
        }

        public async Task<BaseModel> SetPassword(string userId, string newPassword)
        {
            return await _securityService.SetPassword(userId, newPassword);
        }

        public async Task<BaseModel> ChangeEmail(string userId, string email, string code)
        {
            return await _securityService.ChangeEmail(userId, email, code);
        }

        public async Task<BaseModel> RemovePhoneNumber(string userId)
        {
            return await _securityService.RemovePhoneNumber(userId);
        }

        public async Task<BaseModel> ChangePhoneNumber(string userId, string phoneNumber, string code)
        {
            return await _securityService.ChangePhoneNumber(userId, phoneNumber, code);
        }

        public async Task<BaseModel> ConfirmEmail(string userId, string code)
        {
            return await _securityService.ConfirmEmail(userId, code);
        }

        public async Task<BaseModel> BlockUser(string userId)
        {
            return await _securityService.BlockUser(userId);
        }

        #endregion

        #region Login

        public async Task<LoginResponse> Login(string Email, string password, bool persistCookie = false)
        {
            return await _securityService.Login(Email, password, persistCookie);
        }

        public async Task<BaseModel> Logout()
        {
            return await _securityService.Logout();

        }
        
        #endregion

        #region Token
       
        public async Task<AuthenticationResponse> AddUserClaim(string userId, string claimType, string claimValue)
        {
            return await _securityService.AddUserClaim(userId, claimType, claimValue);
        }

        public async Task<BaseModel> GetUserClaim(string userId)
        {
            return await _securityService.GetUserClaim(userId);
        }

        public async Task<AuthenticationResponse> RemoveUserClaim(string userId, string claimType, string claimValue)
        {
            return await _securityService.RemoveUserClaim(userId, claimType, claimValue);
        }
        public async Task<JsonWebToken> RefreshAccessToken(string token)
        {
            return await _securityService.RefreshAccessToken(token);
        }
        public async Task RevokeRefreshToken(string token)
        {
            await _securityService.RevokeRefreshToken(token);
        }

        #endregion

        #region Roles

        public async Task<AuthenticationResponse> CreateRole(string role)
        {
            return await _securityService.CreateRole(role);
        }

        public async Task<AuthenticationResponse> AddUserRole(string userId, IEnumerable<string> roles)
        {
            return await _securityService.AddUserRole(userId, roles);
        }

        public async Task<AuthenticationResponse> SetUserRoles(string userId, IEnumerable<string> roles)
        {
            return await _securityService.SetUserRoles(userId, roles);
        }

        public async Task<AuthenticationResponse> UpdateRole(string id, string role)
        {
            return await _securityService.UpdateRole(id, role);
        }

        public async Task<BaseModel> GetRole(string roleName)
        {
            return await _securityService.GetRole(roleName);
        }

        public async Task<BaseModel> GetRoles(bool isSystem = true)
        {
            return await _securityService.GetRoles(isSystem);
        }

        public async Task<BaseModel> GetUserRoles(string userId)
        {
            return await _securityService.GetUserRoles(userId);
        }

        public async Task<AuthenticationResponse> RemoveRole(string roleName)
        {
            return await _securityService.RemoveRole(roleName);
        }

        public async Task<AuthenticationResponse> RemoveUserRole(string userId, string roleName)
        {
            return await _securityService.RemoveUserRole(userId, roleName);
        }

        public async Task<AuthenticationResponse> RemoveUserRoles(string userId, IEnumerable<string> roles)
        {
            return await _securityService.RemoveUserRoles(userId, roles);
        }

        #endregion
    }
}

