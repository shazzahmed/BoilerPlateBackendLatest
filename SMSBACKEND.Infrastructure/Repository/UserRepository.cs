using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Domain.RepositoriesContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static Common.Utilities.Enums;

namespace Infrastructure.Repository
{
    public class UserRepository : BaseRepository<ApplicationUser, string>, IUserRepository
    {
        private readonly IMapper _mapper;
        protected readonly UserManager<ApplicationUser> _applicationUserManager;
        public UserRepository(ISqlServerDbContext context, 
            UserManager<ApplicationUser> applicationUserManager,
            IMapper mapper) : base(context)
        {
            _applicationUserManager = applicationUserManager;
            _mapper = mapper;
        }
        public async Task<List<string>> GetAdminUserRoles()
        {
            var otherRoles = new List<string> { "", "" };
            //var roles = await DbContext.Roles.Where(x => !otherRoles.Contains(x.Name)).Select(x => x.Name).ToListAsync();
            var roles = await DbContext.User.Where(x => !otherRoles.Contains(x.UserName)).Select(x => x.UserName).ToListAsync();
            return roles;
        }

        public async Task<List<ApplicationUser>> GetUsersByRoles(List<string> userRoles)
        {
            var roles = await DbContext.User.Where(x => userRoles.Contains(x.UserName)).Select(c => c.Id).ToListAsync();
            var result = (await DbContext.User.Where(x => x.UserRoles.Any(c => roles.Contains(c.RoleId))).ToListAsync())
                            //.Select(x => new UserDetailsModel
                            //{
                            //    Id = x.Id,
                            //    Email = x.Email,
                            //    Name = x.FirstName + " " + x.LastName
                            //})
                            .ToList();
            return result;
        }
        public string GetRolesForUserById(string userId)
        {
            //using (
            //    var userManager =
            //        new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            //{
            //    var rolesForUser = userManager.GetRoles(userId);
            //    return rolesForUser.FirstOrDefault();
            //}
            return "";
        }
        public string GetRolesForUserByEmail(string email)
        {
            //using (
            //    var userManager =
            //        new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            //{
            //    var rolesForUserbyemail = userManager.FindByEmail(email);
            //    var rolesForUser = userManager.GetRoles(rolesForUserbyemail.Id);
            //    return rolesForUser.FirstOrDefault();
            //}
            return "";
        }
        public async Task<bool> IsEmailAlreadyExists(string email)
        {
            return await DbContext.User.AnyAsync(x => x.Email.Equals(email) || x.UserName.Equals(email));
        }
        public async Task<bool> IsStudentExists(string email, string ParentId)
        {
            return await DbContext.User.AnyAsync(x => (x.Email.Equals(email) || x.UserName.Equals(email))&& x.ParentId == ParentId);
        }
        public async Task<bool> IsCNICAlreadyExist(string CNIC, string role)
        {
            return await DbContext.User
            .Include(u => u.UserRoles) // Ensure roles are loaded
            .ThenInclude(ur => ur.Role) // Load associated roles
            .AnyAsync(x => x.CNIC == CNIC ); //&& x.UserRoles.Any(ur => ur.Role.Name == role)
            //return await DbContext.User.AnyAsync(x => x.CNIC.Equals(CNIC));
        }

        public async Task<bool> IsPhoneAlreadyExists(string phone)
        {
            return await DbContext.User.AnyAsync(x => x.PhoneNumber.Equals(phone));
        }
        
        public async Task<bool> IsPasswordChangeRequired(string userId)
        {
            var user = await FirstOrDefaultAsync( x => x.Id.Equals(userId));
            return user != null ? user.ForceChangePassword : false;
        }
    }
}
