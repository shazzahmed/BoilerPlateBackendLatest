using System.Web;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.Extensions;
using Application.ServiceContracts;
using Application.Interfaces;
using Presentation.Filters;
using Common.DTO.Response;
using Common.DTO.Request;
using Common.Helpers;
using static Common.Utilities.Enums;
using Infrastructure.Services.Services;

namespace RestApi.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IHttpContextAccessor httpContextAccessor;
        //private readonly ICurrentUserService _currentUser;
        //private readonly ITokenManager _tokenManager;
        private readonly ILogger<AccountController> _logger;
        public AccountController(
                IHttpContextAccessor httpContextAccessor,
                IUserService userService,
                IRefreshTokenService refreshTokenService,
                //ICurrentUserService currentUser,
                //ITokenManager tokenManager, 
                ILogger<AccountController> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            //_currentUser = currentUser;
            _userService = userService;
            //_tokenManager = tokenManager;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }


        #region public actions

        // POST: Api/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ActionName("Register")]
        [Route("Register")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            try
            {
                var result = await _userService.CreateUser(model);

                if (result.Status == LoginStatus.Failed.ToString())
                    return new ObjectResult(BaseModel.Failed(result.Message));

                return new ObjectResult(BaseModel.Succeed(message: result.Message, data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AccountController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Create(success: false, message: ex.InnerException.Message));

            }
        }

        // POST: Api/Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ActionName("Login")]
        [Route("Login")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(LoginResponse))]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var result = await _userService.Login(model.Email, model.Password, model.RememberMe);
                
                if (result.Status == nameof(LoginStatus.Succeded) || result.Status == nameof(LoginStatus.RequiresTwoFactor))
                {
                    result.Message = "Succeded";
                    return new OkObjectResult(result);
                }
                if (result.Status == nameof(LoginStatus.Failed))
                    return new OkObjectResult(new LoginResponse { Status = nameof(LoginStatus.Failed), Data = null, Message = result.Message });
                return StatusCode(StatusCodes.Status406NotAcceptable, new LoginResponse { Status = result.Status, Data = null, Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AccountController] : {ex.Message}");
                return new BadRequestObjectResult(new LoginResponse { Message = "There was an error processing your request, please try again." });
            }
        }

        // POST: Api/Account/Activation
        [HttpGet]
        [AllowAnonymous]
        [ActionName("Activation")]
        [Route("Activation")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> Activation([FromQuery(Name = "userId")] string UserId, [FromQuery(Name = "code")] string Code)
        {
            try
            {
                var response = await _userService.ConfirmEmail(UserId, Code);
                if (response.Success)
                    return new ObjectResult(BaseModel.Succeed(message: "Email confirmed. Please login."));

                return new OkObjectResult(BaseModel.Failed(response.Message));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: Api/Account/GetUser
        [HttpGet]
        [Authorize]
        [ActionName("GetUser")]
        [Route("GetUser")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var userName = GetUserName();
                var result = await _userService.GetUser(userName);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: Api/Account/Users
        [HttpGet]
        [Authorize]
        [ActionName("GetUsers")]
        [Route("Users")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var result = await _userService.GetUsers();
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: Api/Account/AllUsersByRole
        [HttpGet]
        //[Authorize]
        [ActionName("AllUsersByRole")]
        [Route("AllUsersByRole")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> AllUsersByRole(string role)
        {
            try
            {
                var result = await _userService.GetAllUsersByRole(role);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: Api/Account/GetAllSystemRoles
        [HttpGet]
        //[Authorize]
        [ActionName("GetAllSystemRoles")]
        [Route("GetAllSystemRoles")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllSystemRoles()
        {
            try
            {
                var result = await _userService.GetRoles(true);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }
        // GET: Api/Account/GetAllUserRoles
        [HttpGet]
        //[Authorize]
        [ActionName("GetAllUserRoles")]
        [Route("GetAllUserRoles")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetAllUserRoles()
        {
            try
            {
                var result = await _userService.GetRoles(false);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }
        // GET: Api/Account/CreateRole
        [HttpPost]
        [Authorize]
        [ActionName("CreateRole")]
        [Route("CreateRole")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CreateRole(string role)
        {
            try
            {
                var result = await _userService.CreateRole(role);
                return new ObjectResult(BaseModel.Succeed(message: "Succesful", data: result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AccountController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }
        // GET: Api/Account/UpdateRole
        [HttpPost]
        [Authorize]
        [ActionName("UpdateRole")]
        [Route("UpdateRole")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateRole(RoleModel model)
        {
            try
            {
                await _userService.UpdateRole(model.Id, model.Name);
                return new ObjectResult(BaseModel.Succeed(message: "Succesful"));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AccountController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: Api/Account/AllUsers
        [HttpGet]
        [Authorize]
        [ActionName("GetAllUser")]
        [Route("AllAgents")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> AllUsers()
        {
            try
            {
                var result = await _userService.GetAllUsersByRole(UserRoles.User.ToString());
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: Api/Account/GetUserDetail
        [HttpGet]
        [Authorize]
        [ActionName("GetUserDetail")]
        [Route("GetUserDetail")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetUserDetail()
        {
            try
            {
                var userId = GetUserId();
                var result = await _userService.GetUserDetail(userId);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Account/UpdateUserDetail
        [HttpPost]
        [Authorize]
        [ActionName("UpdateUserDetail")]
        [Route("UpdateUserDetail")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> UpdateUserDetail(UserModel userInfo)
        {
            try
            {
                userInfo.Id = GetUserId();
                if (!ModelState.IsValid)
                {
                    var errorMessage = ModelState.Select(x => x.Value.Errors).FirstOrDefault()?.FirstOrDefault()?.ErrorMessage;
                    return new BadRequestObjectResult(BaseModel.Failed(message: errorMessage));
                }

                var result = await _userService.UpdateUserDetail(userInfo);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ActionName("ForgotPassword")]
        [Route("ForgotPassword")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ForgotPassword(ResetModel model)
        {
            try
            {
                var response = await _userService.ForgotPassword(model.Email);
                if (!response.Success)
                    return new OkObjectResult(BaseModel.Failed(response.Message));
                
                var baseModel = JsonSerializerHelper.Deserialize<BaseModel>(response);
                if (!baseModel.Success)
                {
                    return new BadRequestObjectResult(BaseModel.Failed(message: "An errro occured while processing your request, please try again."));
                }

                return new ObjectResult(BaseModel.Succeed(message: response.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AccountController] : {ex.Message}");
                return new BadRequestObjectResult(BaseModel.Failed(message: "An errro occured while processing your request, please try again."));
            }
        }

        // POST: Api/Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ActionName("ResetPassword")]
        [Route("ResetPassword")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ResetPassword(PasswordResetModel model)
        {
            try
            {
                var result = await _userService.ResetPassword(model.Email, model.Code, model.NewPassword);
                if (result.ResponseType == ResponseType.Success)
                    return new ObjectResult(BaseModel.Succeed(message: result.Data));

                return new ObjectResult(BaseModel.Failed(message: result.Data));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "An errro occured while processing your request, please try again."));
            }
        }

        // POST: Api/Account/ChangePassword
        [HttpPost]
        [Authorize]
        [ActionName("ChangePassword")]
        [Route("ChangePassword")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ChangePassword(PasswordChangeModel model)
        {
            try
            {
                var userName = User.GetUserName();
                var response = await _userService.ChangePassword(userName, model.OldPassword, model.NewPassword);
                if (response.ResponseType == ResponseType.Success)
                    return new ObjectResult(BaseModel.Succeed(message: response.Data));

                return new ObjectResult(BaseModel.Failed(response.Data));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "An errro occured while processing your request. Please try again later." + ex));
            }
        }

        // POST: Api/Account/SetPassword
        [HttpPost]
        [Authorize]
        [ActionName("SetPassword")]
        [Route("SetPassword")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> SetPassword(SetPasswordModel model)
        {
            try
            {
                var userId = GetUserId();
                var response = await _userService.SetPassword(userId, model.NewPassword);
                if (!response.Success)
                    return new ObjectResult(BaseModel.Failed(response.Message));

                var updateUserInfoModel = new UserModel
                {
                    Id = userId,
                    FirstName = GetUserFirstName(),
                    LastName = GetUserLastName()
                };
                var result = await _userService.UpdateUserDetail(updateUserInfoModel);
                if (!result.Success)
                    return new ObjectResult(BaseModel.Failed(result.Message));

                return new ObjectResult(BaseModel.Succeed(message: response.Message));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "An errro occured while processing your request. Please try again later."));
            }
        }

        ////
        //// POST: Api/Account/SendEmailCode
        //[HttpPost]
        //[Authorize]
        //[ActionName("SendEmailCode")]
        //[Route("Api/Account/SendEmailCode")]
        //[ServiceFilter(typeof(ValidateModelState))]
        //[Produces("application/json", Type = typeof(BaseModel))]
        //public async Task<IActionResult> SendEmailCode(ChangeEmailModel model)
        //{
        //    try
        //    {
        //        var userId = GetUserId();
        //        var response = await _securityService.SendEmailCode(userId, model.Email);
        //        if (response.Success)
        //            return new ObjectResult(BaseModel.Succeed(message: response.Message));

        //        return new ObjectResult(BaseModel.Failed(response.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BadRequestObjectResult(BaseModel.Failed(message: "An errro occured while processing your request. Please try again later."));
        //    }
        //}

        // POST: Api/Account/ChangeEmail
        [HttpPost]
        [Authorize]
        [ActionName("ChangeEmail")]
        [Route("ChangeEmail")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> ChangeEmail(ConfirmEmailModel model)
        {
            try
            {
                var userId = GetUserId();
                var response = await _userService.ChangeEmail(userId, model.Email, model.Code);
                if (response.Success)
                    return new ObjectResult(BaseModel.Succeed(message: response.Message));

                return new ObjectResult(BaseModel.Failed(response.Message));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "An errro occured while processing your request. Please try again later."));
            }
        }

        // POST: Api/Account/ChangePhoneNumber
        [HttpPut]
        [Authorize]
        [ActionName("ChangePhoneNumber")]
        [Route("ChangePhoneNumber")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<ActionResult> ChangePhoneNumber(VerifyPhoneNumberModel model)
        {
            try
            {
                var userId = GetUserId();
                var response = await _userService.ChangePhoneNumber(userId, model.PhoneNumber, model.Code);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Account/RemovePhoneNumber
        [HttpDelete]
        [Authorize]
        [ActionName("RemovePhoneNumber")]
        [Route("RemovePhoneNumber")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            try
            {
                var userId = GetUserId();
                var response = await _userService.RemovePhoneNumber(userId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }


        [HttpPost]
        [Authorize]
        [ActionName("AddRoletoUser")]
        [Route("AddRoletoUser")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> AddRoletoUser(string UserId,IEnumerable<string> roles)
        {
            try
            {
                var result = await _userService.AddUserRole(UserId, roles);
                return new OkObjectResult(new AuthenticationResponse { ResponseType = result.ResponseType, Data = result.Data});
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Create(success: false, message: ex.InnerException.Message));

            }
        }

        /// <summary>
        /// Sets user roles by replacing all existing roles with the new set of roles.
        /// This endpoint handles both adding new roles and updating existing roles.
        /// </summary>
        /// <param name="UserId">The user ID</param>
        /// <param name="roles">The new set of roles to assign to the user</param>
        /// <returns>AuthenticationResponse indicating success or failure</returns>
        [HttpPost]
        [Authorize]
        [ActionName("SetUserRoles")]
        [Route("SetUserRoles")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> SetUserRoles(string UserId, IEnumerable<string> roles)
        {
            try
            {
                var result = await _userService.SetUserRoles(UserId, roles);
                return new OkObjectResult(new AuthenticationResponse { ResponseType = result.ResponseType, Data = result.Data });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Create(success: false, message: ex.InnerException?.Message ?? ex.Message));
            }
        }

        [HttpPost]
        [Authorize]
        [ActionName("RemoveRoletoUser")]
        [Route("RemoveRoletoUser")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> RemoveRoletoUser(string UserId, IEnumerable<string> roles)
        {
            try
            {
                var result = await _userService.RemoveUserRoles(UserId, roles);
                return new OkObjectResult(new AuthenticationResponse { ResponseType = result.ResponseType, Data = result.Data });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Create(success: false, message: ex.InnerException.Message));

            }
        }
        // GET: Api/Account/CloseSession
        [HttpPost]
        [Authorize]
        [ActionName("CloseSession")]
        [Route("CloseSession")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> CloseSession(string refreshTokenId)
        {
            try
            {
                if (refreshTokenId == null)
                    return new ObjectResult(BaseModel.Failed(message: "Null refreshTokenId ", data: false));

                var result = await _refreshTokenService.RemoveRefreshTokenByID(refreshTokenId);
                return new ObjectResult(BaseModel.Succeed(message: "Success", data: result));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }
        // GET: Api/Account/GetRefreshTokeById
        [HttpPost]
        [Authorize]
        [ActionName("GetRefreshTokeById")]
        [Route("GetRefreshTokeById")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetRefreshTokeById(string refreshTokenId)
        {
            try
            {
                if (refreshTokenId == null)
                    return new ObjectResult(BaseModel.Failed(message: "Null refreshTokenId ", data: false));

                var result = await _refreshTokenService.FindRefreshToken(refreshTokenId);
                return new ObjectResult(BaseModel.Succeed(message: "Success", data: result));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }
        // GET: Api/Account/IsValidSession
        [HttpPost]
        [Authorize]
        [ActionName("IsValidSession")]
        [Route("IsValidSession")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> IsValidSession(string identificationId, string email)
        {
            try
            {
                identificationId = HttpUtility.HtmlDecode(identificationId);
                email = HttpUtility.HtmlDecode(email);
                if (email == null)
                    return new ObjectResult(BaseModel.Failed(message: "Null Email ", data: false));
                var tokenId = await _refreshTokenService.GetRefreshTokenByEmail(email);
                if (tokenId != null)
                    return new ObjectResult(BaseModel.Succeed(message: "Success", data: tokenId.Id.Equals(identificationId)));
                return new ObjectResult(BaseModel.Failed(message: "Not Valid", data: false));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }
        // POST: Api/Account/RefreshAccessToken/{token}/refresh-access-token
        [HttpPost]
        [AllowAnonymous]
        [ActionName("RefreshAccessToken")]
        [Route("RefreshAccessToken/{token}/refresh-access-token")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> RefreshAccessToken(string token)
        {
            try
            {
                //var refreshtoken = await _userService.RefreshAccessToken(token, User.GetUserClaims(out IList<Claim> claims));
                var refreshtoken = await _userService.RefreshAccessToken(token);
                return new OkObjectResult(BaseModel.Succeed(data: refreshtoken));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Account/RevokeRefreshToken/{token}/revoke
        [HttpPost]
        [Authorize]
        [ActionName("RevokeRefreshToken")]
        [Route("RevokeRefreshToken/{token}/revoke")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public IActionResult RevokeRefreshToken(string token)
        {
            try
            {
                _userService.RevokeRefreshToken(token);
                return new NoContentResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Account/RevokeAllRefreshTokens - Logout from all devices
        [HttpPost]
        [Authorize]
        [ActionName("RevokeAllRefreshTokens")]
        [Route("RevokeAllRefreshTokens")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> RevokeAllRefreshTokens()
        {
            try
            {
                var userName = User.GetUserName();
                var result = await _refreshTokenService.RevokeAllRefreshTokensForUser(userName);
                if (result)
                {
                    return new OkObjectResult(BaseModel.Succeed(message: "Logged out from all devices successfully"));
                }
                return new BadRequestObjectResult(BaseModel.Failed(message: "Failed to logout from all devices"));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // GET: Api/Account/GetActiveSessions - Get all active sessions for current user
        [HttpGet]
        [Authorize]
        [ActionName("GetActiveSessions")]
        [Route("GetActiveSessions")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> GetActiveSessions()
        {
            try
            {
                
                var userName = User.GetUserName();
                var result = await _refreshTokenService.GetActiveRefreshTokensByUsername(userName);
                return new OkObjectResult(BaseModel.Succeed(message: "Active sessions retrieved successfully", data: result));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        // POST: Api/Account/RevokeSessionByDevice - Logout from specific device
        [HttpPost]
        [Authorize]
        [ActionName("RevokeSessionByDevice")]
        [Route("RevokeSessionByDevice")]
        [Produces("application/json", Type = typeof(BaseModel))]
        public async Task<IActionResult> RevokeSessionByDevice(string deviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceId))
                {
                    return new BadRequestObjectResult(BaseModel.Failed(message: "Device ID is required"));
                }

                var userName = User.GetUserName();
                var result = await _refreshTokenService.RevokeRefreshTokenByDevice(userName, deviceId);
                if (result)
                {
                    return new OkObjectResult(BaseModel.Succeed(message: "Session revoked successfully"));
                }
                return new BadRequestObjectResult(BaseModel.Failed(message: "Failed to revoke session"));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(BaseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }
        #endregion

        #region private methods

        private IActionResult CreateResponse(AuthenticationResponse response, string ErrorMessage)
        {
            if (response.ResponseType == ResponseType.Error)
                return new BadRequestObjectResult(BaseModel.Failed(message: response.Data));

            if (response.ResponseType == ResponseType.Success)
            {
                var baseModel = JsonSerializerHelper.Deserialize<BaseModel>(response.Data);
                return new OkObjectResult(baseModel);
            }
            return new BadRequestObjectResult(BaseModel.Failed(message: ErrorMessage));
        }

        #endregion
    }
}
