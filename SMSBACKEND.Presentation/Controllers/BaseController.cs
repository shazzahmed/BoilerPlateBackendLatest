using IdentityModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public string GetUserId()
        {
            var identity = User;
            return identity.Claims
                           .Where(c => c.Type == JwtClaimTypes.Id)
                           .Select(c => c.Value)
                           .SingleOrDefault();
        }
        [NonAction]
        public string GetUserName()
        {
            var identity = User;
            return identity.Claims
                           .Where(c => c.Type == JwtClaimTypes.Name)
                           .Select(c => c.Value)
                           .SingleOrDefault();
        }
        [NonAction]
        public string GetUserEmail()
        {
            var identity = User;
            return identity.Claims
                           .Where(c => c.Type == ClaimTypes.Email)
                           .Select(c => c.Value)
                           .SingleOrDefault();
        }
        [NonAction]
        public string GetUserFullName()
        {
            var identity = User;
            var firstName = identity.Claims
                           .Where(c => c.Type == ClaimTypes.GivenName)
                           .Select(c => c.Value)
                           .SingleOrDefault();

            var lastName = identity.Claims
                           .Where(c => c.Type == ClaimTypes.Surname)
                           .Select(c => c.Value)
                           .SingleOrDefault();

            return  $"{firstName} {lastName}";
        }
        [NonAction]
        public string GetUserFirstName()
        {
            var identity = User;
            return identity.Claims
                           .Where(c => c.Type == ClaimTypes.GivenName)
                           .Select(c => c.Value)
                           .SingleOrDefault();
        }
        [NonAction]
        public string GetUserLastName()
        {
            var identity = User;
            return identity.Claims
                           .Where(c => c.Type == ClaimTypes.Surname)
                           .Select(c => c.Value)
                           .SingleOrDefault();
        }
        [NonAction]
        public bool UserIsInRole(string role)
        {
            var identity = User;
            var actualRole = identity.Claims
                                     .Where(c => c.Type == ClaimTypes.Role)
                                     .Select(c => c.Value)
                                     .SingleOrDefault();
            return actualRole == role;
        }
    }
}
