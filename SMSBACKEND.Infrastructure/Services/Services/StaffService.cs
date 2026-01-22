
using Application.ServiceContracts;
using AutoMapper;
using Common.DTO.Request;
using Common.DTO.Response;
using Common.Helpers;
using Domain.Entities;
using Domain.RepositoriesContracts;
using Infrastructure.Services.Communication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using static Common.Utilities.Enums;

namespace Infrastructure.Services.Services
{

    public class StaffService : BaseService<StaffModel, Staff, int>, IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        protected readonly UserManager<ApplicationUser> _applicationUserManager;
        private readonly ICacheProvider _cacheProvider;
        private readonly SseService _sseService;

        public StaffService(
            IMapper mapper,
            IStaffRepository staffRepository,
            IUnitOfWork unitOfWork,
            SseService sseService,
            ICacheProvider cacheProvider,
            IDepartmentService departmentService,
            IUserService userService,
            UserManager<ApplicationUser> applicationUserManager) : base(mapper, staffRepository, unitOfWork, sseService, cacheProvider)
        {
            _staffRepository = staffRepository;
            _userService = userService;
            _departmentService = departmentService;
            _applicationUserManager = applicationUserManager;
            _cacheProvider = cacheProvider;
            _sseService = sseService;
        }
        public async Task<BaseModel> GetAllStaff(StaffRequest request)
        {
            var filterParams = ConvertToFilterParams(request);
            var (staffs, totalCount, lastId) = await _staffRepository.GetAllStaff(filterParams);
            return new BaseModel { Success = true, Data = staffs, Total = totalCount, LastId = lastId };
        }
        public async Task<BaseModel> CreateStaff(StaffCreateReq model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.CNIC))
                {
                    // ✅ Check if user with this CNIC exists (regardless of role)
                    var existingUserResponse = await _userService.GetUser(model.CNIC);
                    ApplicationUserModel staff;
                    
                    if (existingUserResponse.Data == null)
                    {
                        // ✅ No user exists - create new user account
                        var staffRequest = new RegisterUserModel
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            UserName = string.IsNullOrEmpty(model.Email) ? EmailGenerateHelper.GenerateEmail(model.FullName, "gmail.com") : model.Email,
                            CNIC = model.CNIC,
                            Email = string.IsNullOrEmpty(model.Email) ? EmailGenerateHelper.GenerateEmail(model.FullName, "gmail.com") : model.Email,
                            PhoneNumber = model.PhoneNo ?? model.ContactNo,
                            Password = "123",
                            ConfirmPassword = "123",
                            Roles = model.Roles,
                            ParentId = null
                        };

                        staff = (ApplicationUserModel)(await _userService.CreateUser(staffRequest)).userinfo;
                    }
                    else
                    {
                        // ✅ User exists (e.g., Parent) - add new roles to existing account
                        staff = (ApplicationUserModel)existingUserResponse.Data;
                        
                        // Get existing roles
                        var existingUser = await _applicationUserManager.FindByIdAsync(staff.Id);
                        var existingRoles = await _applicationUserManager.GetRolesAsync(existingUser);
                        
                        // Merge new roles with existing roles (avoid duplicates)
                        var rolesToAdd = new List<string>();
                        if (model.Roles != null && model.Roles.Any())
                        {
                            rolesToAdd = model.Roles.Where(r => !existingRoles.Contains(r)).ToList();
                        }
                        else if (model.Role != default)
                        {
                            var roleString = model.Role.ToString();
                            if (!existingRoles.Contains(roleString))
                            {
                                rolesToAdd.Add(roleString);
                            }
                        }
                        
                        // Add new roles if any
                        if (rolesToAdd.Any())
                        {
                            await _applicationUserManager.AddToRolesAsync(existingUser, rolesToAdd);
                        }
                        
                        // Ensure user is active
                        if (!staff.IsActive)
                        {
                            existingUser.IsActive = true;
                            await _applicationUserManager.UpdateAsync(existingUser);
                        }
                    }
                    
                    model.Email = staff.Email;
                    model.UserId = staff.Id;
                    model.ContactNo = string.IsNullOrEmpty(model.ContactNo) ? model.PhoneNo : model.ContactNo;
                }

                var result = await _staffRepository.CreateStaff(model);
                return new BaseModel { Success = true, Data = result, Total = result.Id, Message = "Succesful" };
            }
            catch
            {
                throw;
            }
        }
        public async Task<BaseModel> UpdateStaff(StaffCreateReq model)
        {
            try
            {
                var staffUpdate = await _userService.UpdateUserDetail(new UserModel
                {
                    Id = model.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = string.IsNullOrEmpty(model.Email) ? EmailGenerateHelper.GenerateEmail(model.FullName, "gmail.com") : model.Email,
                    CNIC = model.CNIC,
                    Email = string.IsNullOrEmpty(model.Email) ? EmailGenerateHelper.GenerateEmail(model.FullName, "gmail.com") : model.Email,
                    IsActive = true,
                    PhoneNumber = model.PhoneNo ?? model.ContactNo,
                });
                var result = await _staffRepository.UpdateStaff(model);
                
                // Handle multiple roles if provided
                if (model.Roles != null && model.Roles.Any())
                {
                    AuthenticationResponse response = await _userService.SetUserRoles(model.UserId, model.Roles);
                }
                else if (model.Role != default)
                {
                    // Fallback to single role for backward compatibility
                    var roles = new List<string> { model.Role.ToString() };
                    AuthenticationResponse response = await _userService.SetUserRoles(model.UserId, roles);
                }
                
                return new BaseModel { Success = true, Data = result, Message = "Successful" };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseModel> DeleteStaff(int id)
        {

            var staff = await FirstOrDefaultAsync(x => x.Id == id);
            if (staff != null)
            {
                await SoftDelete(staff);
                var user = await _applicationUserManager.FindByIdAsync(staff.UserId);
                if (user != null)
                {
                    user.IsActive = false;
                    await _applicationUserManager.UpdateAsync(user);
                    var roles = await _applicationUserManager.GetRolesAsync(user);
                    await _applicationUserManager.RemoveFromRolesAsync(user, roles);
                }
                return new BaseModel { Success = true, Message = "Succesful" };
            }
            return new BaseModel { Success = true, Message = "Not Found" };
        }
        public FilterParams ConvertToFilterParams(StaffRequest request)
        {
            var filterParams = new FilterParams
            {
                PaginationParam = request.PaginationParam
            };

            if (!string.IsNullOrEmpty(request.RoleId))
            {
                filterParams.Filters.Add(new Filter { PropertyName = "RoleId", Value = request.RoleId });
            }
            if (!string.IsNullOrEmpty(request.DepartmentId))
            {
                filterParams.Filters.Add(new Filter { PropertyName = "DepartmentId", Value = request.DepartmentId });
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                filterParams.Filters.Add(new Filter { PropertyName = "Name", Value = request.Name });
            }

            return filterParams;
        }

    }
}

