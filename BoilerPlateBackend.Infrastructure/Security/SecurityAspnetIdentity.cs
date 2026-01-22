using Infrastructure.Database;
using Domain.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;
using Common.Helpers;
using Common.DTO.Response;
using Common.DTO.Request;
using Common.Options;
using Common.Utilities;
using ApplicationUser = Domain.Entities.ApplicationUser;
using ApplicationRole = Domain.Entities.ApplicationRole;
using Domain.RepositoriesContracts;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.Middleware;
using Application.Interfaces;
using static Common.Utilities.Enums;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Infrastructure.Security
{
    public class SecurityAspnetIdentity : ISecurityService
    {
        private readonly AppOptions _AppOptions;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly RoleManager<ApplicationRole> _roleManager;
        protected readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtHandler _jwtHandler;
        protected readonly ISqlServerDbContext _dbContext;
        private readonly UrlEncoder _urlEncoder;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly ILogger<SecurityAspnetIdentity> _logger;
        private readonly IMapper _mapper;
        private string clientId = string.Empty;
        private string clientSecret = string.Empty;
        private string authenticatorUriFormat = string.Empty;
        private int numberOfRecoveryCodes;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityAspnetIdentity(
            IOptionsSnapshot<AppOptions> AppOptions,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            UrlEncoder urlEncoder,
            ISqlServerDbContext dbContext,
            IPasswordHasher<ApplicationUser> passwordHasher,
            ILogger<SecurityAspnetIdentity> logger, IMapper mapper, IJwtHandler jwtHandler, IRefreshTokenRepository refreshTokenRepository, IHttpContextAccessor httpContextAccessor)
        {
            _AppOptions = AppOptions.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _urlEncoder = urlEncoder;
            _dbContext = dbContext;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;


            _logger = logger;
            _jwtHandler = jwtHandler;
            _refreshTokenRepository = refreshTokenRepository;
        }

        #region User

        public async Task<LoginResponse> CreateUser(RegisterUserModel model)
        {
            //string finalEmail = "";
            //if (model.Roles.Contains(UserRoles.Student.ToString()))
            //{
            //    string baseEmail = model.LastName.Length > 0 ? EmailGenerateHelper.GenerateEmail($"{model.FirstName}.{model.LastName}","gmail.com") : EmailGenerateHelper.GenerateEmail($"{model.FirstName}","gmail.com");
            //    var existingUser = await _userManager.FindByNameAsync(baseEmail);
            //    int counter = 1;
            //    finalEmail = baseEmail;
            //    while (existingUser != null)
            //    {
            //        finalEmail = model.LastName.Length > 0 ? EmailGenerateHelper.GenerateEmail($"{model.FirstName}.{model.LastName}{counter}","gmail.com") : EmailGenerateHelper.GenerateEmail($"{model.FirstName}{counter}","gmail.com");
            //        existingUser = await _userManager.FindByNameAsync(finalEmail);
            //        counter++;
            //    }
                
            //}
            
            var user = new ApplicationUser
            {
                UserName = !string.IsNullOrEmpty(model.CNIC) ? model.CNIC : model.Email,
                CNIC = model.CNIC,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = string.IsNullOrEmpty(model.Gender) ? Gender.Male : (Gender)Enum.Parse(typeof(Gender), model.Gender),
                StatusId = (int)UserStatus.Preactive,
                ParentId = model.ParentId
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SetUserRoles(user.Id, model.Roles);
                //await _userManager.AddToRoleAsync(user, model.Roles[0].ToString());
                var claims = await AddClaims(user);
                var jwt = _jwtHandler.Create(user, claims);


                //await _userManager.AddToRoleAsync(user, UserRoles.User.ToString());
                //await _userManager.AddToRoleAsync(user, nameof(UserRoles.Worker));
                //await SendActivationEmail(user, model.ConfirmEmailURL);
                //-- Commented Because of requiremnet future will be uncomment these lines
                // var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var link = "Account Has been Registered Sucessfully";
                //_MeshWaterOptions.ApiUrl + "/Api/Account/Activation?userId=" + user.Id + "&code=" + HttpUtility.UrlEncode(emailConfirmationToken);
                //var my = (JsonWebToken)(await Login(model.Email, model.Password)).Data;
                //String myjson = my.AccessToken;
                _logger.LogInformation($"[CreateUserService] : Account Has been Registered Sucessfully");
                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                var userModel = new ApplicationUserModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName, 
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    CNIC = user.CNIC,
                    Picture = user.Picture,
                    StatusId = user.StatusId,
                };
                
                // MULTI-TENANCY: Load tenant information
                var tenant = await _dbContext.Set<Tenant>()
                    .FirstOrDefaultAsync(t => t.TenantId == user.TenantId && !t.IsDeleted);
                
                var tenantInfo = tenant != null ? new Common.DTO.Response.TenantInfo
                {
                    TenantId = tenant.TenantId,
                    TenantCode = tenant.TenantCode,
                    Name = tenant.Name,
                    ShortName = tenant.ShortName,
                    Logo = tenant.Logo,
                    PrimaryColor = tenant.PrimaryColor,
                    SecondaryColor = tenant.SecondaryColor,
                    SubscriptionTier = tenant.SubscriptionTier,
                    SubscriptionEndDate = tenant.SubscriptionEndDate,
                    IsSubscriptionValid = tenant.IsSubscriptionValid,
                    EnabledFeatures = tenant.EnabledFeatures?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(f => f.Trim()).ToList() ?? new List<string>(),
                    Currency = tenant.Currency,
                    CurrencySymbol = tenant.CurrencySymbol,
                    Language = tenant.Language,
                    DateFormat = tenant.DateFormat,
                    TimeZone = tenant.TimeZone,
                    MaxUsers = tenant.MaxUsers,
                    CurrentUserCount = tenant.CurrentUserCount,
                    StorageLimitGB = tenant.StorageLimitGB,
                    CurrentStorageUsedGB = tenant.CurrentStorageUsedGB,
                    OrganizationType = tenant.OrganizationType
                } : null;
                
                return new LoginResponse { 
                    Status = LoginStatus.Succeded.ToString(), 
                    Roles = roles, 
                    userinfo = userModel, 
                    Data = jwt, 
                    token = jwt.AccessToken, 
                    UserId = user.Id,
                    TenantId = user.TenantId, // MULTI-TENANCY
                    TenantInfo = tenantInfo  // MULTI-TENANCY
                };
            }
            var errorMessaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            _logger.LogError($"[CreateUserService] : {errorMessaeg}");
            return new LoginResponse { Status = LoginStatus.Failed.ToString(), Message = errorMessaeg, UserId = user.Id };
        }
        public async Task<AuthenticationResponse> CreateUser(object user, string password)
        {
            var appUser = (ApplicationUser)user;
            
            var result = await _userManager.CreateAsync(appUser, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, nameof(UserRoles.User));

                if (!string.IsNullOrWhiteSpace(appUser.FirstName))
                    await AddUserClaim(appUser.Id, JwtClaimTypes.GivenName.ToString(), appUser.FirstName);
                if (!string.IsNullOrWhiteSpace(appUser.LastName))
                    await AddUserClaim(appUser.Id, JwtClaimTypes.FamilyName.ToString(), appUser.LastName);
                await AddUserClaim(appUser.Id, JwtClaimTypes.Name, appUser.UserName);
                await AddUserClaim(appUser.Id, JwtClaimTypes.Email, appUser.Email);
                await AddUserClaim(appUser.Id, JwtClaimTypes.Role, nameof(UserRoles.User));

                if (!appUser.EmailConfirmed)
                {
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = emailConfirmationToken };

                    //// ToDo: Check how it works in SingleSignOn
                    //var link = webUrl + "Account/ConfirmEmail?userId=" + appUser.Id + "&code=" + HttpUtility.UrlEncode(emailConfirmationToken);
                    //var template = await _notificationTemplateService.GetNotificationTemplate(NotificationTemplates.EmailUserRegisteration, NotificationTypes.Email);
                    //var emailMessage = template.MessageBody.Replace("#Name", $"{ appUser.FirstName} { appUser.LastName}")
                    //                                       .Replace("#Link", $"{link}");

                    //var sent = await _communicationService.SendEmail(template.Subject, emailMessage, appUser.Email);
                    //return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = "Account created successfully. A confirmation link has been sent to your specified email , click the link to confirm your email and proceed to login." };
                }

                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = "Registeration successfull." };

            }
            var errorMessaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = errorMessaeg };
        }
        public async Task<BaseModel> UpdateUserDetail(UserModel userInfo)
        {
            var user = await _userManager.FindByIdAsync(userInfo.Id);
            if (user == null)
                return new BaseModel { Success = false, Message = "User not exists." };

            if (!string.IsNullOrWhiteSpace(userInfo.FirstName))
                user.FirstName = userInfo.FirstName;
            if (!string.IsNullOrWhiteSpace(userInfo.LastName))
                user.LastName = userInfo.LastName;
            if (!string.IsNullOrWhiteSpace(userInfo.CNIC))
                user.CNIC = userInfo.CNIC;
            if (!string.IsNullOrWhiteSpace(userInfo.Email))
                user.Email = userInfo.Email;
            if (!string.IsNullOrWhiteSpace(userInfo.PhoneNumber))
                user.PhoneNumber = userInfo.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(userInfo.CNIC))
                user.UserName = userInfo.CNIC;
            user.IsActive = userInfo.IsActive;

            await _userManager.UpdateAsync(user);
            return new BaseModel { Success = true, Message = "User info has been successfully updated." };
        }
        public async Task<BaseModel> GetUser(string userName)
        {
            var user = await _userManager.Users
                .Include(u => u.UserRoles)
               .ThenInclude(ur => ur.Role)
               .ThenInclude(r => r.RolePermissions)
               .ThenInclude(rp => rp.Permission)
               .ThenInclude(p => p.Module)
               .FirstOrDefaultAsync(u => u.CNIC == userName || u.Email == userName || u.UserName == userName || u.PhoneNumber == userName);
            if (user == null)
            {
                _logger.LogError("[LoginService] : User Not Found.");
                return new BaseModel
                {
                    Success = false,
                    Message = "User Not Found"
                };
            }
            
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            //var appUser = await _securityDbContext.AppUsers.FirstOrDefaultAsync(u => u.Id.Equals(user.Id));
            //if (appUser == null)
            //    return new BaseModel { Success = false, Message = "User not exists." };

            var userClaims = new UserClaims
            {
                Id = user.Id,
                //FirstName = appUser.FirstName,
                //LastName = appUser.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles
            };
            var userModel = new ApplicationUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Picture = user.Picture,
                StatusId = user.StatusId,
            };
            return new BaseModel { Success = true, Data = userModel };
        }
        public async Task<BaseModel> GetUserDetail(string userId)
        {
            //var user = await _userManager.FindByIdAsync(userId);
            var user = await _dbContext.User
                //.Include(u => u.DeliveryBoy)
                //.Include(x => x.UserDeliveryDays)
                //.ThenInclude(x => x.Day)
                .FirstOrDefaultAsync(x=> x.Id == userId);
            if (user == null)
                return new BaseModel { Success = false, Message = "User not exists." };

            //var appUser = await _securityDbContext.AppUsers.FirstOrDefaultAsync(u => u.Id.Equals(user.Id));
            //if (appUser == null)
            //    return new BaseModel { Success = false, Message = "User not exists." };

           
            var userModel = new ApplicationUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Picture = user.Picture,
                StatusId = user.StatusId,
            };
            return new BaseModel { Success = true, Data = userModel };
        }
        public async Task<BaseModel> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null)
                return new BaseModel { Success = false, Message = "No users exist." };

            var userDetails = new List<UserDetail>();
            foreach (var user in users)
            {
                var userDetail = new UserDetail();
                userDetail.Id = user.Id;
                //userDetail.FirstName = appUser.FirstName;
                //userDetail.LastName = appUser.LastName;
                userDetail.UserName = user.UserName;
                userDetail.Email = user.Email;
                userDetail.PhoneNumber = user.PhoneNumber;
                userDetail.IsEmailConfirmed = user.EmailConfirmed;
                userDetail.HasPassword = string.IsNullOrWhiteSpace(user.PasswordHash);
                userDetail.TwoFactorEnabled = user.TwoFactorEnabled;
                //userDetail.TwoFactorType = twoFactorType.Name;
                userDetail.LockoutEnabled = user.LockoutEnabled;
                userDetail.LockoutEnd = user.LockoutEnd;
                userDetail.AccessFailedCount = user.AccessFailedCount;

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    userDetail.Roles.Add(role);
                }

                userDetails.Add(userDetail);
            }

            return new BaseModel { Success = true, Data = userDetails };
        }
        public async Task<BaseModel> GetUsers(Expression<Func<object, bool>> where)
        {
            var users = await _userManager.Users.Where(where).ToListAsync();
            if (users == null)
                return new BaseModel { Success = false, Message = "No users exist." };

            var userDetails = new List<UserDetail>();
            foreach (var user in users)
            {
                var applicationUser = (ApplicationUser)user;
                var userDetail = new UserDetail();
                userDetail.Id = applicationUser.Id;
                //userDetail.FirstName = appUser.FirstName;
                //userDetail.LastName = appUser.LastName;
                userDetail.UserName = applicationUser.UserName;
                userDetail.Email = applicationUser.Email;
                userDetail.PhoneNumber = applicationUser.PhoneNumber;
                userDetail.IsEmailConfirmed = applicationUser.EmailConfirmed;
                userDetail.HasPassword = string.IsNullOrWhiteSpace(applicationUser.PasswordHash);
                userDetail.TwoFactorEnabled = applicationUser.TwoFactorEnabled;
                //userDetail.TwoFactorType = twoFactorType.Name;
                userDetail.LockoutEnabled = applicationUser.LockoutEnabled;
                userDetail.LockoutEnd = applicationUser.LockoutEnd;
                userDetail.AccessFailedCount = applicationUser.AccessFailedCount;

                var roles = await _userManager.GetRolesAsync(applicationUser);
                foreach (var role in roles)
                {
                    userDetail.Roles.Add(role);
                }

                userDetails.Add(userDetail);
            }

            return new BaseModel { Success = true, Data = userDetails };
        }
        public async Task<BaseModel> GetUsers(Expression<Func<object, bool>> where = null, Func<IQueryable<object>, IOrderedQueryable<object>> orderBy = null, params Expression<Func<object, object>>[] includeProperties)
        {
            IQueryable<object> query = _userManager.Users;

            if (ChecksHelper.NotNull(where))
                query = query.Where(where);
            if (ChecksHelper.NotNull(includeProperties))
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            if (ChecksHelper.NotNull(orderBy))
                query = orderBy(query);

            var users = await query.ToListAsync();
            if (users == null)
                return new BaseModel { Success = false, Message = "No users exist." };

            var userDetails = new List<UserDetail>();
            foreach (var user in users)
            {
                var applicationUser = (ApplicationUser)user;
                var userDetail = new UserDetail();
                userDetail.Id = applicationUser.Id;
                //userDetail.FirstName = appUser.FirstName;
                //userDetail.LastName = appUser.LastName;
                userDetail.UserName = applicationUser.UserName;
                userDetail.Email = applicationUser.Email;
                userDetail.PhoneNumber = applicationUser.PhoneNumber;
                userDetail.IsEmailConfirmed = applicationUser.EmailConfirmed;
                userDetail.HasPassword = string.IsNullOrWhiteSpace(applicationUser.PasswordHash);
                userDetail.TwoFactorEnabled = applicationUser.TwoFactorEnabled;
                //userDetail.TwoFactorType = twoFactorType.Name;
                userDetail.LockoutEnabled = applicationUser.LockoutEnabled;
                userDetail.LockoutEnd = applicationUser.LockoutEnd;
                userDetail.AccessFailedCount = applicationUser.AccessFailedCount;

                var roles = await _userManager.GetRolesAsync(applicationUser);
                foreach (var role in roles)
                {
                    userDetail.Roles.Add(role);
                }

                userDetails.Add(userDetail);
            }

            return new BaseModel { Success = true, Data = userDetails };
        }
        public async Task<BaseModel> ForgotPassword(string email)
        {
            //throw new NotImplementedException();

            //// ToDo: Check how it works in SingleSignOn
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new BaseModel { Success = false, Message = "No user exists with the specified email." };

            var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedCode = WebUtility.UrlEncode(resetCode);
            var encodedEmail = WebUtility.UrlEncode(user.Email);
            var link = _AppOptions.WebUrl + "#/authentication/set-password?code=" + encodedCode + "&email=" + encodedEmail;
            return new BaseModel { Success = true, Data = new Common.DTO.Response.ForgotPasswordModel { Link = link, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email }, Message = "Your password reset code has been sent to your specified email address, follow the link to reset your password." };
        }
        public async Task<AuthenticationResponse> ResetPassword(string email, string code, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogError($"[ResetPassword] : No User Exist");
            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "No user exists with the specified user Id." };

            }
            //var previousPasswordValidation = await ValidatePreviousPassword(user, password);
            //if (previousPasswordValidation.ResponseType.Equals(ResponseType.Error))
            //    return previousPasswordValidation;

            var result = await _userManager.ResetPasswordAsync(user, code, password);
            if (result.Succeeded)
            {
                _logger.LogInformation($"[ResetPassword] : Password was reset successfully.");
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = "Password was reset successfully." };
            }
            _logger.LogError($"[ResetPassword] : Password Reset Failed");
            return new AuthenticationResponse
            {
                ResponseType = ResponseType.Error,
                Data = result.Errors.FirstOrDefault() != null
                        ? result.Errors.FirstOrDefault().Description
                        : "Password reset failed."
            };
        }
        public async Task<AuthenticationResponse> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            if (currentPassword == newPassword)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "New password must not be same as cureent password." };

            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "No user exists with the specified email/username." };
            }

            //var previousPasswordValidation = await ValidatePreviousPassword(user, newPassword);
            //if (previousPasswordValidation.ResponseType.Equals(ResponseType.Error))
            //    return previousPasswordValidation;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = "Password changed successfully." };
            }

            string messaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = messaeg ?? "Failed to change password." };
        }
        public async Task<BaseModel> SetPassword(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseModel { Success = false, Message = "No user exists." };

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
                return new BaseModel { Success = false, Message = "You already have a password. You can only change your password." };

            var result = await _userManager.AddPasswordAsync(user, newPassword);
            if (result.Succeeded)
            {
                var userUpdateResult = await _userManager.UpdateAsync(user);
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return new BaseModel { Success = true, Data = emailConfirmationToken, Message = $"Password has been set successfully." };

                //// ToDo: Check how it works in SingleSignOn
                //var link = webUrl + "Account/ConfirmEmail?userId=" + user.Id + "&code=" + HttpUtility.UrlEncode(emailConfirmationToken);
                //var template = await _notificationTemplateService.GetNotificationTemplate(NotificationTemplates.EmailSetPassword, NotificationTypes.Email);
                //var emailMessage = template.MessageBody.Replace("#Name", $"{ user.FirstName} { user.LastName}")
                //                                       .Replace("#Link", $"{link}");

                //var sent = await _communicationService.SendEmail(template.Subject, emailMessage, user.Email);

                //return new BaseModel { success = true, message = $"Password has been set successfully. But to confirm your email address, a confirmation link has been sent to {user.Email}, please verify your email." };
            }
            string messaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new BaseModel { Success = false, Message = messaeg ?? "Failed to set password." };
        }
        public async Task<BaseModel> ChangeEmail(string userId, string email, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (result.Succeeded)
                return new BaseModel { Success = true, Message = "Your email has been changed successfully." };

            var message = result.Errors.FirstOrDefault() != null
               ? result.Errors.FirstOrDefault().Description
               : "Faild to change email.";
            return new BaseModel { Success = false, Message = message };
        }
        public async Task<BaseModel> RemovePhoneNumber(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.SetPhoneNumberAsync(user, null);
            if (!result.Succeeded)
                return new BaseModel { Success = false, Message = "Phone number could not be deleted." };

            return new BaseModel { Success = true, Message = "Your phone number has been deleted successfully." };
        }
        public async Task<BaseModel> ChangePhoneNumber(string userId, string phoneNumber, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, code);
            if (result.Succeeded)
                return new BaseModel { Success = true, Message = "Your phone number has been changed successfully." };

            string messaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new BaseModel { Success = false, Message = messaeg };
        }
        public async Task InitializeUsers(string email, string password, string role, bool isClaimedBasedRole = true)
        {
            var appUser = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var result = await _userManager.CreateAsync(appUser, password);
                if (result.Succeeded)
                {
                    if (isClaimedBasedRole)
                        await _userManager.AddClaimAsync(appUser, new Claim(JwtClaimTypes.Role, role));
                    else
                        await _userManager.AddToRoleAsync(appUser, role);
                }
            }
        }
        public async Task<BaseModel> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseModel { Success = false, Message = "No user exists with the specified email address." };

            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (emailConfirmed)
                return BaseModel.Failed(message: "Email has already been confirmed.");

            var response = await _userManager.ConfirmEmailAsync(user, code);
            if (response.Succeeded)
                return new BaseModel { Success = true, Message = "Email confirmed successfully." };

            var message = response.Errors.FirstOrDefault() != null
                ? response.Errors.FirstOrDefault().Description
                : "Email confirmation failed.";
            return new BaseModel { Success = false, Message = message };
        }
        public async Task<BaseModel> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseModel { Success = false, Message = "User not found with specified Id." };

            var lockoutResult = await _userManager.SetLockoutEnabledAsync(user, true);
            if (!lockoutResult.Succeeded)
            {
                var message = lockoutResult.Errors.FirstOrDefault() != null
                                   ? lockoutResult.Errors.FirstOrDefault().Description
                                   : "User could not be block.";
                return new BaseModel { Success = false, Message = message };
            }
            return new BaseModel { Success = false, Message = "User has been successfully blocked." };
        }
       

        #endregion User

        #region Login
        public async Task<LoginResponse> Login(string Email, string password, bool persistCookie = false)
        {
            //ApplicationUser user = await FindByCNICAsync(Email)
            //    ?? await _userManager.FindByEmailAsync(Email)
            //    ?? await _userManager.FindByNameAsync(Email)
            //    ?? await FindByPhoneNumberAsync(Email);
            var user = await _userManager.Users
                .Include(u => u.Tenant) // MULTI-TENANCY: Eagerly load tenant
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                                .ThenInclude(p => p.Module)
               .FirstOrDefaultAsync(
                u => u.CNIC == Email || u.Email == Email || u.UserName == Email || u.PhoneNumber == Email);
            if (user == null)
            {
                _logger.LogError("[LoginService] : User Not Found.");
                return new LoginResponse
                {
                    Status = LoginStatus.UserNotFound.ToString(),
                    Message = "User Not Found"
                };
            }
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogError($"[LoginService] : Invalid password.");
                return new LoginResponse { Status = LoginStatus.InvalidPassword.ToString(), Message = "Invalid password." };
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogError($"[LoginService] : Email has not yet been confirmed , please confirm your email and login again.");
                return new LoginResponse { Status = LoginStatus.EmailNotConfirmed.ToString(), Message = "Email has not yet been confirmed , please confirm your email and login again." };
            }
            if (!user.IsActive)
            {
                _logger.LogError($"[LoginService] : User is not Active. Please contact to Admin.");
                return new LoginResponse { Status = LoginStatus.UserInActive.ToString(), Message = "User is not Active. Please contact to Admin." };
            }


            var result = await _signInManager.PasswordSignInAsync(user, password, persistCookie, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Todo:
                //var map = new TMap();
                //var data = map.Transform<T, TKey>(user, roles);

                var claims = await AddClaims(user);
                var jwt = _jwtHandler.Create(user,claims);
                var refreshToken = _passwordHasher.HashPassword(user, Guid.NewGuid().ToString())
                .Replace("+", string.Empty)
                .Replace("=", string.Empty)
                .Replace("/", string.Empty);
                
                // Generate a unique device ID for this login session
                var deviceId = GenerateDeviceId();
                
                jwt.RefreshToken = refreshToken;
                await _refreshTokenRepository.AddRefreshToken(new RefreshToken { 
                    Id = user.Id, 
                    Username = user.UserName, 
                    Token = refreshToken, 
                    DeviceId = deviceId, // Add device ID
                    Revoked = false, 
                    Expired = jwt.Expires 
                });
                //var roles = (await _userManager.GetRolesAsync(user)).ToList();
                // Extract roles & permissions
                var roles = user.UserRoles
                    .Select(ur => ur.Role.Name.ToLower())
                    .Distinct()
                    .ToList();

                var permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList();
                var roleId = string.Empty;
                if (roles.Any())
                {
                    var role = await _roleManager.FindByNameAsync(roles.FirstOrDefault());
                    roleId = role?.Id ?? string.Empty;
                }
                _logger.LogInformation($"[LoginService] : Sucessfully Login");
                var userModel = new ApplicationUserModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    Picture = user.Picture,
                    StatusId = user.StatusId,
                };

                // Create device info
                var deviceInfo = new Common.DTO.Response.DeviceInfo
                {
                    DeviceId = deviceId,
                    UserAgent = GetUserAgent(),
                    IpAddress = GetClientIpAddress(),
                    LoginTime = DateTime.Now,
                    ExpiresAt = jwt.Expires,
                    IsCurrentDevice = true
                };

                // MULTI-TENANCY: Get tenant information (already loaded via Include)
                var tenant = user.Tenant;

                var tenantInfo = tenant != null ? new Common.DTO.Response.TenantInfo
                {
                    TenantId = tenant.TenantId,
                    TenantCode = tenant.TenantCode,
                    Name = tenant.Name,
                    ShortName = tenant.ShortName,
                    Logo = tenant.Logo,
                    PrimaryColor = tenant.PrimaryColor,
                    SecondaryColor = tenant.SecondaryColor,
                    SubscriptionTier = tenant.SubscriptionTier,
                    SubscriptionEndDate = tenant.SubscriptionEndDate,
                    IsSubscriptionValid = tenant.IsSubscriptionValid,
                    EnabledFeatures = tenant.EnabledFeatures?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(f => f.Trim()).ToList() ?? new List<string>(),
                    Currency = tenant.Currency,
                    CurrencySymbol = tenant.CurrencySymbol,
                    Language = tenant.Language,
                    DateFormat = tenant.DateFormat,
                    TimeZone = tenant.TimeZone,
                    MaxUsers = tenant.MaxUsers,
                    CurrentUserCount = tenant.CurrentUserCount,
                    StorageLimitGB = tenant.StorageLimitGB,
                    CurrentStorageUsedGB = tenant.CurrentStorageUsedGB,
                    OrganizationType = tenant.OrganizationType
                } : null;

                return new LoginResponse { 
                    Status = LoginStatus.Succeded.ToString(), 
                    RoleId = roleId, 
                    Roles = roles, 
                    userinfo = userModel, 
                    Data = jwt, 
                    token = jwt.AccessToken, 
                    UserId = user.Id, 
                    Message = "Login successful",
                    Permissions = permissions,
                    DeviceId = deviceId,
                    DeviceInfo = deviceInfo,
                    TenantId = user.TenantId, // MULTI-TENANCY
                    TenantInfo = tenantInfo  // MULTI-TENANCY
                };
            }
            else if (result.RequiresTwoFactor)
            {
                //// ToDo: Check how SendTwoFactorToken works in SingleSignOn

                
                //return new LoginResponse { Status = LoginStatus.Failed, Message = authenticationResult.Message };
                _logger.LogError($"[LoginService] : Requires two factor varification.");
                return new LoginResponse { Status = LoginStatus.RequiresTwoFactor.ToString(), Message = "Requires two factor varification.", Data = user.UserName };
            }
            else
            {
                _logger.LogError($"[LoginService] : Invalid login attempt.");
                return new LoginResponse { Status = LoginStatus.Failed.ToString(), Message = result.IsLockedOut ? "Locked Out" : "Invalid login attempt." };
            }
        }
        public async Task<JsonWebToken> RefreshAccessToken(string token)
        {
            var refreshToken = await GetRefreshToken(token);
            if (refreshToken == null)
            {
                throw new Exception("Refresh token was not found.");
            }
            if (refreshToken.Revoked)
            {
                throw new Exception("Refresh token was revoked");
            }
            var user = await _userManager.FindByIdAsync(refreshToken.Id);
            var claims = await AddClaims(user);
            //var claims = await GetUserClaims(refreshToken.Id);
            var jwt = _jwtHandler.Create(user, claims);
            jwt.RefreshToken = refreshToken.Token;

            return jwt;
        }

        public async Task RevokeRefreshToken(string token)
        {
            var refreshToken = await GetRefreshToken(token);
            if (refreshToken == null)
            {
                throw new Exception("Refresh token was not found.");
            }
            if (refreshToken.Revoked)
            {
                throw new Exception("Refresh token was already revoked.");
            }
            refreshToken.Revoked = true;
        }
        public async Task<BaseModel> Logout()
        {
            await _signInManager.SignOutAsync();
            return new BaseModel { Success = true, Message = "Signed out successfully." };
        }
        #endregion Login

        #region Token
        private async Task<IList<Claim>> AddClaims(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>();

            await AddUserClaim(user.Id, ClaimTypes.Name, user.Email);
            await AddUserClaim(user.Id, JwtClaimTypes.Id, user.Id);
            await AddUserClaim(user.Id, JwtClaimTypes.Name, user.UserName);
            await AddUserClaim(user.Id, JwtClaimTypes.Email, user.Email);
            
            // MULTI-TENANCY: Add TenantId claim for data isolation
            await AddUserClaim(user.Id, "TenantId", user.TenantId.ToString());
            
            claims.AddRange(new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtClaimTypes.Id, user.Id),
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim("TenantId", user.TenantId.ToString()), // MULTI-TENANCY: Include tenant in JWT
            });

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
                await AddUserClaim(user.Id, JwtClaimTypes.GivenName, user.FirstName);
            }
            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
                await AddUserClaim(user.Id, ClaimTypes.Surname, user.LastName);
            }

            var roles = (await _userManager.GetRolesAsync(user));

            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role.ToString()));
                await AddUserClaim(user.Id, JwtClaimTypes.Role, role.ToString());
            }

            return claims;
        }
        public async Task<AuthenticationResponse> AddUserClaim(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "User not found with specified Id." };

            var userClaims = await _userManager.GetClaimsAsync(user);

            if (userClaims.Any())
            {
                var givenClaim = userClaims.FirstOrDefault(x => x.Type.ToLower() == claimType.ToLower());

                if (givenClaim != null)
                    return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "The specified claim already assigned to user, try different value." };
            }
            var result = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));

            if (result.Succeeded)
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = "Claim added." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : "Failed to add claim.";

            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = message };
        }
        public async Task<BaseModel> GetUserClaim(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseModel { Success = false, Data = "User not found with specified Id." };

            var userClaims = await _userManager.GetClaimsAsync(user);
            return new BaseModel { Success = false, Data = userClaims };
        }
        public async Task<AuthenticationResponse> RemoveUserClaim(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "User not found with specified Id." };

            var userClaims = await _userManager.GetClaimsAsync(user);

            if (userClaims.Any())
            {
                var givenClaim = userClaims.FirstOrDefault(x => x.Type.ToLower() == claimType.ToLower() && x.Value.ToLower() == claimValue.ToLower());

                if (givenClaim == null)
                    return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "User doesn't have the specified claim." };
            }

            var result = await _userManager.RemoveClaimAsync(user, new Claim(claimType, claimValue));

            if (result.Succeeded)
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = "Claim removed successfully." };

            return new AuthenticationResponse
            {
                ResponseType = ResponseType.Success,
                Data = result.Errors.FirstOrDefault() != null ?
                result.Errors.FirstOrDefault().Description : "Failed to remove claim."
            };
        }
       
        #endregion Token

        #region Roles
        public async Task<AuthenticationResponse> CreateRole(string role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (roleExist)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = $"{role} role already exists." };

            var identityRole = new ApplicationRole
            {
                Name = role
            };
            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = $"{role} role successfully added." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to add {role} role.";

            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = message };
        }
        public async Task<AuthenticationResponse> AddUserRole(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "User not found with specified Id." };

            string notFoundRoles = string.Empty;
            //var notExistingRoles = _roleManager.Roles.ToList().Select(role => { role.Name = role.Name.ToLower(); return role.Name; }).Where(roleName => !roles.Contains(roleName));
            var notExistingRoles = roles
                                    .ToList()
                                    .Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                    .Where(roleName => !_roleManager.Roles
                                                                        .ToList()
                                                                        .Select(role => { role.Name = role.Name.ToLower(); return role.Name; })
                                                                        .Contains(roleName));
            foreach (var roleName in notExistingRoles)
            {
                notFoundRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(notFoundRoles))
                return new AuthenticationResponse
                {
                    ResponseType = ResponseType.Error,
                    Data = $"{notFoundRoles.Remove(notFoundRoles.LastIndexOf(','))} {(notExistingRoles.Count() > 1 ? "roles" : "role")} not found in the system."
                };

            var alreadyFoundUserRoles = string.Empty;
            var userRoles = await _userManager.GetRolesAsync(user);
            var alreadyExistingUserRoles = userRoles
                                                .Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                                .Where(roleName => roles.Contains(roleName));
            foreach (var roleName in alreadyExistingUserRoles)
            {
                alreadyFoundUserRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(alreadyFoundUserRoles))
                return new AuthenticationResponse
                {
                    ResponseType = ResponseType.Error,
                    Data = $"User is already in {alreadyFoundUserRoles.Remove(alreadyFoundUserRoles.LastIndexOf(','))} {(alreadyFoundUserRoles.Count() > 1 ? "roles" : "role")}."
                };

            var result = await _userManager.AddToRolesAsync(user, roles);
            if (result.Succeeded)
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = $"{(roles.Count() > 1 ? "Roles" : "Role")} successfully added for user." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to add {(roles.Count() > 1 ? "roles" : "role")} for user.";

            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = message };
        }

        /// <summary>
        /// Sets user roles by replacing all existing roles with the new set of roles.
        /// This method handles both adding new roles and updating existing roles.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="roles">The new set of roles to assign to the user</param>
        /// <returns>AuthenticationResponse indicating success or failure</returns>
        public async Task<AuthenticationResponse> SetUserRoles(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "User not found with specified Id." };

            // Validate that all roles exist in the system
            string notFoundRoles = string.Empty;
            var notExistingRoles = roles
                                    .ToList()
                                    .Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                    .Where(roleName => !_roleManager.Roles
                                                                        .ToList()
                                                                        .Select(role => { role.Name = role.Name.ToLower(); return role.Name; })
                                                                        .Contains(roleName));
            foreach (var roleName in notExistingRoles)
            {
                notFoundRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(notFoundRoles))
                return new AuthenticationResponse
                {
                    ResponseType = ResponseType.Error,
                    Data = $"{notFoundRoles.Remove(notFoundRoles.LastIndexOf(','))} {(notExistingRoles.Count() > 1 ? "roles" : "role")} not found in the system."
                };

            try
            {
                // Get current user roles
                var currentUserRoles = await _userManager.GetRolesAsync(user);
                
                // Remove all existing roles
                if (currentUserRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentUserRoles);
                    if (!removeResult.Succeeded)
                    {
                        var message = removeResult.Errors.FirstOrDefault() != null
                            ? removeResult.Errors.FirstOrDefault().Description
                            : "Failed to remove existing roles from user.";
                        return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = message };
                    }
                }

                // Add new roles
                if (roles.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, roles);
                    if (!addResult.Succeeded)
                    {
                        var message = addResult.Errors.FirstOrDefault() != null
                            ? addResult.Errors.FirstOrDefault().Description
                            : "Failed to add new roles to user.";
                        return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = message };
                    }
                }

                return new AuthenticationResponse 
                { 
                    ResponseType = ResponseType.Success, 
                    Data = $"User roles successfully updated. {(roles.Count() > 1 ? "Roles" : "Role")} assigned: {string.Join(", ", roles)}" 
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResponse 
                { 
                    ResponseType = ResponseType.Error, 
                    Data = $"An error occurred while updating user roles: {ex.Message}" 
                };
            }
        }
        public async Task<AuthenticationResponse> UpdateRole(string id, string role)
        {
            var existingRole = await _roleManager.FindByIdAsync(id);
            if (existingRole == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = $"Role not found with specified Id." };

            existingRole.Name = role;
            var result = await _roleManager.UpdateAsync(existingRole);
            if (result.Succeeded)
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = $"Role successfully updated." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to update role.";

            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = message };
        }
        public async Task<BaseModel> GetRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return new BaseModel { Success = false, Data = $"{role} role not found." };

            return new BaseModel { Success = true, Data = role };
        }
        public async Task<BaseModel> GetRoles(bool isSystem = true)
        {
            var roles = await _roleManager.Roles.Where(x => x.IsSystem == isSystem).ToListAsync();
            //var roles = await _roleManager.Roles.Where(x => x.IsSystem == isSystem && x.Name != UserRoles.SuperAdmin.ToString()).ToListAsync();

            return new BaseModel { Success = true, Data = roles, LastId = Convert.ToInt32(roles.LastOrDefault().Id), Total = roles.Count };
        }
        public async Task<BaseModel> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseModel { Success = false, Message = "User not found with specified Id." };

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles == null || !userRoles.Any())
                return new BaseModel { Success = false, Message = "User do not have any role." };

            return new BaseModel { Success = true, Data = userRoles };
        }
        public async Task<AuthenticationResponse> RemoveRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = $"{role} role not found." };

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = $"{role} role removed successfully." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to remove {role} role.";

            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = message };
        }
        public async Task<AuthenticationResponse> RemoveUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "User not found with specified Id." };

            var role = await _roleManager.FindByIdAsync(roleName);
            if (role == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = $"{roleName} role not found in the system." };

            var userInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!userInRole)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = $"User do not have {roleName} role." };

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = $"{roleName} role removed successfully from user." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to remove {roleName} role form user.";

            return new AuthenticationResponse {  ResponseType = ResponseType.Error, Data = message };
        }
        public async Task<AuthenticationResponse> RemoveUserRoles(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = "User not found with specified Id." };

            string notFoundRoles = string.Empty;
            //var notExistingRoles = _roleManager.Roles.ToList().Select(role => { role.Name = role.Name.ToLower(); return role.Name; }).Where(roleName => !roles.Contains(roleName));
            var notExistingRoles = roles
                                    .ToList()
                                    .Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                    .Where(roleName => !_roleManager.Roles
                                                                        .ToList()
                                                                        .Select(role => { role.Name = role.Name.ToLower(); return role.Name; })
                                                                        .Contains(roleName));
            foreach (var roleName in notExistingRoles)
            {
                notFoundRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(notFoundRoles))
                return new AuthenticationResponse
                {
                    ResponseType = ResponseType.Error,
                    Data = $"{notFoundRoles.Remove(notFoundRoles.LastIndexOf(','))} {(notExistingRoles.Count() > 1 ? "roles" : "role")} not found in the system."
                };

            var notFoundUserRoles = string.Empty;
            var userRoles = await _userManager.GetRolesAsync(user);
            var notExistingUserRoles = roles
                                        .Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                        .Where(roleName => !userRoles
                                                                .Select(roleNam => { roleNam = roleNam.ToLower(); return roleNam; })
                                                                .Contains(roleName));
            foreach (var roleName in notExistingUserRoles)
            {
                notFoundUserRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(notFoundUserRoles))
                return new AuthenticationResponse
                {
                    ResponseType = ResponseType.Error,
                    Data = $"User is not in {notFoundUserRoles.Remove(notFoundUserRoles.LastIndexOf(','))} {(notFoundUserRoles.Count() > 1 ? "roles" : "role")}."
                };

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (result.Succeeded)
                return new AuthenticationResponse { ResponseType = ResponseType.Success, Data = "Roles removed successfully from user." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : "Failed to remove roles form user.";

            return new AuthenticationResponse { ResponseType = ResponseType.Error, Data = message };
        }

        #endregion Roles




        #region Private Methods
        private async Task<RefreshToken> GetRefreshToken(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshToken(token);
            return refreshToken;
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
                result.Append(unformattedKey.Substring(currentPosition));

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                authenticatorUriFormat,
                _urlEncoder.Encode("Timelogger"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private string GenerateDeviceId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private string GetUserAgent()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                return httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    // Try to get IP from various headers (for proxy/load balancer scenarios)
                    var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                             httpContext.Request.Headers["X-Real-IP"].FirstOrDefault() ??
                             httpContext.Connection.RemoteIpAddress?.ToString();
                    
                    return ip ?? "Unknown";
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
        #endregion Private Methods
    }
}